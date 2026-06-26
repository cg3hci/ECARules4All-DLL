using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    /// <summary>
    /// ECAKey represents a physical key used in the VR environment. 
    /// It mainly tracks whether it is currently being held by the user.
    /// </summary>
    [ECARules4All("key")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECAInteractable))]
    public class ECAKey : ECAProp
    {
        [SerializeField]
        private ECABoolean _isPickedUp = ECABoolean.FALSE;

        /// <summary>
        /// Gets or sets the state of the key. True if the user is holding it, false otherwise.
        /// Automatically notifies the ECA backend upon state change.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("isPickedUp", ECARules4AllType.Boolean)] 
        public ECABoolean isPickedUp
        {
            get => _isPickedUp;
            set
            {
                _isPickedUp = value;
                ECAScript.NotifyUpdate(this, nameof(isPickedUp), _isPickedUp.ToString());
            }
        }

        /// <summary>
        /// Updates the key state to indicate it has been picked up by the user.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAKey), "PickUp")]
        public void PickUp()
        {
            isPickedUp = ECABoolean.TRUE;
        }

        /// <summary>
        /// Updates the key state to indicate it has been released or dropped by the user.
        /// This is an auxiliary method to keep the physics interaction synchronized with the ECA state.
        /// </summary>
        public void Drop()
        {
            isPickedUp = ECABoolean.FALSE;
        }
    }
}