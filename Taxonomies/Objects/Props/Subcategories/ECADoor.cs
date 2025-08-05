using System.Collections;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>ECADoor</b>: This class is used to define a door beviour.
    /// </summary>
    [ECARules4All("door")]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(Interactable))]
    [DisallowMultipleComponent]
    public class ECADoor : MonoBehaviour
    {
        [SerializeField] private bool status = false;
        [SerializeField] private Vector3 _originalRotation;
        
        public Vector3 openRotation = new Vector3(0, 90, 0);
        public bool openOnTrigger = false;
        private GameObject _objectToRotate;

        private void OnEnable()
        {
            // search the child with the name "PlanePanel". If found assign it to _objectToRotate. Otherwise assign the current gameObject.
            _objectToRotate = transform.Find("PlanePanel")?.gameObject ?? gameObject;
            _originalRotation = _objectToRotate.transform.localRotation.eulerAngles;
            // if(_objectToRotate.GetComponent<Collider>() == null)
            //     _objectToRotate.AddComponent<BoxCollider>().isTrigger=true;
        }

        [ECARelevance(true)]
        [Action(typeof(ECADoor), "opens")]
        public void Opens()
        {
            status = true;
            StopAllCoroutines(); // Stop any ongoing rotation
            StartCoroutine(RotateTo(Quaternion.Euler(openRotation)));
            // _objectToRotate.transform.localRotation = Quaternion.Euler(openRotation);
        }

        [ECARelevance(true)]
        [Action(typeof(ECADoor), "closes")]
        public void Closes()
        {
            status = false;
            StopAllCoroutines(); // Stop any ongoing rotation
            StartCoroutine(RotateTo(Quaternion.Euler(_originalRotation)));
            // _objectToRotate.transform.localRotation = Quaternion.Euler(_originalRotation);
        }

        private IEnumerator RotateTo(Quaternion targetRotation)
        {
            Quaternion startRotation = _objectToRotate.transform.localRotation;
            float elapsedTime = 0f;
            float duration = 1f; // Adjust the duration for the lerp

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _objectToRotate.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
                yield return null;
            }

            // Ensure the final rotation matches exactly
            _objectToRotate.transform.localRotation = targetRotation;
        }
        
        public void SwitchState()
        {
            Debug.LogWarning("[ECADOOR] SWITCH STATE");
            if (status)
            {
                Closes();
                EventBus.GetInstance().Publish(new Action(this.gameObject, "closes"));
            }
            else
            {
                Opens();
                EventBus.GetInstance().Publish(new Action(this.gameObject, "opens"));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogWarning("[ECADOOR] ON TRIGGER ENTER " + other.gameObject.name);
            if (!openOnTrigger) return;
            if (other.GetComponent<Character>() == null) return;
            Debug.LogWarning("[ECADOOR] CHARACTER TROVATO");

            if (!status)
            {
                Opens();
                EventBus.GetInstance().Publish(new Action(this.gameObject, "opens"));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.LogWarning("[ECADOOR] ON TRIGGER EXIT " + other.gameObject.name);
            if (!openOnTrigger) return;

            if (other.GetComponent<Character>() == null) return;
            Debug.LogWarning("[ECADOOR] CHARACTER TROVATO");
            if (status)
            {
                Closes();
                EventBus.GetInstance().Publish(new Action(this.gameObject, "closes"));
            }
        }
    }
}