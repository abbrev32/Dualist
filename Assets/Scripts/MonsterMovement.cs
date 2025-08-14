using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float speed = 3f;
    public float leftBound = -10f;
    public float rightBound = 10f;
    private bool movingRight = true;

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound)
                movingRight = true;
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
