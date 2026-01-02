using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECAScottex</b> represents a virtual disposable paper towel used to clean an object equipped with an
    /// <see cref="ECASurface"/> component, typically a surface within the environment.
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
        /// <b>sweeps</b> represents the action of a paper towel (or scottex) cleaning an object equipped with an <see cref="ECASurface"/> component,
        /// typically representing a surface within the environment. This method is usually invoked when the paper towel
        /// comes into contact with an object equipped with an <see cref="ECASurface"/> component.
        /// When <b>sweeps</b> is used as a trigger in an ECA automation, if it interacts with objects equipped with
        /// both <see cref="ECASurface"/> and <see cref="ECADustBall"/> components, it MUST trigger in the same automation
        /// the corresponding <b>removes-dust-with-scottex</b> action (that removes the dust from the surface). 
        /// </summary>
        /// <param name="surface">The object equipped with an <see cref="ECASurface"/> component representing the surface to be swept.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAScottex), "sweeps", typeof(ECASurface))]
        public void Sweeps(ECASurface surface)
        {
            Log.Debug(this.gameObject + " sweeps (scottex) with " + surface.gameObject.name);
        }


        private void OnTriggerEnter(Collider other)
        {
            // If the collided object has an ECASurface component, this triggers a sweep interaction and publishes the event to the ECA system.
            Log.Information("[ECAScottex - OnTriggerEnter] " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
            if (surface != null)
            {
                Log.Debug("[ECAScottex - OnTriggerEnter] The scottex is sweeping the surface: " + surface.gameObject.name);
                Sweeps(surface);
                Action action = new Action(this.gameObject, "sweeps", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action);
            }
        }
    }
}
