using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidContainer.Subcategories
{
    /// <summary>
    /// <b>ECABucket</b> represents a virtual bucket that can contain different types of virtual liquids in the automation system.
    /// It extends the functionality of <see cref="ECALiquidContainer"/> by specializing the container as a bucket.
    /// This class can be filled with water, degreaser, amuchina, or battery killer, and it supports visual feedback such as fill level and liquid type.
    /// </summary>
    [ECARules4All("bucket")]
    [RequireComponent(typeof(ECALiquidContainer))]
    [DisallowMultipleComponent]
    public class ECABucket : MonoBehaviour
    {

    }
}
