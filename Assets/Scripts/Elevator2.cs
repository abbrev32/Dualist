using UnityEngine;
using Mirror;
using System.Collections;

public class Elevator2 : NetworkBehaviour
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform startPoint;
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
    private void RpcMoveElevator()
    {
        StartCoroutine(MoveElevatorRoutine());
    }

    private IEnumerator MoveElevatorRoutine()
    {
        Vector3 start = transform.position;
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
