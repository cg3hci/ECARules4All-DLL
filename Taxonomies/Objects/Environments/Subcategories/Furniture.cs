using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    [ECARules4All("forniture")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Furniture : MonoBehaviour
    {
        [StateVariable("price", ECARules4AllType.Float)]
        public float price;
        [StateVariable("color", ECARules4AllType.Color)]
        public Color color;
        [StateVariable("dimension", ECARules4AllType.Float)]
        public float dimension;

    }
}
