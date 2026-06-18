using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    /// <summary>
    /// Represents an interactive tankard in the taverna scene, managing its containment state and handling its 2D visual representation.
    /// </summary>
    [ECARules4All("tankard")]
    [RequireComponent(typeof(ECAInteraction))]
    [DisallowMultipleComponent]
    public class ECATankard : ECAObject
    {
        [Header("Tankard Properties (HASS)")]
        
        /// <summary>
        /// A boolean state variable that determines if the tankard is completely empty or contains liquid.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("isEmpty", ECARules4AllType.Boolean)]
        public ECABoolean isEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                ECAScript.NotifyUpdate(this, nameof(isEmpty), isEmpty.ToString());
                UpdateVisualEffects();
            }
        }
        [SerializeField]
        private ECABoolean _isEmpty = new ECABoolean(ECABoolean.BoolType.ON);

        /// <summary>
        /// A boolean state variable that specifies if the tankard is currently filled with beer.
        /// </summary>
        [StateVariable("hasBeer", ECARules4AllType.Boolean)]
        public ECABoolean hasBeer
        {
            get => _hasBeer;
            set
            {
                _hasBeer = value;
                ECAScript.NotifyUpdate(this, nameof(hasBeer), hasBeer.ToString());
                UpdateVisualEffects();
            }
        }
        [SerializeField]
        private ECABoolean _hasBeer = new ECABoolean(ECABoolean.BoolType.OFF);

        /// <summary>
        /// A boolean state variable that tracks whether the player is currently holding the tankard in their hands.
        /// </summary>
        [StateVariable("isHeld", ECARules4AllType.Boolean)]
        public ECABoolean isHeld
        {
            get => _isHeld;
            set
            {
                _isHeld = value;
                ECAScript.NotifyUpdate(this, nameof(isHeld), isHeld.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _isHeld = new ECABoolean(ECABoolean.BoolType.OFF);

        private SpriteRenderer beerSprite;

        /// <summary>
        /// Initializes the object, automatically finds the child SpriteRenderer, and sets the initial visual state.
        /// </summary>
        protected void Awake()
        {
            // Recupera in automatico lo SpriteRenderer del cerchio giallo dai figli del boccale
            beerSprite = GetComponentInChildren<SpriteRenderer>();
            
            UpdateVisualEffects();
        }

        /// <summary>
        /// Triggers the action to fill the tankard with beer, updating both its empty and beer containment states.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECATankard), "fill")]
        [ContextMenu("Fill")]
        public void Fill()
        {
            isEmpty = new ECABoolean(ECABoolean.BoolType.OFF);
            hasBeer = new ECABoolean(ECABoolean.BoolType.ON);
        }

        /// <summary>
        /// Triggers the action to pour out all contents, resetting the tankard back to an empty and beerless state.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECATankard), "empty")]
        [ContextMenu("Empty")]
        public void Empty()
        {
            isEmpty = new ECABoolean(ECABoolean.BoolType.ON);
            hasBeer = new ECABoolean(ECABoolean.BoolType.OFF);
        }

        /// <summary>
        /// Triggers the action to place the tankard back down, marking the item as no longer held by the player.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECATankard), "placeOnTable")]
        [ContextMenu("Place On Table")]
        public void PlaceOnTable()
        {
            isHeld = new ECABoolean(ECABoolean.BoolType.OFF);
        }

        /// <summary>
        /// Toggles the visibility of the 2D beer sprite based on the current empty and fluid state variables.
        /// </summary>
        private void UpdateVisualEffects()
        {
            if (beerSprite == null) return;

            bool isTankardEmpty = isEmpty.ToString() == "ON" || isEmpty.ToString() == "True";
            bool isTankardFilledWithBeer = hasBeer.ToString() == "ON" || hasBeer.ToString() == "True";

            if (!isTankardEmpty && isTankardFilledWithBeer)
            {
                if (!beerSprite.enabled) beerSprite.enabled = true;
            }
            else
            {
                if (beerSprite.enabled) beerSprite.enabled = false;
            }
        }
    }
}