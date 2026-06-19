using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidContainer;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    /// <summary>
    /// Represents an interactive tankard in the taverna scene, specializing ECALiquidContainer to manage fluid states for the rule engine.
    /// </summary>
    [ECARules4All("tankard")]
    [DisallowMultipleComponent]
    public class ECATankard : ECALiquidContainer
    {
        /// <summary>
        /// A boolean state variable that determines if the tankard is completely empty or contains liquid.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("isEmpty", ECARules4AllType.Boolean)]
        public ECABoolean isEmpty
        {
            get => _isEmpty;
            set => _isEmpty = value;
        }
        private ECABoolean _isEmpty = new ECABoolean(ECABoolean.TRUE);

        /// <summary>
        /// A boolean state variable that specifies if the tankard is currently filled with beer.
        /// </summary>
        [StateVariable("hasBeer", ECARules4AllType.Boolean)]
        public ECABoolean hasBeer
        {
            get => _hasBeer;
            set => _hasBeer = value;
        }
        private ECABoolean _hasBeer = new ECABoolean(ECABoolean.FALSE);

        /// <summary>
        /// A boolean state variable that tracks whether the player is currently holding the tankard in their hands.
        /// </summary>
        [StateVariable("isHeld", ECARules4AllType.Boolean)]
        public ECABoolean isHeld
        {
            get => _isHeld;
            set => _isHeld = value;
        }
        private ECABoolean _isHeld = new ECABoolean(ECABoolean.FALSE);

        private SpriteRenderer beerSprite;

        /// <summary>
        /// Initializes the object, automatically finds the child SpriteRenderer, and sets the initial visual state.
        /// </summary>
        protected void Awake()
        {
            beerSprite = GetComponentInChildren<SpriteRenderer>(true);
            UpdateVisualEffects();
        }

        /// <summary>
        /// Synchronizes and updates the state variables and visual representation based on the physical drops inside the container.
        /// </summary>
        private void Update()
        {
            int totalDrops = waterDrops + degreaserDrops + batteryKillerDrops + amuchinaDrops + beerDrops;

            bool currentEmpty = totalDrops == 0;
            bool currentHasBeer = beerDrops > 0;
            bool stateChanged = false;

            if (currentEmpty != (isEmpty == ECABoolean.TRUE))
            {
                isEmpty = new ECABoolean(currentEmpty ? ECABoolean.TRUE : ECABoolean.FALSE);
                ECAScript.NotifyUpdate(this, nameof(isEmpty), isEmpty.ToString());
                stateChanged = true;
            }

            if (currentHasBeer != (hasBeer == ECABoolean.TRUE))
            {
                hasBeer = new ECABoolean(currentHasBeer ? ECABoolean.TRUE : ECABoolean.FALSE);
                ECAScript.NotifyUpdate(this, nameof(hasBeer), hasBeer.ToString());
                stateChanged = true;
            }

            // [AGGIORNAMENTO]: Se lo stato è cambiato, aggiorna visibilità E posizione del cerchio della birra
            if (stateChanged)
            {
                UpdateVisualEffects();
            }

            // Se il boccale contiene liquido, aggiorna costantemente la posizione in altezza del cerchio
            if (totalDrops > 0)
            {
                UpdateLiquidPosition(totalDrops);
            }
        }

        /// <summary>
        /// Calcola la quota 3D del cerchio di liquido interpolando tra StartStep ed EndStep in base alle gocce.
        /// </summary>
        private void UpdateLiquidPosition(int totalDrops)
        {
            if (startStep == null || endStep == null || waterCircleInstance == null) return;

            // Formula del laboratorio: quanti step discreti abbiamo riempito?
            int currentLevel = Mathf.FloorToInt(totalDrops / liquidPerLevel);
            
            // Normalizziamo il valore tra 0.0 e 1.0 rispetto al numero massimo di step configurati
            float t = Mathf.Clamp01((float)currentLevel / (numberOfFillSteps - 1));

            // Muove fisicamente l'istanza del cerchio sulla quota corretta
            waterCircleInstance.transform.position = Vector3.Lerp(
                startStep.levelTransform.position, 
                endStep.levelTransform.position, 
                t
            );
        }

        /// <summary>
        /// Triggers the action to fill the tankard with beer, computing the drops required to reach the maximum fill step level.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECATankard), "fill")]
        [ContextMenu("Fill")]
        public void Fill()
        {
            beerDrops = Mathf.CeilToInt(numberOfFillSteps * liquidPerLevel);
        }

        /// <summary>
        /// Triggers the action to pour out all contents, resetting all fluid types and drop counters to zero.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECATankard), "empty")]
        [ContextMenu("Empty")]
        public void Empty()
        {
            waterDrops = 0;
            degreaserDrops = 0;
            batteryKillerDrops = 0;
            amuchinaDrops = 0;
            beerDrops = 0;
        }

        /// <summary>
        /// Triggers the action to place the tankard back down, marking the item as no longer held by the player.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECATankard), "placeOnTable")]
        [ContextMenu("Place On Table")]
        public void PlaceOnTable()
        {
            isHeld = new ECABoolean(ECABoolean.FALSE);
            ECAScript.NotifyUpdate(this, nameof(isHeld), isHeld.ToString());
        }

        /// <summary>
        /// Toggles the visibility of the 2D fluid sprite based on the current empty and liquid containment state variables.
        /// </summary>
        private void UpdateVisualEffects()
        {
            if (beerSprite == null)
            {
                beerSprite = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (beerSprite == null) return;

            if (isEmpty == ECABoolean.FALSE && hasBeer == ECABoolean.TRUE)
            {
                if (!beerSprite.enabled) beerSprite.enabled = true;
                
                // Forza l'attivazione del GameObject se la classe madre lo ha spento nello Start()
                if (waterCircleInstance != null && !waterCircleInstance.activeSelf) 
                {
                    waterCircleInstance.SetActive(true);
                }
            }
            else
            {
                if (beerSprite.enabled) beerSprite.enabled = false;
                
                if (waterCircleInstance != null && waterCircleInstance.activeSelf) 
                {
                    waterCircleInstance.SetActive(false);
                }
            }
        }
    }
}