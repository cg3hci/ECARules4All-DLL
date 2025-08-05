using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>dustBall</b> is a <see cref="Behaviour">Behaviour</see> that represents dust on surfaces.
    /// </summary>
    [ECARules4All("physicalGrabbable")]
    [RequireComponent(typeof(Behaviour))]
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class ECAPhysicalGrabbable : MonoBehaviour
    {
        [SerializeField] private Collider rightHandCollider;
        [SerializeField] private Collider leftHandCollider;

        private GameObject player_character =>
            GameObject.FindObjectOfType<Character>()
                .gameObject; //TODO J 2nd July '25: Salvare la reference al personaggio del giocatore in un campo privato, per evitare di cercarlo ogni volta.
        
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
        /// <b>grabbed</b> TODO.
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
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(Character), "starts-grabbing")]
        public void _StartsGrabbing(Character c)
        {
            Debug.Log("Starting grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
            grabbed = new ECABoolean(ECABoolean.BoolType.YES);
        }

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(Character), "stops-grabbing")]
        public void _StopsGrabbing(Character c)
        {
            Debug.Log("Stops grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
            grabbed = new ECABoolean(ECABoolean.BoolType.NO);
        }

        private void OnTriggerEnter(Collider other)
        {
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
            // Character c = GameObject.FindWithTag("Player").GetComponent<Character>();
            Debug.Log("ECAPhysicalGrabbable LogicStartsGrabbing called for " + player_character.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
            _StartsGrabbing(player_character.GetComponent<Character>());
            Action action = new Action(player_character, "starts-grabbing", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            Debug.Log("ECAPhysicalGrabbable Finished LogicStartsGrabbing for " + player_character.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
        }

        private void LogicStopsGrabbing()
        {
            Debug.Log("ECAPhysicalGrabbable LogicStopsGrabbing called for " + player_character.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
            _StopsGrabbing(player_character.GetComponent<Character>());
            Action action = new Action(player_character, "stops-grabbing", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            Debug.Log("ECAPhysicalGrabbable Finished LogicStopsGrabbing for " + player_character.name + " on " +
                      this.gameObject.name + " that is son of " + this.transform.parent.name);
        }
    }
}