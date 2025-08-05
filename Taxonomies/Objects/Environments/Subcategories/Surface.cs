using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    /// <summary>
    /// Defines a  TODO.
    /// </summary>
    [ECARules4All("surface")]
    [RequireComponent(typeof(Environment))]
    [DisallowMultipleComponent]
    public class Surface : MonoBehaviour
    {
        /// <summary>
        /// <b>type</b> specifies 
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("type", ECARules4AllType.Text)]
        public string type
        {
            get => _type;
            set
            {
                _type = value;
                ECAScript.NotifyUpdate(this, nameof(type), type);
            }
        }

        [SerializeField] private string _type = "?";
    }
}