using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class EntityChecker : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GetComponent<GameManager>().LevelClear();
        }
    }
}
