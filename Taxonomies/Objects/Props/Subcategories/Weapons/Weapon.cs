using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.Weapons
{
    /// <summary>
    /// The <b>Weapon</b> class is a base class for all weapons.
    /// </summary>
    [ECARules4All("weapon")]
    [RequireComponent(typeof(ECAProp))] 
    [DisallowMultipleComponent]
    public class Weapon: MonoBehaviour
    {
        /// <summary>
        /// <b>Power</b>: a float value that represents the power of the weapon.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("power", ECARules4AllType.Float)] 
        public float power
        {
            get => _power;
            set
            {
                _power = value;
                ECAScript.NotifyUpdate(this, nameof(power), power.ToString());
            }
        }
        [SerializeField]
        private float _power;
    }
}
