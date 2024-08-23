using ECARules4All_DLL.Utils;
using ECARules4All_DLL.SmartHomeHubClients;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    [ECARules4All("forniture")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Furniture : ECAScript
    {
        [StateVariable("price", ECARules4AllType.Float)]
        public float price
        {
            get => _price;
            set
            {
                _price = value;
                var attribute = GetStateVariableProperty(nameof(price));
                if(attribute != null)
                {
                    UpdateValueWrapper.UpdateValue(
                        this.ToString(),
                        attribute.Name,
                        _price.ToString()
                    );
                }
            }
        }
        private float _price;
        
        [StateVariable("color", ECARules4AllType.Color)]
        public Color color
        {
            get => _color;
            set
            {
                _color = value;
                var attribute = GetStateVariableProperty(nameof(color));
                if(attribute != null)
                {
                    UpdateValueWrapper.UpdateValue(
                        this.ToString(),
                        attribute.Name,
                        _color.ToString()
                    );
                }
            }
        }
        private Color _color;
        
        [StateVariable("dimension", ECARules4AllType.Float)]
        public float dimension
        {
            get => _dimension;
            set
            {
                _dimension = value;
                var attribute = GetStateVariableProperty(nameof(dimension));
                if(attribute != null)
                {
                    UpdateValueWrapper.UpdateValue(
                        this.ToString(),
                        attribute.Name,
                        _dimension.ToString()
                    );
                }
            }
        }
        private float _dimension;

    }
}
