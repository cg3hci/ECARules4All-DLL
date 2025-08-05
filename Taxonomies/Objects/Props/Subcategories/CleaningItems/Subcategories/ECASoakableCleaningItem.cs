using System;
using System.Collections;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECASoakableCleaningItem</b> is a virtual object that represents a reusable cleaning item
    /// capable of absorbing and releasing different types of liquids (e.g., water, degreaser, battery killer, disinfectant).
    /// It supports being wetted and dried, and tracks its current state using dedicated ECA boolean variables.
    /// </summary>
    [ECARules4All("soakableCleaningItem")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECASoakableCleaningItem : MonoBehaviour
    {
        public Material dryMaterial;
        public Material wetMaterial;
        public Renderer _renderer;

        void Awake()
        {
            // Initializes the component by checking that required materials are assigned.
            if (dryMaterial == null) throw new Exception("Material Dry not assigned");
            if (wetMaterial == null) throw new Exception("Material Wet not assigned");
        }

        private void OnCollisionEnter(Collision other)
        {
            // If the object is a LiquidDrop, it is considered an attempt to wet the item.
            var ldrop = other.gameObject.GetComponent<LiquidDrop>();
            if (ldrop != null)
            {
                ECALiquidDispenser ld = ldrop.owner;
                _Wets(ld);
                Action action = new Action(ld.gameObject, "wets", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // If it belongs to a LiquidDrop, it initiates the wetting process.
            var ldrop = other.gameObject.GetComponent<LiquidDrop>();
            if (ldrop != null)
            {
                ECALiquidDispenser ld = ldrop.owner;
                _Wets(ld);
                Action action = new Action(ld.gameObject, "wets", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }


        /// <summary>
        /// <b>hasWater</b> indicates whether the item currently contains water.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("hasWater", ECARules4AllType.Boolean)]
        public ECABoolean hasWater
        {
            get => _hasWater;
            set
            {
                _hasWater = value;
                ECAScript.NotifyUpdate(this, nameof(hasWater), hasWater);
            }
        }

        [SerializeField] private ECABoolean _hasWater = new ECABoolean(ECABoolean.BoolType.NO);


        /// <summary>
        /// <b>hasDegreaser</b> indicates whether the item currently contains degreaser.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("hasDegreaser", ECARules4AllType.Boolean)]
        public ECABoolean hasDegreaser
        {
            get => _hasDegreaser;
            set
            {
                _hasDegreaser = value;
                ECAScript.NotifyUpdate(this, nameof(hasDegreaser), hasDegreaser);
            }
        }

        [SerializeField] private ECABoolean _hasDegreaser = new ECABoolean(ECABoolean.BoolType.NO);


        /// <summary>
        /// <b>hasBatteryKiller</b> indicates whether the item currently contains battery killer solution.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("hasBatteryKiller", ECARules4AllType.Boolean)]
        public ECABoolean hasBatteryKiller
        {
            get => _hasBatteryKiller;
            set
            {
                _hasBatteryKiller = value;
                ECAScript.NotifyUpdate(this, nameof(hasBatteryKiller), hasBatteryKiller);
            }
        }

        [SerializeField] private ECABoolean _hasBatteryKiller = new ECABoolean(ECABoolean.BoolType.NO);


        /// <summary>
        /// <b>hasAmuchina</b> indicates whether the item currently contains Amuchina (a disinfectant).
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("hasAmuchina", ECARules4AllType.Boolean)]
        public ECABoolean hasAmuchina
        {
            get => _hasAmuchina;
            set
            {
                _hasAmuchina = value;
                ECAScript.NotifyUpdate(this, nameof(hasAmuchina), hasAmuchina);
            }
        }

        [SerializeField] private ECABoolean _hasAmuchina = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b>Wets</b> is an action method that updates the item’s internal state to reflect it has absorbed a specific liquid.
        /// It changes the item's material to a "wet" visual and starts a timer for automatic drying.
        /// </summary>
        /// <param name="ld">The liquid dispenser responsible for wetting this item.</param>
        [Action(typeof(ECALiquidDispenser), "wets", typeof(ECASoakableCleaningItem))]
        [ECARelevance(true)]
        public void _Wets(ECALiquidDispenser ld)
        {
            //////// Update liquid type ////////
            var lqType = ld.liquidSpawner.GetLiquidType();
            if (lqType == LiquidSpawner.LiquidType.Water)
            {
                hasWater = ECABoolean.YES;
                //TODO Cambiare stile per Water
            }
            else if (lqType == LiquidSpawner.LiquidType.Degreaser)
            {
                hasDegreaser = ECABoolean.YES;
                //TODO Cambiare stile per Degreaser
            }
            else if (lqType == LiquidSpawner.LiquidType.BatteryKiller)
            {
                hasBatteryKiller = ECABoolean.YES;
                //TODO Cambiare stile per Battery Killer
            }
            else if (lqType == LiquidSpawner.LiquidType.Amuchina)
            {
                hasAmuchina = ECABoolean.YES; //TODO Cambiare stile per Amuchina
            }

            _renderer.material = wetMaterial;
            StopAllCoroutines();
            StartCoroutine(CoroutineDries(30f)); //TODO Pensare se vogliamo rimetterlo
        }

        // Waits for a specified delay before drying the item automatically.
        IEnumerator CoroutineDries(float delay)
        {
            yield return new WaitForSeconds(delay);
            Dries();
            var action = new Action(this.gameObject, "dries");
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        }

        /// <summary>
        /// <b>Dries</b> is an action method that resets the item to a dry state,
        /// both visually and logically by clearing the water state variable.
        /// </summary>
        [Action(typeof(ECASoakableCleaningItem), "dries")]
        [ECARelevance(true)]
        public void Dries()
        {
            _renderer.material = dryMaterial;
            hasWater = ECABoolean.NO;
        }
    }
}