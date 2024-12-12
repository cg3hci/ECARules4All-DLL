using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Characters.Animals.Subcategories
{
    /// <summary>
    /// The <b>Robot</b> class represents a robot character (non-animal counterpart of a human).
    /// A Robot can perform various movements such as running, walking, and swimming, each with a specific animation.
    /// This class extends the functionality of <see cref="Animal"/> to include robot-specific behaviors.
    /// </summary>
    [ECARules4All("robot")]
    [RequireComponent(typeof(Animal))]
    [DisallowMultipleComponent]
    public class Robot : MonoBehaviour
    {
        private bool isBusyMoving = false;

        /// <summary>
        /// <b>IdleAnimation</b> specifies the name of the animation clip played when the robot is idle.
        /// </summary>
        public string IdleAnimation;

        /// <summary>
        /// <b>SwimAnimation</b> specifies the name of the animation clip played when the robot is swimming.
        /// </summary>
        public string SwimAnimation;

        /// <summary>
        /// <b>RunAnimation</b> specifies the name of the animation clip played when the robot is running.
        /// </summary>
        public string RunAnimation;

        /// <summary>
        /// <b>WalkAnimation</b> specifies the name of the animation clip played when the robot is walking.
        /// </summary>
        public string WalkAnimation;

        private string selected;

        /// <summary>
        /// <b>Runs</b> (to) is a method that moves the robot to a specific position with a running animation.
        /// </summary>
        /// <param name="p">The target position to run to.</param>
        [Action(typeof(Robot), "runs to", typeof(Position))]
        public void Runs(Position p)
        {
            float speed = 2.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = RunAnimation;
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Runs</b> (on) is a method that moves the robot along a specified path while playing the running animation.
        /// </summary>
        /// <param name="p">The path to follow while running.</param>
        [Action(typeof(Robot), "runs on", typeof(Path))]
        public void Runs(Path p)
        {
            selected = RunAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "runs"));
        }

        /// <summary>
        /// <b>Swims</b> (to) is a method that moves the robot to a specific position with a swimming animation.
        /// </summary>
        /// <param name="p">The target position to swim to.</param>
        [Action(typeof(Robot), "swims to", typeof(Position))]
        public void Swims(Position p)
        {
            float speed = 0.5F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = SwimAnimation;
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Swims</b> (on) is a method that moves the robot along a specified path while playing the swimming animation.
        /// </summary>
        /// <param name="p">The path to follow while swimming.</param>
        [Action(typeof(Robot), "swims on", typeof(Path))]
        public void Swims(Path p)
        {
            selected = SwimAnimation;
            StartCoroutine(WaitForOrderedMovement(p, "swims"));
        }

        /// <summary>
        /// <b>Walks</b> (to) is a method that moves the robot to a specific position with a walking animation.
        /// </summary>
        /// <param name="p">The target position to move to.</param>
        [ECARelevance(true)]
        [Action(typeof(Robot), "walks to", typeof(Position))]
        public void Walks(Position p)
        {
            float speed = 1.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            selected = WalkAnimation;
            StartCoroutine(MoveObject(speed, endMarker));

        }

        /// <summary>
        /// <b>Walks</b> (on) is a method that moves the robot along a specified path while playing the walking animation.
        /// </summary>
        /// <param name="p">The path to follow while walking.</param>
        [ECARelevance(true)]
        [Action(typeof(Robot), "walks on", typeof(Path))]
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
                //GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
                GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
                yield return null;
            }
            //GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
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
                    case "runs":
                        Runs(pos);
                        break;
                    case "swims":
                        Swims(pos);
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
