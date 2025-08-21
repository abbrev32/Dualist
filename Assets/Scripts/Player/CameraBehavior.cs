using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform playerPos;
    private readonly float smoothing = 1f;
    private readonly float jumpSmoothing = 0.6f;
    public Vector3 offset = new(0, -15f, 0);

    public Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (playerPos == null) return;
        Debug.Log("Camera Active: " + gameObject.activeSelf + " | Enabled: " + enabled);
        //transform.position = Vector3.Lerp(transform.position, playerPos.position + offset, smoothing);
        float newX = Mathf.Lerp(transform.position.x, playerPos.position.x, smoothing);
        float newY = Mathf.Lerp(transform.position.y, playerPos.position.y + offset.y, jumpSmoothing);
        transform.position = new Vector3(newX, newY, transform.position.z);
        //transform.LookAt(transform);
    }
}
