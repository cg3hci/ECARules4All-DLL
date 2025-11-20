using ECARules4All_DLL.Utils;
using UnityEngine;
using Serilog;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Interactable</b> is a component that can be attached to an object in order to make it
    /// interactable with other objects collision.
    /// </summary>
    [ECARules4All("interactable")]
    [RequireComponent(typeof(ECABehaviour))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class ECAInteractable : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("NonEcaInteractable")) return;
            
            //PROVE
            Action action = new Action(other.gameObject, "interacts with", this.gameObject);
            Log.Information(other.gameObject + "Interacts-with (trigger) " + this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("NonEcaInteractable")) return;

            Action action = new Action(other.gameObject, "interacts with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
            Log.Information(other.gameObject + "Interacts-with (collision) " + this.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            //PROVE
            if (other.CompareTag("NonEcaInteractable")) return;
            Action action = new Action(other.gameObject, "stops-interacting with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
            Log.Information(other.gameObject + "stops-interacting with (trigger) " + this.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("NonEcaInteractable")) return;
            Action action = new Action(other.gameObject, "stops-interacting with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
            Log.Information(other.gameObject + "stops-interacting with (collision) " + this.gameObject);
        }
    }
}
