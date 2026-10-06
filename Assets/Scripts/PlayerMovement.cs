using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
 public float moveSpeed = 5f;
 public float jumpForce = 5f;
 public Transform groundCheck;
 public float groundDistance = 0.4f;
 public LayerMask groundMask;
 private Rigidbody rb;
 private Vector2 moveInput;
 private PlayerInput playerInput;
 public AudioClip footstepSound;
 private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
        StartCoroutine(PlayFootStep());
    }

    void Update()
    {
        CheckGrounded();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
     
    void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction.Normalize();
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z *moveSpeed);
    }

    IEnumerator PlayFootStep()
    {
        while (true)
        {
            if (rb.linearVelocity.magnitude > 0.1f && isGrounded)
            {
                AudioManager.Instance.PlaySFX(footstepSound, 0.5f);
                
            }
             yield return new WaitForSeconds(0.5f); 
        }
    }
}
