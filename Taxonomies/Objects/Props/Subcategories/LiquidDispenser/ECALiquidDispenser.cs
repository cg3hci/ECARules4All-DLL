using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser
{
    /// <summary>
    /// <b>ECALiquidDispenser</b> is a virtual dispenser capable of filling containers with specific types of liquid.
    /// Supports defining the type of liquid it dispenses (e.g., water, degreaser, amuchina, battery killer).
    /// </summary>
    [ECARules4All("liquidDispenser")]
    [RequireComponent(typeof(ECAProp))]
    [DisallowMultipleComponent]
    public class ECALiquidDispenser : MonoBehaviour
    {
        public LiquidSpawner liquidSpawner;

        /// <summary>
        /// <b>liquidType</b> specifies the type of liquid dispensed by this object.
        /// Possible values include <c>"water"</c>, <c>"degreaser"</c>, <c>"amuchina"</c>, and <c>"battery killer"</c>.
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
