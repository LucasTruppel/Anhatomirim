using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    CharacterController controller;

    Animator animator;

    [SerializeField] Transform cam;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float defaultJumpHeight;
    [SerializeField] float gravity;

    float jumpHeight;
    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Vector2 movement;
    float currentSpeed;
    bool isGrounded;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;
        jumpHeight = defaultJumpHeight;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        if (isGrounded && velocity.y <= 0)
        {
            animator.SetBool("isJumping", false);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = walkSpeed;
        }

        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 direction = new Vector3(movement.x, 0f, movement.y).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
            // change animation to running
            animator.SetBool("isRunning", true);
            Debug.Log("running");
        } else {
            // change animation to idle
            animator.SetBool("isRunning", false);
        }


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 10 * -2 * gravity);
            animator.SetBool("isJumping", true);
            Debug.Log("jumped");
        }

        if (velocity.y > -20) {
            velocity.y += (gravity * 10) * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -30)
        {
            transform.position = new Vector3(0, 1, 0);
            velocity = Vector3.zero;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) 
    { 
        if (hit.gameObject.tag == ("Jump Platform")) 
        { 
            jumpHeight = 2 * defaultJumpHeight;
        } else {
            jumpHeight = defaultJumpHeight;
        }
    }

    
}
