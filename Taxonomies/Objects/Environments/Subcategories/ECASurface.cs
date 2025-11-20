using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories
{
    /// <summary>
    /// <b>ECASurface</b> is a component that represents a virtual surface within the automation environment.
    /// It defines surfaces such as tables, floors, walls, or ceilings that can be detected and interacted with by cleaning items (<see cref="ECACleaningItem"/>) or other objects.
    /// </summary>
    [ECARules4All("surface")]
    [RequireComponent(typeof(ECAEnvironment))]
    [DisallowMultipleComponent]
    public class ECASurface : MonoBehaviour
    {
        /// <summary>
        /// <b>type</b> specifies the kind of surface represented by an object equipped with the ECASurface component.
        /// Valid values include 'table', 'floor', 'wall', and 'ceiling'.
        /// This attribute is used by cleaning items to determine how to interact with the surface.
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