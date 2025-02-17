using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Characters.Animals.Subcategories
{
    /// <summary>
    /// The <b>AquaticAnimal</b> class represents an aquatic animal.
    /// An AquaticAnimal can swim to specific positions or follow predefined paths, with animations for both swimming and idling states.
    /// This class extends the functionality of <see cref="Animal"/> to include aquatic-specific behaviors.
    /// </summary>
    [ECARules4All("aquatic-animal")]
    [RequireComponent(typeof(Animal))]
    [DisallowMultipleComponent]
    public class AquaticAnimal : MonoBehaviour
    {
        private bool isBusyMoving = false;

        /// <summary>
        /// <b>IdleAnimation</b> specifies the name of the animation clip played when the aquatic animal is idle.
        /// </summary>
        public string IdleAnimation = "";

        /// <summary>
        /// <b>SwimAnimation</b> specifies the name of the animation clip played when the aquatic animal is swimming.
        /// </summary>
        public string SwimAnimation = "";

        /// <summary>
        /// <b>Swims</b> (to) is a method that moves the aquatic animal to a specific position with a swimming animation.
        /// </summary>
        /// <param name="p">The target position to swim to.</param>
        [ECARelevance(false)]
        [Action(typeof(AquaticAnimal), "swims to", typeof(Position))]
        public void Swims(Position p)
        {
            float speed = 0.5F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Swims</b> (on) is a method that moves the aquatic animal along a specified path while playing the swimming animation.
        /// </summary>
        /// <param name="p">The path to follow while swimming.</param>
        [Action(typeof(AquaticAnimal), "swims on", typeof(Path))]
        public void Swims(Path p)
        {
            StartCoroutine(WaitForOrderedMovement(p, "swims"));
        }

        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            Animator anim = gameObject.GetComponent<Animator>();
            anim.Play(SwimAnimation);
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
            anim.Play(IdleAnimation);
        }

        private IEnumerator WaitForOrderedMovement(Path p, string method)
        {
            foreach (Position pos in p.Points)
            {
                while (isBusyMoving)
                {
                    yield return null;
                }

                switch (method)
                {
                    case "swims":
                        Swims(pos);
                        break;
                }

            }
        }
    }
}