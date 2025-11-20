using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems
{
    /// <summary>
    /// <b>ECACleaningItem</b> is a base component that represents a generic cleaning tool.
    /// It provides shared properties and behaviors for all cleaning-related objects
    /// and enables integration with other components and ECA-based automation rules.
    /// </summary>
    [ECARules4All("cleaningItem")]
    [RequireComponent(typeof(ECAProp))]
    [DisallowMultipleComponent]
    public class ECACleaningItem : MonoBehaviour
    {

    }
}
