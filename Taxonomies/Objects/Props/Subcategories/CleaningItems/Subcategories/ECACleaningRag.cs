using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECACleaningRag</b> is a virtual object that simulates a cleaning rag used for washing surfaces.
    /// It interacts with <see cref="ECASurface"/> objects and triggers the washing action when it comes into contact with them.
    /// </summary>
    [ECARules4All("cleaningRag")]
    [RequireComponent(typeof(ECASoakableCleaningItem))]
    [DisallowMultipleComponent]
    public class ECACleaningRag : MonoBehaviour
    {

        /// <summary>
        /// <b>Washes</b> is a method that simulates the action of cleaning a surface using the rag.
        /// It is triggered when the rag interacts with a surface, typically via collision detection.
        /// </summary>
        /// <param name="surface">The <see cref="ECASurface"/> to be washed.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACleaningRag), "washes", typeof(ECASurface))]
        public void Washes(ECASurface surface)
        {
            //TODO Make the logic. I think no logic is needed here, just the notification.
            Debug.Log(this.gameObject + " washes (cleaningRag) with " + surface.gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the object is a ECASurface, it triggers the "Washes" method and publishes the action to the event system.
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
