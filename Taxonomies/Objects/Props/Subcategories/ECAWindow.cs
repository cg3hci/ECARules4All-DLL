using System.Collections;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>ECAWindow</b> is a component that represents a virtual window within the environment.
    /// It can be used as part of automation rules involving environmental interactions,
    /// such as opening or closing actions.
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
        
        /// <summary>
        /// <b>isOpen</b> is a boolean state variable that indicates whether a window is open.
        /// Its value is automatically updated when the window executes the <b>opens</b> or <b>closes</b> action.
        /// This variable can be used as a condition within automation rules related to environmental control.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("isOpen", ECARules4AllType.Boolean)]
        public ECABoolean isOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                ECAScript.NotifyUpdate(this, nameof(isOpen), isOpen.ToString());
            }
        }
        [SerializeField] private ECABoolean _isOpen = new ECABoolean(ECABoolean.BoolType.NO);
        
        /// <summary>
        /// <b>opens</b> represents the action of opening a window.
        /// When executed, if the window is currently closed (<c>isOpen = false</c>), it automatically
        /// updates the internal state variable <c>isOpen</c> to <c>true</c> and
        /// rotates the window to its predefined open position.
        /// Executing this action may be involved in automations related to ventilation, lighting,
        /// or other environmental interactions that depend on the window’s open state.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAWindow), "opens")]
        [ContextMenu("opens")]
        public void Opens()
        {
            if (!isOpen)
            {
                isOpen = new ECABoolean(ECABoolean.BoolType.TRUE);
                StopAllCoroutines();
                StartCoroutine(RotateTo(Quaternion.Euler(openRotation)));
            }
        }
        
        /// <summary>
        /// <b>closes</b> represents the action of closing a window.
        /// When executed, if the window is currently open (<c>isOpen = true</c>), it automatically updates the
        /// internal state variable <c>isOpen</c> to <c>false</c> and rotates
        /// the window to its predefined closed position. This action may be involed in automations
        /// related to ventilation, insulation, energy efficiency, or security.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAWindow), "closes")]
        [ContextMenu("closes")]
        public void Closes()
        {
            if (isOpen)
            {
                isOpen = new ECABoolean(ECABoolean.BoolType.FALSE);
                StopAllCoroutines();
                StartCoroutine(RotateTo(Quaternion.Euler(closedRotation)));
            }
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