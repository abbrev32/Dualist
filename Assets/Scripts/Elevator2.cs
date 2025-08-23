using UnityEngine;
using Mirror;

public class Elevator2 : NetworkBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed = 2f;

    private bool activated = false;

    [Server]
    public void ActivateElevator()
    {
        if (!activated)
        {
            activated = true;
            RpcMoveElevator();
        }
    }

    [ClientRpc]
    void RpcMoveElevator()
    {
        StartCoroutine(MoveElevatorRoutine());
    }

    private System.Collections.IEnumerator MoveElevatorRoutine()
    {
        Vector3 start = transform.position;
        // if elevator is closer to startPoint → move to endPoint, else go back to startPoint
        Vector3 target = (Vector3.Distance(start, startPoint.position) < Vector3.Distance(start, endPoint.position))
                         ? endPoint.position : startPoint.position;

        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
    }
}
