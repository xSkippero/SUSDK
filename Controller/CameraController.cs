using UnityEngine;

namespace SUSDK.Controller
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("The transform that the camera should follow.")]
        public Transform target;

        [Header("Offset")]
        [Tooltip("The offset of the camera relative to the target position.")]
        public Vector3 offset = new Vector3(0, 5, -10);

        [Header("Follow Axes")]
        [Tooltip("Should the camera follow the target on the X-axis?")]
        public bool followX = true;

        [Tooltip("Should the camera follow the target on the Y-axis?")]
        public bool followY = true;

        [Tooltip("Should the camera follow the target on the Z-axis?")]
        public bool followZ = true;

        [Header("Smoothing")]
        [Tooltip("Enable smooth following.")]
        public bool smoothFollow = true;

        [Tooltip("Smoothing speed for each axis.")]
        public Vector3 smoothSpeed = new Vector3(5f, 5f, 5f);

        [Header("Rotation Follow")]
        [Tooltip("Should the camera follow the target's rotation?")]
        public bool followRotation = false;

        [Tooltip("Should the camera's rotation adjustment be smooth?")]
        public bool smoothRotation = true;

        [Tooltip("Rotation smoothing speed.")]
        [Min(0f)]
        public float rotationSmoothSpeed = 5f;

        private void LateUpdate()
        {
            if (!target) return;

            var currentPosition = transform.position;
            var targetPosition = target.position + offset;

            var newPosition = currentPosition;

            if (followX)
            {
                newPosition.x = smoothFollow
                    ? Mathf.Lerp(currentPosition.x, targetPosition.x, Time.deltaTime * smoothSpeed.x)
                    : targetPosition.x;
            }

            if (followY)
            {
                newPosition.y = smoothFollow
                    ? Mathf.Lerp(currentPosition.y, targetPosition.y, Time.deltaTime * smoothSpeed.y)
                    : targetPosition.y;
            }

            if (followZ)
            {
                newPosition.z = smoothFollow
                    ? Mathf.Lerp(currentPosition.z, targetPosition.z, Time.deltaTime * smoothSpeed.z)
                    : targetPosition.z;
            }

            transform.position = newPosition;

            if (!followRotation) return;
            transform.rotation = smoothRotation ? Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationSmoothSpeed) : target.rotation;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
