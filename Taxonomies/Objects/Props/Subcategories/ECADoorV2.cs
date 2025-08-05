using UnityEngine;
using System.Collections;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Utils;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// Custom door component controllable via ECA actions.
    /// Handles open/close with rotation around a hinge, and lock/unlock logic.
    /// </summary>
    [ECARules4All("door_custom")]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(Interactable))]
    [DisallowMultipleComponent]
    public class ECADoorV2 : MonoBehaviour
    {
        [Header("Door Settings")]

        /// <summary>
        /// <b>isLocked</b>: If true, the door is locked and cannot be opened.
        /// </summary>
        public bool isLocked = false;

        /// <summary>
        /// <b>openAngle</b>: Target angle for the door when fully open.
        /// </summary>
        public float openAngle = 90f;

        /// <summary>
        /// <b>hingeAxis</b>: The axis around which the door rotates.
        /// </summary>
        public Vector3 hingeAxis = Vector3.up;

        /// <summary>
        /// <b>openSpeed</b>: Duration (in seconds) to fully open/close the door.
        /// </summary>
        public float openSpeed = 2.0f;

        [Header("Hinge Calculation")]

        /// <summary>
        /// <b>hingeLocalOffset</b>: Local offset from the object origin to place the hinge pivot.
        /// </summary>
        public Vector3 hingeLocalOffset = new Vector3(-0.5f, 0, 0);

        private Transform hinge;
        private Quaternion _closedRotationHinge;
        private Quaternion _openRotationHinge;
        private bool isOpen = false;
        private bool isRotating = false;

        /// <summary>
        /// Initializes the hinge pivot and precomputes open/closed rotations.
        /// </summary>
        void Awake()
        {
            CreateHingePivot();
            _closedRotationHinge = hinge.localRotation;
            _openRotationHinge = _closedRotationHinge * Quaternion.Euler(hingeAxis.normalized * openAngle);
            isOpen = Quaternion.Angle(hinge.localRotation, _openRotationHinge) < 1.0f;
        }

        /// <summary>
        /// Creates a hinge pivot object at the specified offset and parents this object to it.
        /// </summary>
        void CreateHingePivot()
        {
            GameObject hingeObj = new GameObject(gameObject.name + "_HingePivot");
            hinge = hingeObj.transform;

            Vector3 worldHingePoint = transform.TransformPoint(hingeLocalOffset);
            hinge.position = worldHingePoint;
            hinge.rotation = transform.rotation;
            hinge.SetParent(transform.parent);
            transform.SetParent(hinge);
        }

        /// <summary>
        /// ECA action to open the door, if it's not locked or already open.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECADoorV2), "open")]
        public void OpenDoorAction()
        {
            if (isRotating || isOpen) return;

            if (isLocked)
            {
                Debug.Log($"[{gameObject.name}] Door is locked, flickering.");
                StartCoroutine(FlickedDoor());
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Opening door...");
                StartCoroutine(RotateDoor(_openRotationHinge));
            }
        }

        /// <summary>
        /// ECA action to close the door, if it's currently open.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECADoorV2), "close")]
        public void CloseDoorAction()
        {
            if (isRotating || !isOpen) return;

            Debug.Log($"[{gameObject.name}] Closing door...");
            StartCoroutine(RotateDoor(_closedRotationHinge));
        }

        /// <summary>
        /// ECA action to unlock the door.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECADoorV2), "unlock")]
        public void UnlockDoor()
        {
            if (isLocked)
            {
                isLocked = false;
                Debug.Log($"[{gameObject.name}] Door unlocked.");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Door is already unlocked.");
            }
        }

        /// <summary>
        /// ECA action to lock the door.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECADoorV2), "lock")]
        public void LockDoor()
        {
            if (!isLocked)
            {
                isLocked = true;
                Debug.Log($"[{gameObject.name}] Door locked.");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Door is already locked.");
            }
        }

        /// <summary>
        /// Coroutine to smoothly rotate the hinge toward the target rotation.
        /// </summary>
        /// <param name="targetRotationHinge">The target rotation to reach.</param>
        private IEnumerator RotateDoor(Quaternion targetRotationHinge)
        {
            isRotating = true;
            Quaternion startRotationHinge = hinge.localRotation;
            float elapsedTime = 0.0f;

            while (elapsedTime < openSpeed)
            {
                elapsedTime += Time.deltaTime;
                hinge.localRotation = Quaternion.Slerp(startRotationHinge, targetRotationHinge, elapsedTime / openSpeed);
                yield return null;
            }

            hinge.localRotation = targetRotationHinge;
            isOpen = Quaternion.Angle(hinge.localRotation, _openRotationHinge) < 1.0f;
            Debug.Log($"[{gameObject.name}] Rotation finished. Door is " + (isOpen ? "open" : "closed"));
            isRotating = false;
        }

        /// <summary>
        /// Coroutine for visual feedback when trying to open a locked door (door flick effect).
        /// </summary>
        private IEnumerator FlickedDoor()
        {
            if (isRotating) yield break;
            isRotating = true;

            Debug.Log($"[{gameObject.name}] FlickedDoor effect started.");
            Quaternion originalDoorLocalRotation = transform.localRotation;
            float angle = 20f;
            Quaternion targetFlickRotation = originalDoorLocalRotation * Quaternion.Euler(hingeAxis.normalized * angle);
            float elapsedTime = 0f;
            float flickDurationPart = 1f;

            while (elapsedTime < flickDurationPart)
            {
                transform.localRotation = Quaternion.Slerp(originalDoorLocalRotation, targetFlickRotation, elapsedTime / flickDurationPart);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < flickDurationPart)
            {
                transform.localRotation = Quaternion.Slerp(targetFlickRotation, originalDoorLocalRotation, elapsedTime / flickDurationPart);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = originalDoorLocalRotation;
            isRotating = false;
            Debug.Log($"[{gameObject.name}] FlickedDoor effect finished.");
        }

        /// <summary>
        /// Cleanup hinge object when the door is destroyed.
        /// </summary>
        void OnDestroy()
        {
            if (hinge != null)
            {
                if (transform.parent == hinge)
                {
                    transform.SetParent(null, true);
                }

                Destroy(hinge.gameObject);
                Debug.Log($"Pivot per {gameObject.name} distrutto.");
            }
        }
    }
}
