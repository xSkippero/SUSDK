using System.Linq;
using UnityEngine;

namespace SUSDK.Controller
{
    public class MovementController3D : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("The movement speed of the player.")]
        public float moveSpeed = 5f;

        [Tooltip("Movement speed multiplier when crouching.")]
        public float crouchSpeedMultiplier = 0.5f;

        [Tooltip("Controls how much the player can move in the air (0 = no control, 1 = full control).")]
        [Range(0f, 1f)]
        public float airControlFactor = 0.1f;

        [Header("Jump Settings")]
        [Tooltip("The force applied for jumping.")]
        public float jumpForce = 7f;

        [Tooltip("How many times the player can jump (e.g., 2 for double jump).")]
        [Min(1)]
        public int maxJumpCount = 1;

        [Tooltip("Allow crouching while in the air.")]
        public bool allowAirCrouch = false;

        [Header("Runtime States")]
        [Tooltip("Indicates if the player is grounded.")]
        public bool isGrounded;

        [Tooltip("Indicates if the player is crouching.")]
        public bool isCrouching;

        private Rigidbody _rb;
        private int _jumpCount;
        private Vector2 _moveInput;

        public void Move(Vector2 direction)
        {
            _moveInput = direction;
        }

        public bool Jump()
        {
            if (_jumpCount >= maxJumpCount) return false;
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpCount++;
            isGrounded = false;
            return true;
        }

        public void Crouch(bool crouch)
        {
            if (!isGrounded && !allowAirCrouch) return;
            isCrouching = crouch;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
                Debug.LogError("MovementController3D requires a Rigidbody.");
        }
        
        private void CheckGrounded()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            var speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
            var targetVelocity = new Vector3(_moveInput.x * speed, _rb.linearVelocity.y, _moveInput.y * speed);

            if (isGrounded)
            {
                _rb.linearVelocity = targetVelocity;
            }
            else
            {
                var currentVelocity = _rb.linearVelocity;
                var horizontalTarget = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
                var horizontalCurrent = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
                var newHorizontal = Vector3.Lerp(horizontalCurrent, horizontalTarget, airControlFactor);
                _rb.linearVelocity = new Vector3(newHorizontal.x, currentVelocity.y, newHorizontal.z);
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.contacts.Any(contact => contact.normal.y > 0.5f)) return;
            isGrounded = true;
            _jumpCount = 0;
        }
    }
}