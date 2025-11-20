using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser
{
    /// <summary>
    /// <b>ECALiquidDispenser</b> is a component that represents a virtual dispenser capable of releasing or
    /// filling containers with specific types of liquids within the environment.
    /// It allows the definition of the liquid type it dispenses, such as water, degreaser, disinfectant, or battery killer,
    /// and integrates with other ECA components to support automated filling and wetting actions.
    /// </summary>
    [ECARules4All("liquidDispenser")]
    [RequireComponent(typeof(ECAProp))]
    [DisallowMultipleComponent]
    public class ECALiquidDispenser : MonoBehaviour
    {
        public ECALiquidSpawner liquidSpawner;

        /// <summary>
        /// <b>liquidType</b> specifies the type of liquid dispensed by this object.
        /// Possible values include 'water', 'degreaser', 'amuchina', and 'battery killer'.
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
