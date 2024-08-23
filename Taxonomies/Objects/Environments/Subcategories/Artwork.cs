using ECARules4All_DLL.Utils;
using ECARules4All_DLL.SmartHomeHubClients;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    [ECARules4All("artwork")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Artwork : ECAScript
    {
        [StateVariable("author", ECARules4AllType.Text)]
        public string author
        {
            get => _author;
            set
            {
                _author = value;
                var attribute = GetStateVariableProperty(nameof(author));
                if(attribute != null)
                {
                    UpdateValueWrapper.UpdateValue(
                        this.ToString(),
                        attribute.Name,
                        _author.ToString()
                    );
                }
            }
        }
        private string _author;
        
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
        
        [StateVariable("year", ECARules4AllType.Integer)]
        public int year
        {
            get => _year;
            set
            {
                _year = value;
                var attribute = GetStateVariableProperty(nameof(year));
                if(attribute != null)
                {
                    UpdateValueWrapper.UpdateValue(
                        this.ToString(),
                        attribute.Name,
                        _year.ToString()
                    );
                }
            }
        }
        private int _year;
    }
}
