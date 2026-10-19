const SPREADSHEET_ID = "1UFKcoNFXt2XYhWucsPbPN878E0_lMlvTaqzrd5R7S44";
const SHEET_NAME = "GameResults";
const HEADERS = ["Timestamp", "RunId", "Player", "Score", "DurationSeconds", "Difficulty", "PlayCount"];

function doGet() {
  return jsonResponse({ success: true, message: "Google Sheets endpoint is ready." });
}

function doPost(e) {
  if (!e || !e.postData || !e.postData.contents) {
    return jsonResponse({ success: false, message: "Missing JSON request body." });
  }

  let record;
  try {
    record = JSON.parse(e.postData.contents);
  } catch (error) {
    return jsonResponse({ success: false, message: "Invalid JSON request body." });
  }

  const validationError = validateRecord(record);
  if (validationError) {
    return jsonResponse({ success: false, message: validationError });
  }

  const lock = LockService.getScriptLock();
  lock.waitLock(10000);
  try {
    const sheet = getResultsSheet();
    const values = sheet.getDataRange().getValues();

    for (let row = 1; row < values.length; row++) {
      if (String(values[row][1]) === record.runId) {
        return jsonResponse({
          success: true,
          message: "Record already saved.",
          playCount: Number(values[row][6])
        });
      }
    }

    const normalizedPlayer = record.player.trim().toLocaleLowerCase();
    let playCount = 0;
    for (let row = 1; row < values.length; row++) {
      if (String(values[row][2]).trim().toLocaleLowerCase() === normalizedPlayer) {
        playCount = Math.max(playCount, Number(values[row][6]) || 0);
      }
    }
    if (record.incrementPlayCount !== false) {
      playCount++;
    }

    const nextRow = sheet.getLastRow() + 1;
    const range = sheet.getRange(nextRow, 1, 1, HEADERS.length);
    range.getCell(1, 2).setNumberFormat("@");
    range.getCell(1, 3).setNumberFormat("@");
    range.getCell(1, 6).setNumberFormat("@");
    range.setValues([[
      new Date(),
      safeText(record.runId),
      safeText(record.player.trim()),
      record.score,
      Math.round(record.durationSeconds * 10) / 10,
      safeText(record.difficulty),
      playCount
    ]]);

    return jsonResponse({
      success: true,
      message: "Record saved.",
      playCount: playCount
    });
  } finally {
    lock.releaseLock();
  }
}

function validateRecord(record) {
  if (!record || typeof record !== "object") {
    return "Request must be a JSON object.";
  }
  if (typeof record.runId !== "string" || !/^[a-fA-F0-9]{32}$/.test(record.runId)) {
    return "Invalid run ID.";
  }
  if (typeof record.player !== "string" || !record.player.trim() || record.player.length > 100) {
    return "Player name must contain 1 to 100 characters.";
  }
  if (!Number.isInteger(record.score) || record.score < 0 || record.score > 1000000000) {
    return "Score must be a non-negative integer.";
  }
  if (typeof record.durationSeconds !== "number" ||
      !Number.isFinite(record.durationSeconds) ||
      record.durationSeconds < 0 ||
      record.durationSeconds > 3600) {
    return "Duration must be between 0 and 3600 seconds.";
  }
  if (record.incrementPlayCount !== undefined && typeof record.incrementPlayCount !== "boolean") {
    return "Play count increment flag must be a boolean.";
  }
  if (["Easy", "Normal", "Hard"].indexOf(record.difficulty) < 0) {
    return "Unknown difficulty level.";
  }
  return "";
}

function getResultsSheet() {
  const spreadsheet = SpreadsheetApp.openById(SPREADSHEET_ID);
  let sheet = spreadsheet.getSheetById(0) || spreadsheet.getSheetByName(SHEET_NAME);
  if (!sheet) {
    sheet = spreadsheet.insertSheet(SHEET_NAME);
  }

  if (sheet.getLastRow() === 0) {
    sheet.getRange(1, 1, 1, HEADERS.length).setValues([HEADERS]);
    sheet.setFrozenRows(1);
  }
  sheet.hideColumns(2);
  return sheet;
}

function safeText(value) {
  const text = String(value);
  return /^[=+\-@]/.test(text) ? "'" + text : text;
}

function jsonResponse(value) {
  return ContentService
    .createTextOutput(JSON.stringify(value))
    .setMimeType(ContentService.MimeType.JSON);
}
