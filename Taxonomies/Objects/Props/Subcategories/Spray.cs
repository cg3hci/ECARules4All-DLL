using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>Spray</b> is a class that represents a spray.
    /// </summary>
    [ECARules4All("spray")]
    [RequireComponent(typeof(ECAProp))]
    [DisallowMultipleComponent]
    public class Spray : MonoBehaviour
    {
        /// <summary>
        /// <b>SprayType</b>: This enum is used to define the spray types: SMALL, MEDIUM and LARGE.
        /// </summary>
        public enum SprayType
        {
            SMALL,
            MEDIUM,
            LARGE
        }

        public SprayType type;

        /// <summary>
        /// <b>Charge</b>: a float value that represents the charge of the spray.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("charge", ECARules4AllType.Float)] 
        public float charge
        {
            get => _charge;
            set
            {
                _charge = value;
                ECAScript.NotifyUpdate(this, nameof(charge), charge.ToString());
            }
        }
        [SerializeField]
        private float _charge = 100;
        
        /// <summary>
        /// <b> ChangesSprayType</b> changes the spray type.
        /// </summary>
        /// <param name="type">The new <see cref="SprayType"/> value. </param>
        [ECARelevance(true)]
        [Action(typeof(Spray), "changes", "SprayType", "to", typeof(SprayType))]
        public void ChangesSprayType(SprayType type)
        {
            this.type = type;
        }
        
        /// <summary>
        /// <b>Sprays</b>: The action of using the spray. It decreases the charge of the spray.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(Spray), "sprays")]
        public void Sprays()
        {
            if (charge > 0)
            {
                float amount = 0;

                switch (type)
                {
                    case SprayType.SMALL:
                        amount = charge >= 1 ? 1 : charge;
                        //...
                        break;
                
                    case SprayType.MEDIUM:
                        amount = charge >= 3 ? 3 : charge;
                        //...
                        break;

                    case SprayType.LARGE:
                        amount = charge >= 5 ? 5 : charge;
                        //...
                        break; 
                }
                
                charge -= amount;
            }
        }
    }
}