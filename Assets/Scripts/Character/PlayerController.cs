using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;
    [SerializeField]
    private float jumpHeight = 1.5f;
    [SerializeField]
    private float gravity = -9.81f;

    private CharacterController characterController;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private float originalHeight;
    private Vector3 originalCenter;

    private enum PlayerState { Running, Jumping, Sliding }
    private PlayerState currentState = PlayerState.Running;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        originalHeight = characterController.height;
        originalCenter = characterController.center;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && currentState == PlayerState.Running)
        {
            JumpPlayer();
            currentState = PlayerState.Jumping;
            animator.SetTrigger("isJumping");

        }
        else if (Input.GetKeyDown(KeyCode.S) && currentState == PlayerState.Running)
        {
            StartSlide();

        }
    }
    private void FixedUpdate()
    {
        MovePlayer();

    }

    public void SetRunningState()
    {
        currentState = PlayerState.Running;
    }

    private void StartSlide()
    {
        currentState = PlayerState.Sliding;
        animator.SetTrigger("isSliding");

        characterController.height = originalHeight / 3;
        characterController.center = new Vector3(originalCenter.x, originalCenter.y / 3, originalCenter.z);

    }

    private void EndSlide()
    {
        currentState = PlayerState.Running;

        characterController.height = originalHeight;
        characterController.center = originalCenter;
    }


    public void MovePlayer()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        Vector3 move = transform.forward * moveSpeed * Time.deltaTime;
        characterController.Move(move);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    public void JumpPlayer()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
