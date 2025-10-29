using System.Collections;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>ECAWindow</b>: This class is used to define a window object.
    /// </summary>
    [ECARules4All("window")]
    [RequireComponent(typeof(ECAProp))]
    [RequireComponent(typeof(ECAInteractable))]
    [DisallowMultipleComponent]
    public class ECAWindow : MonoBehaviour
    {
        public GameObject _objectToRotate;
        [SerializeField] private Vector3 closedRotation = Vector3.zero;
        [SerializeField] private Vector3 openRotation = new Vector3(0, 0, -90);
        [SerializeField] private float duration = 1.0f;
        
        private void OnEnable()
        {
        }

        [ECARelevance(true)]
        [Action(typeof(ECAWindow), "opens")]
        public void Opens()
        {
            StopAllCoroutines();
            StartCoroutine(RotateTo(Quaternion.Euler(openRotation)));
        }

        [ECARelevance(true)]
        [Action(typeof(ECAWindow), "closes")]
        public void Closes()
        {
            StopAllCoroutines();
            StartCoroutine(RotateTo(Quaternion.Euler(closedRotation)));
        }

        private IEnumerator RotateTo(Quaternion targetRotation)
        {
            Quaternion startRotation = _objectToRotate.transform.localRotation;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _objectToRotate.transform.localRotation =
                    Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
                yield return null;
            }

            _objectToRotate.transform.localRotation = targetRotation;
        }
    }
}