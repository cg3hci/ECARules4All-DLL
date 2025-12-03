using System;
using System.Collections;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECASoakableCleaningItem</b> is a component that represents a reusable virtual cleaning item
    /// capable of absorbing and releasing various types of liquids, such as water, degreaser, battery killer,
    /// or disinfectant.
    /// It can be wetted or dried and maintains its current state through dedicated ECA boolean variables,
    /// which indicate the presence or absence of specific absorbed substances.
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
            Log.Information("[ECASoakableCleaningItem] on collision enter");
            
            // If the object is a LiquidDrop, it is considered an attempt to wet the item.
            //var ldrop = other.gameObject.GetComponent<ECALiquidDrop>();
            var ldrop = other.gameObject.GetComponent<ECALiquidDispenser>();
            if (ldrop != null)
            {
                //ECALiquidDispenser ld = ldrop.owner;
                ECALiquidDispenser ld = ldrop;
                _Wets(ld);
                Action action = new Action(ld.gameObject, "wets", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
                Log.Information("[ECASoakableCleaningItem] on collision enter - publish wets action");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Log.Information("[ECASoakableCleaningItem] on trigger enter");
            
            // If it belongs to a LiquidDrop, it initiates the wetting process.
            //var ldrop = other.gameObject.GetComponent<ECALiquidDrop>();
            var ldrop = other.gameObject.GetComponent<ECALiquidDispenser>();
            if (ldrop != null)
            {
                //ECALiquidDispenser ld = ldrop.owner;
                ECALiquidDispenser ld = ldrop;
                _Wets(ld);
                Action action = new Action(ld.gameObject, "wets", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
                Log.Information("[ECASoakableCleaningItem] on trigger enter - publish wets action");
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
        /// <b>wets</b> represents the action in which an object equipped with the <see cref="ECASoakableCleaningItem"/>
        /// component, such as a cloth, rag, or paper towel, becomes wet after being poured on by an object equipped with
        /// the <see cref="ECALiquidDispenser"/> component, such as a bottle or sprayer within the environment.
        /// When the <see cref="ECASoakableCleaningItem"/> is wetted, this event acts as a trigger within an ECA automation.
        /// The resulting state change depends on the liquid dispensed:
        /// - When the <see cref="ECALiquidDispenser"/> contains water, the cleaning item implicitly performs the action
        /// <b>changes hasWater</b>.
        /// - When the <see cref="ECALiquidDispenser"/> contains degreaser, it implicitly performs the action <b>changes hasDegreaser</b>.
        /// Executing this action updates the internal state of the cleaning item to reflect the absorbed liquid.
        /// </summary>
        [Action(typeof(ECALiquidDispenser), "wets", typeof(ECASoakableCleaningItem))]
        [ECARelevance(true)]
        public void _Wets(ECALiquidDispenser ld)
        {
            //////// Update liquid type ////////
            var lqType = ld.liquidSpawner.GetLiquidType();
            if (lqType == ECALiquidSpawner.LiquidType.Water)
            {
                hasWater = ECABoolean.YES;
                //TODO Cambiare stile per Water
            }
            else if (lqType == ECALiquidSpawner.LiquidType.Degreaser)
            {
                hasDegreaser = ECABoolean.YES;
                //TODO Cambiare stile per Degreaser
            }
            else if (lqType == ECALiquidSpawner.LiquidType.BatteryKiller)
            {
                hasBatteryKiller = ECABoolean.YES;
                //TODO Cambiare stile per Battery Killer
            }
            else if (lqType == ECALiquidSpawner.LiquidType.Amuchina)
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
        
        /// <summary>
        /// <bchanges has water</b> represents the implicit action performed when an object equipped with an
        /// <see cref="ECASoakableCleaningItem"/> component becomes wet due to the <b>wets</b> action
        /// triggered by an object equipped with a <see cref="ECALiquidDispenser"/> component containing <b>water</b>.
        /// This action updates the internal state of the cleaning item by setting the <b>hasWater</b> variable to <c>true</c>,
        /// indicating that the object has absorbed water and is now ready for water-based cleaning operations.
        /// </summary>
        [Action(typeof(ECASoakableCleaningItem), "changes has water")]
        [ECARelevance(true)]
        public void changesHasWater(){
            this.hasWater = new ECABoolean(true);
        }
        
        /// <summary>
        /// <b>changes-has-degreaser</b> represents the implicit action performed when an object equipped with an
        /// <see cref="ECASoakableCleaningItem"/> component becomes wet due to the <b>wets</b> action
        /// triggered by an object equipped with a <see cref="ECALiquidDispenser"/> component containing <b>degreaser</b>.
        /// This action updates the internal state of the cleaning item by setting the <b>hasDegreaser</b> variable to <c>true</c>,
        /// indicating that the object has absorbed degreaser and is now ready for degreasing operations.
        /// </summary>
        [Action(typeof(ECASoakableCleaningItem), "changes has degreaser")]
        [ECARelevance(true)]
        public void changesHasDegreaser(){
            this.hasDegreaser = new ECABoolean(true);
        }
    }
}