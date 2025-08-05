using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser
{
    /// <summary>
    /// TODO ADD
    /// </summary>
    [ECARules4All("liquidDispenser")]
    [RequireComponent(typeof(Prop))]
    [DisallowMultipleComponent]
    public class ECALiquidDispenser : MonoBehaviour
    {
        public LiquidSpawner liquidSpawner;

        /// <summary>
        /// <b>liquidType</b> specifies 
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("liquidType", ECARules4AllType.Text)]
        public string liquidType
        {
            get => _liquidType;
            set
            {
                _liquidType = value;
                ECAScript.NotifyUpdate(this, nameof(liquidType), liquidType);
            }
        }

        [SerializeField] private string _liquidType = "water";

        private void Start()
        {
            liquidSpawner.liquidDispenser = this;
        }
    }
}
