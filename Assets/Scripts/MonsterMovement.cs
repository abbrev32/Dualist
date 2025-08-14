using Mirror;
using Mirror.Examples.CCU;
using UnityEngine;

public class MonsterMovement : NetworkBehaviour
{
    public float speed = 3f;  // Movement speed
    public float leftBound = -10f;  // Left edge X position
    public float rightBound = 10f;  // Right edge X position

    private bool movingLeft = true;

    void Update()
    {
        if(!isServer) return;

        if (movingLeft)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound)
                movingLeft = false;
        }
        else
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound)
                movingLeft = true;
        }
    }
}