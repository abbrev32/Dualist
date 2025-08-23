using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform playerPos;
    private readonly float smoothing = 0.1f;
    private readonly float jumpSmoothing = 0.05f;
    public Vector3 offset = new(0, -15f, 0);

    public Vector3 velocity = Vector3.zero;

    public Vector2 minBounds;
    public Vector2 maxBounds;
    void LateUpdate()
    {
        if (playerPos == null) return;

        float newX = Mathf.Lerp(transform.position.x, playerPos.position.x, smoothing);
        float newY = Mathf.Lerp(transform.position.y, playerPos.position.y + offset.y, jumpSmoothing);
        Vector3 smoothPos = new(newX, newY, transform.position.z);

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        float clampedX = Mathf.Clamp(smoothPos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        float clampedY = Mathf.Clamp(smoothPos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        //transform.position = Vector3.Lerp(transform.position, playerPos.position + offset, smoothing);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        //transform.LookAt(transform);
    }
}
