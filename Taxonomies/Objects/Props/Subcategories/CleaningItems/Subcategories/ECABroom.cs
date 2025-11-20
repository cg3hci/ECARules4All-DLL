using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECABroom</b> is a component that represents broom objects within the environment.
    /// It defines the properties and capabilities associated with virtual brooms used in automation scenarios.
    /// </summary>
    [ECARules4All("broom")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECABroom : MonoBehaviour
    {
        
        /// <summary>
        /// <b>sweeps</b> represents the action of a broom cleaning an object equipped with an <see cref="ECASurface"/> component,
        /// typically a surface within the environment. This method is usually invoked when the broom comes into contact
        /// with an object equipped with an <see cref="ECASurface"/> component.
        /// When executed, if it interacts with objects equipped with both <see cref="ECASurface"/> and ECADustBall components,
        /// it removes any dust balls present, triggering the corresponding <b>remove-dust</b> action.
        /// In addition to removing dust, this action generally activates the <b>collects-dust</b> action of nearby objects
        /// equipped with an <see cref="ECADustPan"/> component, allowing them to collect the detached dust balls.
        /// </summary>
        /// <param name="surface">The object equipped with an <see cref="ECASurface"/> component representing the surface to be swept.</param>
        [ECARelevance(true)]
        [Action(typeof(ECABroom), "sweeps", typeof(ECASurface))]
        public void Sweeps(ECASurface surface)
        {
            Log.Debug(this.gameObject + " sweeps (broom) with " + surface.gameObject.name);
        }

        
        private void OnTriggerEnter(Collider other)
        {
            // If the collided object has an ECASurface component, the broom sweeps it,
            // and the action is published to the event system and optionally triggers an ECA update.
            
            Log.Information("[ECABroom - OnTriggerEnter] " + other.gameObject.name);

            ECASurface surface = other.gameObject.GetComponent<ECASurface>();
            if (surface != null)
            {
                Log.Debug("[ECABroom - OnTriggerEnter] The broom is sweeping the surface: " + surface.gameObject.name);
                Sweeps(surface);
                Action action = new Action(this.gameObject, "sweeps", surface.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action);
            }
        }
    }
}