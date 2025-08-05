using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECAScottex</b> represents a virtual disposable paper towel used to clean surfaces in the simulation.
    /// It interacts with surfaces by sweeping over them and is automatically linked to a soakable cleaning system via the <see cref="ECASoakableCleaningItem"/> component.
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
        /// <b>Sweeps</b> is a method that simulates the action of the scottex wiping or cleaning a given surface.
        /// This action is used to trigger an ECA event when a surface is swept by the object.
        /// </summary>
        /// <param name="surface">The surface being swept by the scottex.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAScottex), "sweeps", typeof(ECASurface))]
        public void Sweeps(ECASurface surface)
        {
            Debug.Log(this.gameObject + " sweeps (scottex) with " + surface.gameObject.name);
        }


        private void OnTriggerEnter(Collider other)
        {
            // If the collided object has an ECASurface component, this triggers a sweep interaction and publishes the event to the ECA system.
            Debug.Log("STO TRIGGERANDO CON " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
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
