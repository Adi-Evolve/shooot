using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerLook : MonoBehaviour
{
    public  float mouseSensitivity = 200f;
    public Transform cam;
    public static PlayerLook Instance;
    private float xRotation = 0f;
    private Vector2 lookInput;
    private float ShakeDuration = 0f;
    private float ShakeMagnitude = 0.1f;
    private float shakeFadeSpeed =1.5f;
    private Vector3 initalCamPos;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initalCamPos = cam.localPosition;
    }

    void Update()
    {
        HandleMouseLook();
        HandleShake();
    }

    public void OnLook (InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX); 
    }
    void HandleShake()
    {
        if (ShakeDuration > 0)
        {
            cam.localPosition = initalCamPos + Random.insideUnitSphere * ShakeMagnitude;
            ShakeDuration -= Time.deltaTime * shakeFadeSpeed;
        }
        else
        {
            cam.localPosition = initalCamPos;
        }
    }
    public void AddShake(float duration, float magnitude)
    {
        ShakeDuration = duration;
        ShakeMagnitude = magnitude;
    }
}
