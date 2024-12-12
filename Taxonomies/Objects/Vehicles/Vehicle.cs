using System;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Vehicles
{
    [ECARules4All("vehicle")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Vehicle : MonoBehaviour
    {
        [StateVariable("speed", ECARules4AllType.Float)]
        public float speed
        {
            get => _speed;
            set
            {
                _speed = value;
                ECAScript.NotifyUpdate(this, nameof(speed), speed.ToString());
            }
        }
        [SerializeField]
        private float _speed;

        [StateVariable("on", ECARules4AllType.Boolean)]
        public ECABoolean on 
        {
            get => _on;
            set
            {
                _on = value;
                ECAScript.NotifyUpdate(this, nameof(on), on.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _on = new ECABoolean(ECABoolean.BoolType.OFF);

        private Vector3 localForward;
        private ECAObject reference;

        [Action(typeof(Vehicle), "starts")]
        public void Starts()
        {
            //on.Assign(ECABoolean.BoolType.ON);
            on = new ECABoolean(ECABoolean.BoolType.ON);
        }

        //TODO: verb not present in grammar
        [Action(typeof(Vehicle), "steers-at", typeof(float))]
        public void Steers(float angle)
        {
            Quaternion rotation = gameObject.transform.rotation;
            gameObject.transform.Rotate(rotation.x, rotation.y + angle, rotation.z);
        }

        [Action(typeof(Vehicle), "accelerates-by", typeof(float))]
        public void Accelerates(float f)
        {
            speed += f;
        }

        [Action(typeof(Vehicle), "slows-by", typeof(float))]
        public void SlowsDown(float f)
        {
            speed -= f;
        }

        [Action(typeof(Vehicle), "stops")]
        public void Stops()
        {
            //on.Assign(ECABoolean.BoolType.OFF);
            on = new ECABoolean(ECABoolean.BoolType.OFF);
            speed = 0;
        }

        private void Start()
        {
            localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            reference = GetComponent<ECAObject>();
            //reference.p.Assign(gameObject.transform.position);
            //reference.r.Assign(gameObject.transform.rotation);
            reference.p = new Position(gameObject.transform.position);
            reference.r = new Rotation(gameObject.transform.rotation);
        }

        private void Update()
        {
            if (on)
            {
                transform.Translate(speed * Time.deltaTime * localForward);
                //reference.p.Assign(gameObject.transform.position);
                //reference.r.Assign(gameObject.transform.rotation);
                reference.p = new Position(gameObject.transform.position);
                reference.r = new Rotation(gameObject.transform.rotation);
            }
        }
    }
}
