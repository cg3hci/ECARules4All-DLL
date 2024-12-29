using System.Collections;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;


namespace ECARules4All_DLL.Taxonomies.Objects.Characters
{
    /// <summary>
    /// Represents a versatile character within the ECA rules framework. 
    /// A <b>Character</b> can embody various forms, including animals, humanoids, robots, or generic creatures. 
    /// It can operate autonomously or be controlled by the player, supporting a range of actions and state attributes 
    /// to interact dynamically with the environment
    /// </summary>
    [ECARules4All("character")]
    [RequireComponent(typeof(ECAObject), typeof(Animator))]
    [DisallowMultipleComponent]
    public class Character : MonoBehaviour
    {
        /// <summary>
        /// <b>life</b> is the current life of the character, represented as a float number.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("life", ECARules4AllType.Float)]
        public float life
        {
            get => _life;
            set
            {
                _life = value;
                ECAScript.NotifyUpdate(this, nameof(_life), _life.ToString());
            }
        }
        [SerializeField]
        private float _life;
        
        /// <summary>
        /// <b>playing</b> indicates whether the character is controlled by the player ("yes") or operating autonomously ("no").
        /// </summary>
        [StateVariable("playing", ECARules4AllType.Boolean)]
        public ECABoolean playing
        {
            get => _playing;
            set
            {
                _playing = value;
                ECAScript.NotifyUpdate(this, nameof(playing), playing.ToString());
            }
        }
        
        [SerializeField]
        private ECABoolean _playing;

        private bool isBusyMoving = false;
        private Animator anim;

        void Start()
        {
            anim = gameObject.GetComponent<Animator>();
        }

        /// <summary>
        /// <b>Interacts</b> enables the character to interact with a specified interactable object.
        /// The implementation details are managed by the <see cref="Interactable"/> class logic.
        /// </summary>
        /// <param name="o">The target interactable object</param>
        [ECARelevance(true)]
        [Action(typeof(Character), "interacts with", typeof(Interactable))]
        public void Interacts(Interactable o)
        {
            Debug.LogWarning("[CHARACTER] Interacts with " + o.gameObject.name);
            var door = o.GetComponent<ECADoor>();
            Debug.LogWarning("[CHARACTER] Door: " + door);
            if(door != null)
                door.SwitchState();
        }

        /// <summary>
        /// <b>Stops interaction</b> allows the character to stop its interaction with a specified interactable object.
        /// The implementation details are managed by the <see cref="Interactable"/> class logic.
        /// </summary>
        /// <param name="o">The target interactable object</param>
        [ECARelevance(true)]
        [Action(typeof(Character), "stops-interacting with", typeof(Interactable))]
        public void StopsInteracting(Interactable o)
        {
        }
        
        //TODO Cambiare typeof(ECAObject) in typeof(Interactable) o typeof(XRInteractable) o cosa?
        /// <summary>
        /// <b>Points</b> the character to point at a specified object, emphasizing its focus or attention on the target.
        /// </summary>
        /// <param name="o">The target object to point at.</param>
        [Action(typeof(Character), "points to", typeof(ECAObject))]
        public void Points(ECAObject o)
        {
            // TODO is an extension? 
        }


        //TODO Cambiare typeof(ECAObject) in typeof(Interactable) o typeof(XRInteractable) o cosa?
        /// <summary>
        /// <b>StopsPointing</b> commands the character to stop pointing at a specified object, ceasing its focus or attention on the target.
        /// </summary>
        /// <param name="o">The target object to stop pointing at.</param>
        [Action(typeof(Character), "stops-pointing to", typeof(ECAObject))]
        public void StopsPointing(ECAObject o)
        {
            // TODO is an extension?
        }
        

        /// <summary>
        /// <b>Jumps</b> commands the character to jump to a specific position in the 3D world.
        /// </summary>
        /// <param name="p">The destination position where the character will jump.</param>
        [Action(typeof(Character), "jumps to", typeof(Position))]
        public void Jumps(Position p)
        {
            float speed = 5.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            Vector3 startMarker = gameObject.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startMarker, endMarker);
            while (gameObject.transform.position != endMarker)
            {
                float distCovered = (Time.time - startTime) * speed;

                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.

                gameObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                //GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
                GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
                yield return null;
            }

            //GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
            GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
            isBusyMoving = false;
        }

        /// <summary>
        /// <b>Jumps</b> directs the character to follow a predefined path, jumping between each position in the path.
        /// </summary>
        /// <param name="p">The path consisting of multiple positions to follow. Each position is a vector in the 3D world with x, y, and z coordinates.</param>
        [Action(typeof(Character), "jumps on", typeof(Path))]
        public void Jumps(Path p)
        {
            StartCoroutine(WaitForOrderedMovement(p));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator WaitForOrderedMovement(Path p)
        {
            foreach (Position pos in p.Points)
            {
                while (isBusyMoving)
                {
                    yield return null;
                }

                Jumps(pos);
            }
        }
        
        /// <summary>
        /// <b>StartsAnimation</b> triggers a predefined animation for the character, using the provided animation identifier.
        /// </summary>
        /// <param name="s">The string of the animation clip to play</param>
        [ECARelevance(false)]
        [Action(typeof(Character), "starts-animation", typeof(string))]
        public void StartsAnimation(string s)
        {
            //In order to make it works the template builder must specify a trigger that is directly connected to
            //the idle state and brings to the correct animation
            anim.Play(s);
        }
    }
}
