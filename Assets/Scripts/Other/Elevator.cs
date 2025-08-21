using UnityEngine;
using Mirror;

public class Elevator : NetworkBehaviour
{
    [Header("Points (world positions)")]
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Settings")]
    public float speed = 2f;

    [SyncVar] private bool isMoving = false;    // elevator moving state
    [SyncVar] private Vector3 targetPos;        // current target

    public override void OnStartServer()
    {
        if (topPoint == null || bottomPoint == null)
        {
            Debug.LogError("[Elevator] TopPoint/BottomPoint not assigned.");
            enabled = false;
            return;
        }

        // Elevator starts wherever it is, no movement until lever is pressed
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

    [Server]
    public void ToggleElevator()
    {
        // Switch target to opposite point
        if (Vector3.Distance(transform.position, topPoint.position) < 0.01f)
        {
            targetPos = bottomPoint.position; // currently at top, go down
        }
        else if (Vector3.Distance(transform.position, bottomPoint.position) < 0.01f)
        {
            targetPos = topPoint.position; // currently at bottom, go up
        }
        else
        {
            // If somewhere in between, move to the opposite of current target
            targetPos = (targetPos == topPoint.position) ? bottomPoint.position : topPoint.position;
        }

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