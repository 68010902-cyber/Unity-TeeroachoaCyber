using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Start()
    {
        mainCamera = Camera.main;

        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, Mathf.Abs(mainCamera.transform.position.z)));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Mathf.Abs(mainCamera.transform.position.z)));

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        float halfWidth = spriteRenderer != null ? spriteRenderer.bounds.extents.x : 0.5f;
        float halfHeight = spriteRenderer != null ? spriteRenderer.bounds.extents.y : 0.5f;

        float paddingX = 0.3f;
        float paddingY = 0.3f;

        minBounds = new Vector2(bottomLeft.x + halfWidth + paddingX, bottomLeft.y + halfHeight + paddingY);
        maxBounds = new Vector2(topRight.x - halfWidth - paddingX, topRight.y - halfHeight - paddingY);
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos;
    }
}
