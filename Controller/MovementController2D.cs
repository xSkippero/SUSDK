using System.Linq;
using UnityEngine;

namespace SUSDK.Controller
{
    public class MovementController2D : MonoBehaviour
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
        
        private Rigidbody2D _rb;
        private int _jumpCount;
        private Vector2 _moveInput;

        public void Move(Vector2 direction)
        {
            _moveInput = direction;
        }

        public bool Jump()
        {
            if (_jumpCount >= maxJumpCount) return false;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
                Debug.LogError("SimpleMover2D requires Rigidbody2D.");
        }
        
        private void CheckGrounded()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            var speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
            var targetVelocityX = _moveInput.x * speed;
            if (isGrounded)
            {
                _rb.linearVelocity = new Vector2(targetVelocityX, _rb.linearVelocity.y);
            }
            else
            {
                var velocityX = Mathf.Lerp(_rb.linearVelocity.x, targetVelocityX, airControlFactor);
                _rb.linearVelocity = new Vector2(velocityX, _rb.linearVelocity.y);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.contacts.Any(contact => contact.normal.y > 0.5f)) return;
            isGrounded = true;
            _jumpCount = 0;
        }

    }
}