using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props
{
    /// <summary>
    /// In <b>Prop</b> category we represent generic objects that can be placed in a scene and manipulated by characters.
    /// The possible sub-categories are, in this case, several; we can have passive actions, such as <c>wear</c> in Clothing
    /// script.
    /// </summary>
    [ECARules4All("prop")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Prop : MonoBehaviour
    {
        /// <summary>
        /// <b>Price</b>: The price of the prop object.
        /// </summary>
        [StateVariable("price", ECARules4AllType.Float)]
        public float price
        {
            get => _price;
            set
            {
                _price = value;
                ECAScript.NotifyUpdate(this, nameof(price), price.ToString());
            }
        }
        [SerializeField]
        private float _price;
    }
}