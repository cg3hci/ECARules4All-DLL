using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECAMop</b> is a component that represents a virtual mop used to wash objects equipped with an <see cref="ECASurface"/> component,
    /// typically representing surfaces within the environment.
    /// </summary>
    [ECARules4All("mop")]
    [RequireComponent(typeof(ECASoakableCleaningItem))]
    [DisallowMultipleComponent]
    public class ECAMop : MonoBehaviour
    {
        
        /// <summary>
        /// <b>washes</b> specifies the action of a mop cleaning an object equipped with an <see cref="ECASurface"/> component,
        /// typically representing a surface within the environment. This method is typically invoked when the mop comes
        /// into contact with an object equipped with an <see cref="ECASurface"/> component, and it assumes that
        /// the mop has the property <b>hasWater</b> set to <c>true</c>.
        /// When executed, if it interacts with objects equipped with both <see cref="ECASurface"/> and <see cref="ECAOilStain"/> components,
        /// it removes dirt, liquid residues, or stains from the surface as part of the cleaning process.
        /// </summary>
        /// <param name="surface">The object equipped with an <see cref="ECASurface"/> component representing the surface to be cleaned.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAMop), "washes", typeof(ECASurface))]
        public void Washes(ECASurface surface)
        {
            Log.Debug(this.gameObject + " washes (mop) with " + surface.gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the collided object contains an ECASurface component, the mop performs a wash action.
            // This action is then published to the event bus and optionally triggers an ECA system update.
            Log.Information("[ECAMop - OnTriggerEnter] " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
            if (surface != null)
            {
                Log.Debug("[ECAMop - OnTriggerEnter] The rag is washing the surface: " + surface.gameObject.name);
                Washes(surface);
                Action action = new Action(this.gameObject, "washes", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }
    }
}
