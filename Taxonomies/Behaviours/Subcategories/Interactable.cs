using ECARules4All_DLL.Utils;
using UnityEngine;
using Serilog;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Interactable</b> is a <see cref="Behaviour">Behaviour</see> that can be attached to an object in order to make it
    /// interactable with the player collison. If the action is not player initiated, then refer to <see cref="Trigger"/>
    /// </summary>
    [ECARules4All("interactable")]
    [RequireComponent(typeof(Behaviour))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class Interactable : ECAScript
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("NonEcaInteractable")) return;
            
            //PROVE
            Action action = new Action(other.gameObject, "interacts with", this.gameObject);
            Log.Information("Interacts-with (trigger) " + other.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("NonEcaInteractable")) return;

            Action action = new Action(other.gameObject, "interacts with", this.gameObject);
            Log.Information("Interacts-with (collision) " + other.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
        }

        private void OnTriggerExit(Collider other)
        {
            //PROVE
            if (other.CompareTag("NonEcaInteractable")) return;
            Action action = new Action(other.gameObject, "stops-interacting with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("NonEcaInteractable")) return;
            Log.Information("OnCollisionExit 1");
            Action action = new Action(other.gameObject, "stops-interacting with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
            Log.Information($"OnCollisionExit 2 - other: {other.gameObject.name} - obj: {this.gameObject.name}");
        }
    }
}
