using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// TODO ADD
    /// </summary>
    [ECARules4All("broom")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECABroom : MonoBehaviour
    {

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABroom), "sweeps", typeof(ECASurface))]
        public void Sweeps(ECASurface surface)
        {
            //TODO Make the logic. I think no logic is needed here, just the notification.
            Debug.Log(this.gameObject + " sweeps (broom) with " + surface.gameObject.name);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("STO TRIGGERANDO CON " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
            if (surface != null)
            {
                Debug.Log("AAA The broom is sweeping the surface: " + surface.gameObject.name);
                Sweeps(surface);
                Action action = new Action(this.gameObject, "sweeps", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }
    }
}