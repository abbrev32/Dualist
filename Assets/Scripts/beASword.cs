using UnityEngine;

public class SwordMouseRotate : MonoBehaviour
{
    public float rotationSpeed = 5f; // Adjust to your liking
    public Transform swordPivot; // The empty pivot object

    void Update()
    {
        // Get mouse horizontal movement
        float mouseX = Input.GetAxis("Mouse X");

        // Rotate pivot based on mouse movement
        swordPivot.Rotate(Vector3.forward, mouseX * rotationSpeed);
    }
}
