using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    [ECARules4All("artwork")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Artwork : MonoBehaviour
    {
        [StateVariable("author", ECARules4AllType.Text)]
        public string author;
        [StateVariable("price", ECARules4AllType.Float)]
        public float price;
        [StateVariable("year", ECARules4AllType.Integer)]
        public int year;
    }
}
