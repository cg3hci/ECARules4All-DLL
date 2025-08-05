using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    /// <summary>
    /// <b>ECASurface</b> represents a physical or virtual surface within the automation environment.
    /// It is used to define surfaces such as tables, floors, walls, or ceilings that cleaning items can interact with.
    /// </summary>
    [ECARules4All("surface")]
    [RequireComponent(typeof(ECAEnvironment))]
    [DisallowMultipleComponent]
    public class ECASurface : MonoBehaviour
    {
        /// <summary>
        /// <b>type</b> specifies the kind of surface.
        /// Possible values include "table", "floor", "wall", or "ceiling".
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