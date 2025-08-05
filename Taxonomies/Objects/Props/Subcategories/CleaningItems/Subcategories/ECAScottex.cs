using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// TODO
    /// </summary>
    [ECARules4All("scottex")]
    [RequireComponent(typeof(ECASoakableCleaningItem))]
    [DisallowMultipleComponent]
    public class ECAScottex : MonoBehaviour
    {
        private ECASoakableCleaningItem _soakableCleaningItem;

        private void Awake()
        {
            _soakableCleaningItem = GetComponent<ECASoakableCleaningItem>();
        }

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAScottex), "sweeps", typeof(Surface))]
        public void Sweeps(Surface surface)
        {
            //TODO Make the logic. Update. I think no logic is needed here, just the notification.
            Debug.Log(this.gameObject + " sweeps (scottex) with " + surface.gameObject.name);
        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("STO TRIGGERANDO CON " + other.gameObject.name);

            Surface surface = other.gameObject.GetComponent<Surface>();
            if (surface != null)
            {
                Debug.Log("AAA The scottex is sweeping the surface: " + surface.gameObject.name);
                Sweeps(surface);
                Action action = new Action(this.gameObject, "sweeps", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }
    }
}
