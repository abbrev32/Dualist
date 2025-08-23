using UnityEngine;
using Mirror;

public class Elevator : NetworkBehaviour
{
    [Header("Points (world positions)")]
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Settings")]
    public float speed = 2f;

    [SyncVar] private bool isMoving = false;    
    [SyncVar] private Vector3 targetPos;        

    public override void OnStartServer()
    {
        if (topPoint == null || bottomPoint == null)
        {
            Debug.LogError("[Elevator] TopPoint/BottomPoint not assigned.");
            enabled = false;
            return;
        }
        transform.position = topPoint.position;

        // Start idle
        targetPos = transform.position;
        isMoving = false;
    }

    void Update()
    {
        if (!isServer) return;
        if (!isMoving) return;

        // Move elevator toward target
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Stop if reached target
        if ((transform.position - targetPos).sqrMagnitude <= 0.0001f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }

    // Player calls this
    [Server]
    public void MoveToBottom()
    {
        targetPos = bottomPoint.position;
        isMoving = true;
    }

    // Monster calls this
    [Server]
    public void MoveToTop()
    {
        targetPos = topPoint.position;
        isMoving = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (topPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(topPoint.position, 0.1f);
        }
        if (bottomPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bottomPoint.position, 0.1f);
        }
        if (topPoint != null && bottomPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(topPoint.position, bottomPoint.position);
        }
    }
#endif
}
