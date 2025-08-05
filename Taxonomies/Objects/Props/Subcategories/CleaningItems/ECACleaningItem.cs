using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems
{
    /// <summary>
    /// <b>ECACleaningItem</b> represents a generic cleaning item within the automation framework.
    /// It serves as a base component for all cleaning tools and supports integration with ECA objects.
    /// </summary>
    [ECARules4All("cleaningItem")]
    [RequireComponent(typeof(ECAProp))]
    [DisallowMultipleComponent]
    public class ECACleaningItem : MonoBehaviour
    {

    }
}
