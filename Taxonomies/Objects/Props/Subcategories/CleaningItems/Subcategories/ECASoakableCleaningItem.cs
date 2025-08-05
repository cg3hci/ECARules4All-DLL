using System;
using System.Collections;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// TODO ADD
    /// </summary>
    [ECARules4All("soakableCleaningItem")]
    [RequireComponent(typeof(CleaningItem))]
    [DisallowMultipleComponent]
    public class ECASoakableCleaningItem : MonoBehaviour
    {
        public Material dryMaterial;
        public Material wetMaterial;
        public Renderer _renderer;

        void Awake()
        {
            if (dryMaterial == null) throw new Exception("Material Dry not assigned");
            if (wetMaterial == null) throw new Exception("Material Wet not assigned");
        }

        private void OnCollisionEnter(Collision other)
        {
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
        /// <b>variableName</b> blabla 
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
        /// <b>variableName</b> blabla 
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
        /// <b>variableName</b> blabla 
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
        /// <b>variableName</b> blabla 
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
        /// TODO
        /// </summary>
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
        /// TODO
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