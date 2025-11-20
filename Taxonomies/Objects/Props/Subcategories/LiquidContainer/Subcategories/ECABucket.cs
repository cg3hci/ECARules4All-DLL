using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidContainer.Subcategories
{
    /// <summary>
    /// <b>ECABucket</b> is a component that represents a virtual bucket capable of containing different types of virtual
    /// liquids within the environment.
    /// It extends the functionality of <see cref="ECALiquidContainer"/> by specializing it as a bucket-type container.
    /// The bucket can be filled with liquids such as water, degreaser, disinfectant, or battery killer,
    /// and provides visual feedback to indicate both the current fill level and the type of liquid contained.
    /// </summary>
    [ECARules4All("bucket")]
    [RequireComponent(typeof(ECALiquidContainer))]
    [DisallowMultipleComponent]
    public class ECABucket : MonoBehaviour
    {

    }
}
