using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>Broom</b> is a class that represents a broom.
    /// </summary>
    [ECARules4All("broom")]
    [RequireComponent(typeof(Prop))]
    [DisallowMultipleComponent]
    public class Broom : MonoBehaviour
    {
        /// <summary>
        /// <b>Sweeps</b>: The action of sweeping with the broom.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(Broom), "sweeps")]
        public void Sweeps()
        {
            Debug.Log("SWEEPING");
        }
    }
}