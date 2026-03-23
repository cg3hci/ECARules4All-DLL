using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Characters.Animals.Subcategories
{
    /// <summary>
    /// The <b>FlyingAnimal</b> class represents a flying animal.
    /// An FlyingAnimal can move using flying or walking animations and supports navigation to specific positions or along predefined paths.
    /// This class extends the functionality of <see cref="Animal"/> to include flying-specific behaviors.
    /// </summary>
    [ECARules4All("flying-animal")]
    [RequireComponent(typeof(ECAAnimal))]
    [DisallowMultipleComponent]
    public class FlyingAnimal : MonoBehaviour
    {
        private bool isBusyMoving = false;
        private string selected;

       /// <summary>
        /// <b>IdleAnimation</b> specifies the name of the animation clip played when the flying animal is idle.
        /// </summary>
        public string IdleAnimation;

        /// <summary>
        /// <b>FlyAnimation</b> specifies the name of the animation clip played when the flying animal is flying.
        /// </summary>
        public string FlyAnimation;

        /// <summary>
        /// <b>WalkAnimation</b> specifies the name of the animation clip played when the flying animal is walking.
        /// </summary>
        public string WalkAnimation;

        /// <summary>
        /// <b>Flies</b> (to) is a method that moves the flying animal to a specific position with a flying animation.
        /// </summary>
        /// <param name="p">The target position to fly to.</param>
        [Action(typeof(FlyingAnimal), "flies to", typeof(Position))]
        [ECARelevance(true)]
        public void Flies(Position p)
        {
            float speed = 5.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = FlyAnimation;
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Flies</b> (on) is a method that moves the flying animal along a specified path while playing the flying animation.
        /// </summary>
        /// <param name="p">The path to follow while flying.</param>
        [Action(typeof(FlyingAnimal), "flies on", typeof(Path))]
        [ECARelevance(true)]
        public void Flies(Path p)
        {
            selected = FlyAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "flies"));
        }

        /// <summary>
        /// <b>Walks</b> (to) is a method that moves the flying animal to a specific position with a walking animation.
        /// </summary>
        /// <param name="p">The target position to walk to.</param>
        [Action(typeof(FlyingAnimal), "walks to", typeof(Position))]
        [ECARelevance(true)]
        public void Walks(Position p)
        {
            float speed = 1.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = WalkAnimation;
            StartCoroutine(MoveObject(speed, endMarker));

        }

        /// <summary>
        /// <b>Walks</b> (on) is a method that moves the flying animal along a specified path while playing the walking animation.
        /// </summary>
        /// <param name="p">The path to follow while walking.</param>
        [Action(typeof(FlyingAnimal), "walks on", typeof(Path))]
        public void Walks(Path p)
        {
            selected = WalkAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "walks"));
        }

        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            Animate(selected);
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
                // GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
                GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
                yield return null;
            }

            // GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
            GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
            isBusyMoving = false;
            Animate(IdleAnimation);
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
                    case "flies":
                        Flies(pos);
                        break;
                    case "walks":
                        Walks(pos);
                        break;
                }

            }
        }

        private void Animate(string animation)
        {
            gameObject.GetComponent<Animator>().Play(animation);
        }


    }
}