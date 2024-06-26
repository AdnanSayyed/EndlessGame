using EndlessGame.ObjectPool;
using EndlessGame.Spawnable;
using UnityEngine;
using EndlessGame.Service;
using EndlessGame.Constant;
using EndlessGame.Manager;

namespace EndlessGame.Player
{
    public class PlayerController : MonoBehaviour, IPlayerController
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

        private enum PlayerState { Running, Jumping, Sliding, Dead }
        private PlayerState currentState = PlayerState.Running;


        private IInputService inputService;

        private Vector3 spawnPos;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            spawnPos = transform.position; 
        }

        void Start()
        {
            originalHeight = characterController.height;
            originalCenter = characterController.center;

            ServiceLocator.RegisterService<IPlayerController>(this);

            inputService = ServiceLocator.GetService<IInputService>();
        }

        private void Update()
        {
            if (inputService.IsJumpPressed() && currentState == PlayerState.Running)
            {
                JumpPlayer();
                currentState = PlayerState.Jumping;
                animator.SetTrigger("isJumping");
            }
            else if (inputService.IsSlidePressed() && currentState == PlayerState.Running)
            {
                StartSlide();
            }

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

        public void EndSlide()
        {
            currentState = PlayerState.Running;
            characterController.height = originalHeight;
            characterController.center = originalCenter;
        }

        public void DeathAnimFinished()
        {
            GameManager.Instance.EndGame();
        }

        public void MovePlayer()
        {
            if (currentState == PlayerState.Dead)
                return;

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

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag(Constants.CollectableTag))
            {
                HandleCollisionWithCollectable(hit.gameObject);
            }

            if (hit.gameObject.CompareTag(Constants.ObstacleTag))
            {
                HandleCollisionWithObstacle();
            }
        }

        private void HandleCollisionWithCollectable(GameObject collidedObject)
        {
            SpawnableBase spawnable = collidedObject.GetComponent<SpawnableBase>();
            var objectPooler = ServiceLocator.GetService<IObjectPooler>();
            objectPooler.ReturnToPool(spawnable.SpawnableTag, collidedObject);
        }

        private void HandleCollisionWithObstacle()
        {
            currentState = PlayerState.Dead;
            animator.SetTrigger("isDead");
            velocity = Vector3.zero;
        }

        public Transform GetTransform()
        {
            return transform;
        }


        public void ResetService()
        {
            // Reset player state and position
            characterController.enabled = false; // Disable to reset position
            transform.position = spawnPos;
            characterController.enabled = true; // Re-enable controller
            currentState = PlayerState.Running;

            animator.Rebind();
            animator.Update(0f);
        }
    }
}