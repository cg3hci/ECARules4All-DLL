using System.Collections;
using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    /// <summary>
    /// <b>ECABeerBarrel</b> represents a rustic or medieval beer barrel in the Virtual Tavern.
    /// It extends <see cref="ECALiquidDispenser"/> to manage beer dispensing, tracking the tap state
    /// and the volume of remaining liquid, using the native physics-based spawning system (<see cref="ECALiquidSpawner"/>).
    /// </summary>
    [ECARules4All("beerBarrel")]
    [RequireComponent(typeof(ECAInteraction))]
    [DisallowMultipleComponent]
    public class ECABeerBarrel : ECALiquidDispenser
    {
        [Header("Beer Barrel Properties (HASS)")]
        [SerializeField]
        private ECABoolean _isTapped = new ECABoolean(ECABoolean.BoolType.FALSE);

        [SerializeField]
        private float _remainingLiquid = 100f; // Capacità iniziale (100 litri o 100%)

        /// <summary>
        /// <b>isTapped</b> specifies whether the barrel's tap is currently open and dispensing beer drops.
        /// It uses the strict <see cref="ECABoolean"/> type to maintain compliance with the Rule Engine.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("isTapped", ECARules4AllType.Boolean)]
        public ECABoolean isTapped
        {
            get => _isTapped;
            set
            {
                _isTapped = value;
                ECAScript.NotifyUpdate(this, nameof(isTapped), isTapped.ToString());
            }
        }

        /// <summary>
        /// <b>remainingLiquid</b> tracks the volume or percentage of beer left inside the barrel.
        /// Changes to this value are synchronized with Home Assistant using invariant culture formatting.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("remainingLiquid", ECARules4AllType.Float)]
        public float remainingLiquid
        {
            get => _remainingLiquid;
            set
            {
                _remainingLiquid = value;
                ECAScript.NotifyUpdate(this, nameof(remainingLiquid), remainingLiquid.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Initializes the barrel by forcing the inherited liquidType property to "beer" 
        /// and linking the owner reference.
        /// </summary>
        protected void Awake()
        {
            liquidType = "beer";
        }

        /// <summary>
        /// <b>OpenTap</b> opens the valve of the barrel to initiate the beer pouring process.
        /// It starts the native physics spawner (<see cref="ECALiquidSpawner"/>) at Ambient temperature.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABeerBarrel), "open tap")]
        [ContextMenu("Open Tap")]
        public void OpenTap()
        {
            if (remainingLiquid > 0f)
            {
                isTapped = new ECABoolean(ECABoolean.BoolType.TRUE);
                Debug.Log($"[{gameObject.name}] OpenTap() - Avvio lo spawner fisico delle gocce di birra.");
                
                // Attiviamo lo spawner nativo ereditato dalla superclasse
                if (liquidSpawner != null)
                {
                    liquidSpawner.SpawnLiquid(ECALiquidSpawner.LiquidTemperature.Ambient);
                }
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Il barile è vuoto! Impossibile spillare birra.");
                CloseTap();
            }
        }

        /// <summary>
        /// <b>CloseTap</b> closes the valve of the barrel, immediately halting the liquid drop spawning.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABeerBarrel), "close tap")]
        [ContextMenu("Close Tap")]
        public void CloseTap()
        {
            isTapped = new ECABoolean(ECABoolean.BoolType.FALSE);
            Debug.Log($"[{gameObject.name}] CloseTap() - Rubinetto chiuso, blocco lo spawner nativo.");

            // Fermiamo lo spawner nativo ereditato
            if (liquidSpawner != null)
            {
                liquidSpawner.StopSpawningLiquid();
            }
        }

        /// <summary>
        /// <b>PourBeer</b> opens the tap to dispense beer and automatically closes it after 3 seconds.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABeerBarrel), "pour beer")]
        [ContextMenu("Open Tap Timed (3s)")]
        public void PourBeer()
        {
            StartCoroutine(PourBeerCourtine());
        }

        /// <summary>
        /// Coroutine interna che gestisce l'attesa di 3 secondi tra l'apertura e la chiusura della botte di birra.
        /// </summary>        
        private IEnumerator PourBeerCourtine()
        {
            OpenTap();
            
            yield return new WaitForSeconds(3f);
            
            CloseTap();
        }
    }
}