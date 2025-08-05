using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECAMop</b> represents a virtual mop used to clean surfaces within the simulation.
    /// It is designed to interact with surfaces by "washing" them, typically triggered when coming into contact with a surface.
    /// </summary>
    [ECARules4All("mop")]
    [RequireComponent(typeof(ECASoakableCleaningItem))]
    [DisallowMultipleComponent]
    public class ECAMop : MonoBehaviour
    {
        /// <summary>
        /// <b>Washes</b> is a method that simulates the action of the mop cleaning a surface.
        /// This action is triggered when the mop collides with a surface object, and notifies the automation system accordingly.
        /// </summary>
        /// <param name="surface">The surface being cleaned by the mop.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAMop), "washes", typeof(ECASurface))]
        public void Washes(ECASurface surface)
        {
            Debug.Log(this.gameObject + " washes (mop) with " + surface.gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the collided object contains an ECASurface component, the mop performs a wash action.
            // This action is then published to the event bus and optionally triggers an ECA system update.
            Debug.Log("STO TRIGGERANDO CON " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
            if (surface != null)
            {
                Debug.Log("AAA The rag is washing the surface: " + surface.gameObject.name);
                Washes(surface);
                Action action = new Action(this.gameObject, "washes", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }
    }
}
