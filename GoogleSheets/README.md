# Google Sheets setup

This integration records a completed game's score when the player chooses **Retry** or **Home** on the results screen. It stores the server timestamp, player name, score, elapsed game time in seconds, difficulty, and that player's play count. Choosing **Retry** increases PlayCount. Choosing **Home** increases it only if the player has already chosen Retry during the current login session; otherwise, Home saves the score without increasing it. The RunID column is hidden in the spreadsheet and retained only to prevent duplicate rows if an upload is retried. Play counts match player names without regard to letter case or surrounding spaces.

## Deploy the Apps Script

1. Open the [configured results spreadsheet](https://docs.google.com/spreadsheets/d/1UFKcoNFXt2XYhWucsPbPN878E0_lMlvTaqzrd5R7S44/edit).
2. In that spreadsheet, choose **Extensions > Apps Script** and replace the editor contents with [`Code.gs`](./Code.gs).
3. Save the project. The script is configured to write to the first tab (the tab with `gid=0` in the shared link); it creates a `GameResults` tab if that tab is unavailable.
4. Choose **Deploy > Manage deployments**, edit the existing web app deployment, select **New version**, verify **Execute as** is your account and access is **Anyone**, then deploy the new version. This updates the existing `/exec` endpoint used by Unity.
5. If there is no existing deployment, choose **Deploy > New deployment**, select **Web app**, set **Execute as** to your account and access to **Anyone**, deploy, grant the requested permissions, and copy the Web app URL ending in `/exec`.
6. If the copied URL differs from the one in [`GoogleSheetsReporter.cs`](../Assets/Scene/GoogleSheetsReporter.cs), replace `WebAppUrl` with the new URL.
7. Return to Unity and wait for scripts to compile. The header row is created automatically when the first result is submitted.

The deployed endpoint is public so a game client can submit without Google sign-in. It validates incoming fields and prevents duplicate rows when a queued request is retried, but it does not authenticate players; names and scores submitted by a client should not be treated as verified or private data. A completed round is held until the player chooses **Retry** or **Home**. If an upload fails, the result is kept in local `PlayerPrefs` and retried the next time the game starts or another round is submitted.
