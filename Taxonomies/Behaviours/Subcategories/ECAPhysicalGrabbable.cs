using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>ECAPhysicalGrabbable</b> represents a physical object in the scene that can be grabbed by the player using one or both hands.
    /// It tracks the grabbing state, handles interaction logic based on trigger collisions with hand colliders, and communicates grabbing events through the automation system.
    /// </summary>
    [ECARules4All("physicalGrabbable")]
    [RequireComponent(typeof(ECABehaviour))]
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class ECAPhysicalGrabbable : MonoBehaviour
    {
        [SerializeField] private Collider rightHandCollider;
        [SerializeField] private Collider leftHandCollider;

        private GameObject player_GoRef => ECAPlayer_Singleton.Instance.playerGoRef;
        private ECACharacter player_ecaCharacterRef => ECAPlayer_Singleton.Instance.playerEcaCharacterRef;
        
        private void Awake()
        {
            // 1. Searches for the game object "HandInteractorsRight" -> "HandGrabInteractor" -> "Rigidbody" -> "GripPoint" -> "Collider"
            // 2. Get the collider and assign it to rightHandCollider
            if (rightHandCollider == null)
            {
                rightHandCollider = GameObject
                    .Find("HandInteractorsRight/HandGrabInteractor/Rigidbody/GripPoint/Collider")
                    .GetComponent<Collider>();
            }

            // 1. Searches for the game object "HandInteractorsLeft" -> "HandGrabInteractor" -> "Rigidbody" -> "GripPoint" -> "Collider"
            // 2. Get the collider and assign it to leftHandCollider
            leftHandCollider = GameObject.Find("HandInteractorsLeft/HandGrabInteractor/Rigidbody/GripPoint/Collider")
                .GetComponent<Collider>();
        }

        /// <summary>
        /// <b>grabbed</b> indicates whether the object is currently being held by the player.
        /// This state is updated based on collision triggers with hand colliders and is used to drive interactive behaviors.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("grabbed", ECARules4AllType.Boolean)]
        public ECABoolean grabbed
        {
            get => _grabbed;
            set
            {
                _grabbed = value;
                ECAScript.NotifyUpdate(this, nameof(grabbed), grabbed.ToString());
            }
        }

        [SerializeField] private ECABoolean _grabbed = new ECABoolean(ECABoolean.BoolType.NO);

        //TODO We need to handle the case where the player grabs with both hands, like
        // Casr 1. Triggers with left -> Stops triggering with left => Trigger "stops-grabbing" action
        // Case 2. Triggers with left -> Triggers with right => We shouldn't trigger the "starts-grabbing" action twice
        // Case 3. Triggers with left -> Triggers with right -> Stops triggering with left => It still grabs right, so it should not stop grabbing
        private bool leftHandGrabbed = false;
        private bool rightHandGrabbed = false;

        /// <summary>
        /// <b>starts-grabbing</b> is triggered when the player begins to grab the object with either hand.
        /// Updates the <see cref="grabbed"/> state and notifies the system about the interaction.
        /// </summary>
        /// <param name="c">The character initiating the grab.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "starts-grabbing", typeof(ECAPhysicalGrabbable))]
        public void _StartsGrabbing(ECACharacter c)
        {
            Debug.Log("Starting grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
            grabbed = new ECABoolean(ECABoolean.BoolType.YES);
        }

        /// <summary>
        /// <b>stops-grabbing</b> is triggered when the player releases the object with both hands.
        /// Resets the <see cref="grabbed"/> state and notifies the system of the interaction ending.
        /// </summary>
        /// <param name="c">The character releasing the object.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "stops-grabbing")]
        public void _StopsGrabbing(ECACharacter c)
        {
            Debug.Log("Stops grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
            grabbed = new ECABoolean(ECABoolean.BoolType.NO);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Handles detection of grabbing initiation by checking if either hand's collider enters this object's collider.
            // Initiates grab logic when the first hand touches the object.
            Debug.Log("OnTriggerEnter: " + other.gameObject.name);
            if (other.CompareTag("NonEcaInteractable")) return;

            if (other == leftHandCollider && !leftHandGrabbed)
            {
                leftHandGrabbed = true;
                if (!rightHandGrabbed)
                {
                    LogicStartsGrabbing(); // First hand to grab
                }
            }
            else if (other == rightHandCollider && !rightHandGrabbed)
            {
                rightHandGrabbed = true;
                if (!leftHandGrabbed)
                {
                    LogicStartsGrabbing(); // First hand to grab
                }
            }

        }

        private void OnTriggerExit(Collider other)
        {
            // Handles detection of grabbing termination when both hands are no longer in contact with the object.
            // Triggers ungrab logic only when both hand colliders have exited.
            Debug.Log("OnTriggerExit: " + other.gameObject.name);
            if (other.CompareTag("NonEcaInteractable")) return;

            if (other == leftHandCollider && leftHandGrabbed)
            {
                leftHandGrabbed = false;
            }
            else if (other == rightHandCollider && rightHandGrabbed)
            {
                rightHandGrabbed = false;
            }

            // Only stop grabbing if both hands have released
            if (!leftHandGrabbed && !rightHandGrabbed)
            {
                LogicStopsGrabbing();
            }
        }

        private void LogicStartsGrabbing()
        {
            // Internal logic for handling the start of a grab interaction.
            // Publishes an ECA action and updates the system state accordingly.
            
            // Character c = GameObject.FindWithTag("Player").GetComponent<Character>();
            Debug.Log("ECAPhysicalGrabbable LogicStartsGrabbing called for " + player_GoRef.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
            _StartsGrabbing(player_ecaCharacterRef);
            Action action = new Action(player_GoRef, "starts-grabbing", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            Debug.Log("ECAPhysicalGrabbable Finished LogicStartsGrabbing for " + player_GoRef.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
        }

        private void LogicStopsGrabbing()
        {
            // Internal logic for handling the end of a grab interaction.
            // Publishes an ECA action and updates the system state accordingly.
            
            Debug.Log("ECAPhysicalGrabbable LogicStopsGrabbing called for " + player_GoRef.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
            _StopsGrabbing(player_ecaCharacterRef);
            Action action = new Action(player_GoRef, "stops-grabbing", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            Debug.Log("ECAPhysicalGrabbable Finished LogicStopsGrabbing for " + player_GoRef.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
        }
    }
}