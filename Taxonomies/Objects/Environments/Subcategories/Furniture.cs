using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    [ECARules4All("forniture")]
    [RequireComponent(typeof(ECAEnvironment))]
    [DisallowMultipleComponent]
    public class Furniture : MonoBehaviour
    {
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
        [SerializeField] private float _price;
        
        [ECARelevance(true)]
        [StateVariable("color", ECARules4AllType.Color)]
        public Color color
        {
            get => _color;
            set
            {
                _color = value;
                ECAScript.NotifyUpdate(this, nameof(color), color.ToString());
            }
        }

        [SerializeField] private Color _color;

        [StateVariable("dimension", ECARules4AllType.Float)]
        public float dimension
        {
            get => _dimension;
            set
            {
                _dimension = value;
                ECAScript.NotifyUpdate(this, nameof(dimension), dimension.ToString());
            }
        }

        [SerializeField] private float _dimension;
    }
}
