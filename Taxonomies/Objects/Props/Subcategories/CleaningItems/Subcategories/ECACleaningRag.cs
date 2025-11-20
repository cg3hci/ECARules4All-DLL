using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECACleaningRag</b> is a component that represents a cleaning rag object used to wash
    /// objects equipped with an <see cref="ECASurface"/> component.
    /// </summary>
    [ECARules4All("cleaningRag")]
    [RequireComponent(typeof(ECASoakableCleaningItem))]
    [DisallowMultipleComponent]
    public class ECACleaningRag : MonoBehaviour
    {
        
        /// <summary>
        /// <b>washes</b> simulates the cleaning action performed by a rag on an object
        /// equipped with an <see cref="ECASurface"/> component, typically a surface within the environment.
        /// When executed, it removes dirt, dust, or stains from the surface. If the rag contains water or detergent,
        /// the action represents a washing process; otherwise, it performs a dry wiping action.
        /// Both cases trigger the <b>washes</b> automation event.
        /// When interacting with surfaces equipped with an <see cref="ECAOilStain"/> component, it removes oil stains,
        /// and when the surface includes an <see cref="ECADustBall"/> component, it removes dust balls.
        /// This method is automatically invoked when the rag comes into contact with an object equipped
        /// with an <see cref="ECASurface"/> component, typically detected through a collision event.
        /// </summary>
        /// <param name="surface">The object equipped with an <see cref="ECASurface"/> component representing the surface to be cleaned.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACleaningRag), "washes", typeof(ECASurface))]
        public void Washes(ECASurface surface)
        {
            Log.Debug(this.gameObject + " washes (cleaningRag) with " + surface.gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the object is a ECASurface, it triggers the "Washes" method and publishes the action to the event system.
            Log.Information("[ECACleaningItem - OnTriggerEnter] " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
            if (surface != null)
            {
                Log.Debug("[ECACleaningItem - OnTriggerEnter] The rag is washing the surface: " + surface.gameObject.name);
                Washes(surface);
                Action action = new Action(this.gameObject, "washes", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action);
            }
        }
    }
}
