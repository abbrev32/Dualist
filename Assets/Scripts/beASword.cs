using Mirror;
using UnityEngine;

public class SwordMouseRotate : MonoBehaviour
{
    public float rotationSpeed = 5f; // Adjust to your liking
    public Transform swordPivot; // The empty pivot object
    float currentAngle = 0f;//rotation memory

    SoundEFX soundEFX;
    private void Awake()
    {
        soundEFX = GetComponent<SoundEFX>();
        

    }
    void Update()
    {
        // Get mouse horizontal movement
        float mouseX = Input.GetAxis("Mouse X");

        currentAngle += mouseX * rotationSpeed;//rotation update
        // Rotate pivot based on mouse movement
        swordPivot.localRotation = Quaternion.Euler(0f,0f,currentAngle);
        soundEFX.PlaySFX(soundEFX.sword_waving);// adding sound effect

    }
}
