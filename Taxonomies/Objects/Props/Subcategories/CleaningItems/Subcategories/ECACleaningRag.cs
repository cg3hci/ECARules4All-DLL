using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// TODO ADD
    /// </summary>
    [ECARules4All("cleaningRag")]
    [RequireComponent(typeof(ECASoakableCleaningItem))]
    [DisallowMultipleComponent]
    public class ECACleaningRag : MonoBehaviour
    {

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECACleaningRag), "washes", typeof(Surface))]
        public void Washes(Surface surface)
        {
            //TODO Make the logic. I think no logic is needed here, just the notification.
            Debug.Log(this.gameObject + " washes (cleaningRag) with " + surface.gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("STO TRIGGERANDO CON " + other.gameObject.name);

            Surface surface = other.gameObject.GetComponent<Surface>();
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
