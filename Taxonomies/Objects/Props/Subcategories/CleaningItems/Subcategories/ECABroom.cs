using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECABroom</b> is a virtual cleaning tool used to simulate sweeping actions within an interactive environment.
    /// When it comes into contact with a surface, it triggers the sweeping action, which is then propagated through the automation system.
    /// </summary>
    [ECARules4All("broom")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECABroom : MonoBehaviour
    {

        /// <summary>
        /// <b>Sweeps</b> is a method that simulates the broom sweeping a surface.
        /// It is typically invoked upon collision with an <see cref="ECASurface"/>, either manually or automatically.
        /// </summary>
        /// <param name="surface">The surface to be swept by the broom.</param>
        [ECARelevance(true)]
        [Action(typeof(ECABroom), "sweeps", typeof(ECASurface))]
        public void Sweeps(ECASurface surface)
        {
            Debug.Log(this.gameObject + " sweeps (broom) with " + surface.gameObject.name);
        }

        
        private void OnTriggerEnter(Collider other)
        {
            // If the collided object has an ECASurface component, the broom sweeps it,
            // and the action is published to the event system and optionally triggers an ECA update.
            
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