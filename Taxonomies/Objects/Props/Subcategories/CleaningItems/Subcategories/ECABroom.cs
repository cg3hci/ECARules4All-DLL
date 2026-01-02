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
        /// <b>sweeps</b> represents a triggerable interaction performed by a broom on an object equipped with an <see cref="ECASurface"/> component.
        /// When <b>sweeps</b> is used as a trigger in an ECA automation, it implicitly defines a causal action within the same automation.
        ///
        /// Specifically, if the swept surface is equipped with both <see cref="ECASurface"/> and ECADustBall components,
        /// the trigger <b>sweeps</b> MUST result in the execution of the <b>removes-dust-with-broom</b> action.
        /// This action removes all dust balls present on the surface.
        ///
        /// Therefore, in an ECA rule, <b>sweeps</b> should be interpreted not only as a trigger condition
        /// but also as a semantic indicator that the automation includes the action <b>removes-dust-with-broom</b>
        /// applied to the same surface involved in the trigger.
        ///
        /// Optionally, the removal of dust can enable secondary actions (e.g., dust collection)
        /// by nearby objects equipped with an <see cref="ECADustPan"/> component.
        /// </summary>
        /// <param name="surface">
        /// The object equipped with an <see cref="ECASurface"/> component representing the surface being swept.
        /// </param>

        
        /// <summary>
        /// <b>sweeps</b> represents the action of a broom cleaning an object equipped with an <see cref="ECASurface"/> component,
        /// typically a surface within the environment. This method is usually invoked when the broom comes into contact
        /// with an object equipped with an <see cref="ECASurface"/> component.
        /// When <b>sweeps</b> is used as a trigger in an ECA automation, if it interacts with objects equipped with
        /// both <see cref="ECASurface"/> and <see cref="ECADustBall"/> components, it MUST trigger in the same automation
        /// the corresponding <b>removes-dust-with-broom</b> action. 
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