using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Vehicles.Subcategories
{
    [ECARules4All("space-vehicle")]
    [RequireComponent(typeof(Vehicle))]
    [DisallowMultipleComponent]
    public class SpaceVehicle : ECAScript
    {
        [StateVariable("oxygen", ECARules4AllType.Float)]
        public float oxygen
        {
            get => _oxygen;
            set
            {
                _oxygen = value;
                NotifyUpdate(nameof(oxygen), oxygen.ToString());
            }
        }
        [SerializeField]
        private float _oxygen;

        [StateVariable("gravity", ECARules4AllType.Float)]
        public float gravity
        {
            get => _gravity;
            set
            {
                _gravity = value;
                NotifyUpdate(nameof(gravity), gravity.ToString());
            }
        }
        [SerializeField]
        private float _gravity;

        [Action(typeof(SpaceVehicle), "takes-off", typeof(Position))]
        public void TakesOff(Position p)
        {
            float speed = 20.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        [Action(typeof(SpaceVehicle), "lands", typeof(Position))]
        public void Lands(Position p)
        {
            float speed = 20.0F;
            Vector3 endMarker = new Vector3(p.x, p.y, p.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            Vector3 startMarker = gameObject.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startMarker, endMarker);
            while (gameObject.transform.position != endMarker)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;
                gameObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                yield return null;
            }
        }
    }
}
