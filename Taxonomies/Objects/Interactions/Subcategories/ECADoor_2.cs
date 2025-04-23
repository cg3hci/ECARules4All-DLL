using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Utils;

namespace ECARules4All_DLL.Taxonomies.Objects.custom
{
    [ECARules4All("door_custom")]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(Interactable))]
    [DisallowMultipleComponent]
    public class ECADoor_Custom : MonoBehaviour
    {
        [Header("Door Settings")]
        public bool isLocked = false;
        public float openAngle = 90f;
        public Vector3 hingeAxis = Vector3.up;
        public float openSpeed = 2.0f;

        [Header("Hinge Calculation")]
        public Vector3 hingeLocalOffset = new Vector3(-0.5f, 0, 0);

        private Transform hinge;
        private Quaternion _closedRotationHinge;
        private Quaternion _openRotationHinge;
        private bool isOpen = false;
        private bool isRotating = false;

        void Awake()
        {
            CreateHingePivot();
            _closedRotationHinge = hinge.localRotation;
            _openRotationHinge = _closedRotationHinge * Quaternion.Euler(hingeAxis.normalized * openAngle);
            isOpen = Quaternion.Angle(hinge.localRotation, _openRotationHinge) < 1.0f;
        }

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

        [ECARelevance(true)]
        [Action(typeof(ECADoor_Custom), "open")]
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

        [ECARelevance(true)]
        [Action(typeof(ECADoor_Custom), "close")]
        public void CloseDoorAction()
        {
             if (isRotating || !isOpen) return;

             Debug.Log($"[{gameObject.name}] Closing door...");
            StartCoroutine(RotateDoor(_closedRotationHinge));
        }

        [ECARelevance(true)]
        [Action(typeof(ECADoor_Custom), "unlock")]
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

        [ECARelevance(true)]
        [Action(typeof(ECADoor_Custom), "lock")]
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

        private IEnumerator FlickedDoor()
        {
             if(isRotating) yield break;
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

         void OnDestroy()
         {
              if (hinge != null)
              {
                   if(transform.parent == hinge)
                   {
                        transform.SetParent(null, true);
                   }
                   Destroy(hinge.gameObject);
                   Debug.Log($"Pivot per {gameObject.name} distrutto.");
              }
         }
    }
}