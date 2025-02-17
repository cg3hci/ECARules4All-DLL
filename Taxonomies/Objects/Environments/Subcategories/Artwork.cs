using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    /// <summary>
    /// <b>Artwork</b> represents an artwork in the environment.
    /// The Artwork class defines properties such as the author, price, and creation year of the artwork.
    /// </summary>
    [ECARules4All("artwork")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Artwork : MonoBehaviour
    {
        /// <summary>
        /// <b>author</b> specifies the name of the artist of the artwork.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("author", ECARules4AllType.Text)]
        public string author
        {
            get => _author;
            set
            {
                _author = value;
                ECAScript.NotifyUpdate(this, nameof(author), author);
            }
        }
        [SerializeField]
        private string _author;
        
        /// <summary>
        /// <b>price</b> represents the monetary value of the artwork.
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
        
        /// <summary>
        /// <b>year</b> denotes the year in which the artwork was created.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("year", ECARules4AllType.Integer)]
        public int year
        {
            get => _year;
            set
            {
                _year = value;
                ECAScript.NotifyUpdate(this, nameof(year), year.ToString());
            }
        }
        [SerializeField]
        private int _year;

        // TODO J Ha aggiunto questo
        /// <summary>
        /// <b>type</b> denotes the type of the artwork (e.g., painting, sculpture).
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("type", ECARules4AllType.Text)]
        public string type
        {
            get => _type;
            set
            {
                _type = value;
                ECAScript.NotifyUpdate(this, nameof(type), type.ToString());
            }
        }
        [SerializeField]
        private string _type;
        
        //////
        /// <summary>
        /// <b>description</b> denotes the description of the artwork (e.g., style, technique).
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("description", ECARules4AllType.Text)]
        public string description
        {
            get => _description;
            set
            {
                _description = value;
                ECAScript.NotifyUpdate(this, nameof(description), description.ToString());
            }
        }
        [SerializeField]
        private string _description;
    }
}
