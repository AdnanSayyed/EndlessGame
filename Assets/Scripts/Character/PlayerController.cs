using EndlessGame.ObjectPool;
using EndlessGame.Spawnable;
using UnityEngine;
using EndlessGame.Service;
using EndlessGame.Constant;
using EndlessGame.Manager;
using EndlessGame.Powerup;
using System;
using EndlessGame.Score;

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
        [SerializeField]
        private float groundCheckDistance = 0.2f;
        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private float speedIncrementFactor = 0.001f;

        private CharacterController characterController;
        private Animator animator;

        private Vector3 velocity;
        private bool isGrounded;

        private float originalHeight;
        private Vector3 originalCenter;

        private enum PlayerState { Running, Jumping, Sliding, Dead }
        private PlayerState currentState = PlayerState.Running;

        private IInputService inputService;
        private IPowerUpService powerUpService;
        private IScoreService scoreManager;

        private Vector3 spawnPos;
        private bool isInvincible = false;
        private bool isJumpBoostActive;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            spawnPos = transform.position;

            powerUpService = ServiceLocator.GetService<IPowerUpService>();
            scoreManager = ServiceLocator.GetService<IScoreService>();
        }

        void Start()
        {
            originalHeight = characterController.height;
            originalCenter = characterController.center;

            ServiceLocator.RegisterService<IPlayerController>(this);

            inputService = ServiceLocator.GetService<IInputService>();
        }

        private void FixedUpdate()
        {
            CheckGroundStatus();
        }


        private void Update()
        {

            if (inputService.IsJumpPressed() && isGrounded && currentState == PlayerState.Running)
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

            powerUpService.Update(Time.deltaTime);

            float distanceTraveled = transform.position.x - spawnPos.x;

            // Adjust the move speed based on the distance traveled
            moveSpeed += distanceTraveled * speedIncrementFactor * Time.deltaTime;

            // Adjusting the score increment based on scoreFactor
            scoreManager.SetScore((int)distanceTraveled);
        }

        private void CheckGroundStatus()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);
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
                float effectiveJumpHeight = isJumpBoostActive ? jumpHeight * 2f : jumpHeight;
                velocity.y = Mathf.Sqrt(effectiveJumpHeight * -2f * gravity);

            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag(Constants.CollectableTag))
            {
                HandleCollisionWithCollectable(hit.gameObject);
            }

            if (!isInvincible)
            {
                if (hit.gameObject.CompareTag(Constants.ObstacleTag))
                {
                    HandleCollisionWithObstacle();
                }
            }
        }

        private void HandleCollisionWithCollectable(GameObject collidedObject)
        {
            SpawnableBase spawnable = collidedObject.GetComponent<SpawnableBase>();
            var objectPooler = ServiceLocator.GetService<IObjectPooler>();
            objectPooler.ReturnToPool(spawnable.SpawnableTag, collidedObject);

            Collectable collectable = null;
            collidedObject.TryGetComponent<Collectable>(out collectable);

            if (collectable != null)
            {
                powerUpService.ActivatePowerUp(collectable.PowerUpType, this);
            }
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

        public void SetInvincible(bool invincible)
        {
            isInvincible = invincible;

            gameObject.layer = isInvincible ?
                LayerMask.NameToLayer(Constants.InvincibleLayer)
                : LayerMask.NameToLayer(Constants.DefaultLayer);
        }

        public void SetJumpBoostActive(bool active)
        {
            isJumpBoostActive = active;
        }

        public void ResetService()
        {
            // Reset player state and position
            characterController.enabled = false;
            transform.position = spawnPos;
            characterController.enabled = true;
            currentState = PlayerState.Running;

            // Reset animation controller
            animator.Rebind();
            animator.Update(0f);

            // Deactivate all active power-ups
            foreach (PowerUpType activePowerUp in powerUpService.GetActivePowerUps())
            {
                powerUpService.DeactivatePowerUp(activePowerUp);
            }
        }
    }
}
