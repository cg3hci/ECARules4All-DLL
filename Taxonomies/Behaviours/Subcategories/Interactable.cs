using ECARules4All_DLL.Utils;
using UnityEngine;


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
            //PROVE
            Action action = new Action(other.gameObject, "interacts with", this.gameObject);
            Debug.Log("Interacts-with (trigger) " + other.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
        }

        private void OnCollisionEnter(Collision other)
        {
            Action action = new Action(other.gameObject, "interacts with", this.gameObject);
            Debug.Log("Interacts-with (collision) " + other.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
        }

        private void OnTriggerExit(Collider other)
        {
            //PROVE
            Action action = new Action(other.gameObject, "stops-interacting with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
        }

        private void OnCollisionExit(Collision other)
        {
            Debug.Log("OnCollisionExit 1");
            Action action = new Action(other.gameObject, "stops-interacting with", this.gameObject);
            EventBus.GetInstance().Publish(action);
            NotifyUpdate(action);
            Debug.Log($"OnCollisionExit 2 - other: {other.gameObject.name} - obj: {this.gameObject.name}");
        }
    }
}
