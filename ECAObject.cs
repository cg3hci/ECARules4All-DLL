using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL
{
    /// <summary>
    /// <b>ECAObject</b> is the base class for all objects that can be used in the rule engine.
    /// All the other classes in this package inherit from this class or one of its subclasses.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECATracker))]
    [ECARules4All("object")]
    public class ECAObject : ECAScript
    {
        /// <summary>
        /// <b> GameCollider </b> is the collider of the object.
        /// </summary>
        private Collider[] gameCollider;
        /// <summary>
        /// <b>GameRender</b> is the renderer of the object.
        /// </summary>
        private Renderer[] gameRenderer;
        /// <summary>
        /// <b>isBusyMoving</b> is a boolean that indicates if the object is moving.
        /// </summary>
        private bool isBusyMoving = false;

        /// <summary>
        /// <b>p</b> is the position of the object.
        /// </summary>
        [StateVariable("position", ECARules4AllType.Position)]
        public Position p
        {
            get => _p;
            set
            {
                _p = value;
                NotifyUpdate(nameof(p), _p);
            }
        }
        private Position _p;
        
        /// <summary>
        /// <b>r</b> is the rotation of the object.
        /// </summary>
        [StateVariable("rotation", ECARules4AllType.Rotation)]
        public Rotation r
        {
            get => _r;
            set
            {
                _r = value;
                NotifyUpdate(nameof(r), _r);
            }
        }
        private Rotation _r;
        
        /// <summary>
        /// <b>r</b> is the scale of the object.
        /// </summary>
        [StateVariable("scale", ECARules4AllType.Scale)]
        public Scale s
        {
            get => _s;
            set
            {
                _s = value;
                NotifyUpdate(nameof(s), _s);
            }
        }
        private Scale _s;
        
        private Vector3 _originalPosition ;
        private Quaternion _originalQuaternion ;
        private Vector3 _originalScale3 ;
        
        /// <summary>
        /// <b>isVisible</b> is a boolean that indicates if the object is visible.
        /// If the object is invisible, it will not be rendered but it will still collide with other objects.
        /// </summary>
        [StateVariable("visible", ECARules4AllType.Boolean)]
        public ECABoolean isVisible // = new ECABoolean(ECABoolean.BoolType.YES);
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                NotifyUpdate(nameof(isVisible), isVisible.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _isVisible = new ECABoolean(ECABoolean.BoolType.YES);
        
        /// <summary>
        /// <b>isActive</b> is a boolean that indicates if the object is active and visible.
        /// If the object is inactive, it will not be rendered and it will not collide with other objects.
        /// </summary>
        [StateVariable("active", ECARules4AllType.Boolean)]
        public ECABoolean isActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                NotifyUpdate(nameof(isActive), isActive.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _isActive = new ECABoolean(ECABoolean.BoolType.YES);
        
        /// <summary>
        /// <b>isInsideCamera</b> is a boolean that indicates if the object is within the camera's field of view.
        /// </summary>
        [StateVariable("isInsideCamera", ECARules4AllType.Boolean)]
        public ECABoolean isInsideCamera
        {
            get => _isInsideCamera;
            set
            {
                _isInsideCamera = value;
                NotifyUpdate(nameof(isInsideCamera), isInsideCamera.ToString());
            }
        }
        private ECABoolean _isInsideCamera = new ECABoolean(ECABoolean.BoolType.NO);
        
        private Canvas canvas;
        public Camera xrCamera;

        void Start()
        {
            // TryGetComponent<Canvas>(out _canvas);
            _originalPosition = gameObject.transform.localPosition;
            _originalQuaternion = gameObject.transform.rotation;
            _originalScale3 = gameObject.transform.localScale;
            //p.Owner = this;
            TryGetComponent<Canvas>(out canvas);
            UpdateVisibility();
        }

        /// <summary>
        /// <b>Moves</b> (to) is a method that moves the object to a new position.
        /// </summary>
        /// <param name="newPos">The new position for the object </param>
        [Action(typeof(ECAObject), "moves to", typeof(Position))]
        public void Moves(Position newPos)
        {
            float speed = 1.0F;
            Vector3 endMarker = new Vector3(newPos.x, newPos.y, newPos.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Moves</b> (on) is a method that moves the object to a new position, following a path.
        /// </summary>
        /// <param name="path">The array of positions the object will follow, one after another</param>
        [Action(typeof(ECAObject), "moves on", typeof(Path))]
        public void Moves(Path path)
        {
            StartCoroutine(WaitForOrderedMovement(path));
        }

        private IEnumerator WaitForOrderedMovement(Path path)
        {
            foreach (Position pos in path.Points)
            {
                while (isBusyMoving)
                {
                    yield return null;
                }

                Moves(pos);
            }
        }

        /// <summary>
        /// <b>Rotates</b> sets the rotation of the object to a new value.
        /// </summary>
        /// <param name="newRot">The new rotation value fo the object. </param>
        [Action(typeof(ECAObject), "rotates around", typeof(Rotation))]
        public void Rotates(Rotation newRot)
        {
            //r.Assign(newRot);
            r = new Rotation(newRot);
            transform.Rotate(r.x, r.y, r.z); // todo verify rotation
        }
        
        /// <summary>
        /// <b>Looks</b> sets the rotation of the object in a way that it looks at a target.
        /// </summary>
        /// <param name="o">The target GameObject to look at.</param>
        [Action(typeof(ECAObject), "looks at", typeof(GameObject))]
        public void Looks(GameObject o)
        {
            ECAObject looked = o.GetComponent<ECAObject>();
            Physics.Linecast (transform.position, o.transform.position, out var hit);
            if (o.name == hit.collider.gameObject.name && looked.isActive)
            {
                transform.LookAt(looked.gameObject.transform);
                //r.Assign(transform.rotation);
                r = new Rotation(transform.rotation);
            }
        }
        
        /// <summary>
        /// <b>Scales</b> sets the scale of the object to a new value.
        /// </summary>
        /// <param name="newScale">The new scale value fo the object. </param>
        [Action(typeof(ECAObject), "scales to", typeof(Scale))]
        public void Scales(Scale newScale)
        {
            //r.Assign(newRot);
            s = new Scale(newScale);
            transform.localScale = new Vector3(s.x, s.y, s.z);
        }
        
        [Action(typeof(ECAObject), "restores original settings")]
        public void MovesOriginalPosition()
        {
            float speed = 3.0F; ;
            StartCoroutine(MoveObject(speed, _originalPosition));
            gameObject.transform.rotation = _originalQuaternion;
            gameObject.transform.localScale = _originalScale3;
        }

        /// <summary>
        /// <b>Shows</b> makes the object visible. It makes it visible if it is not already.
        /// </summary>
        [Action(typeof(ECAObject), "shows")]
        public void Shows()
        {
            //isVisible.Assign(ECABoolean.BoolType.YES);
            isVisible = new ECABoolean(ECABoolean.BoolType.YES);
            UpdateVisibility();
        }
        
        /// <summary>
        /// <b>Hides</b> makes the object invisible. It makes it invisible if it is not already.
        /// </summary>
        [Action(typeof(ECAObject), "hides")]
        public void Hides()
        {
            //isVisible.Assign(ECABoolean.BoolType.NO);
            isVisible = new ECABoolean(ECABoolean.BoolType.NO);
            UpdateVisibility();
        }
        
        /// <summary>
        /// <b>Activates</b> makes the object active. It makes it active if it is not already. 
        /// </summary>
        [Action(typeof(ECAObject), "activates")]
        public void Activates()
        {
            //isActive.Assign(ECABoolean.BoolType.YES);
            isActive = new ECABoolean(ECABoolean.BoolType.YES);
            UpdateVisibility();
        }
        /// <summary>
        /// <b>Deactivates</b> makes the object inactive. It makes it inactive if it is not already.
        /// </summary>
        [Action(typeof(ECAObject), "deactivates")]
        public void Deactivates()
        {
            //isActive.Assign(ECABoolean.BoolType.NO);
            isActive = new ECABoolean(ECABoolean.BoolType.NO);
            UpdateVisibility();
        }
        
        /// <summary>
        /// <b>ShowsHides</b> sets the visibility of the object, defined by a parameter.
        /// </summary>
        /// <param name="yesNo">The new visibility state for the object. </param>
        [Action(typeof(ECAObject), "changes", "visible", "to", typeof(YesNo))]
        public void ShowsHides(ECABoolean yesNo)
        {
            isVisible = yesNo;
            UpdateVisibility();
        }

        /// <summary>
        /// <b>ActivatesDeactivates</b> sets the active state of the object, defined by a parameter.
        /// </summary>
        /// <param name="yesNo"></param>
        [Action(typeof(ECAObject), "changes", "active", "to", typeof(YesNo))]
        public void ActivatesDeactivates(ECABoolean yesNo)
        {
            isActive = yesNo;
            UpdateVisibility();
        }

        private void Awake()
        {
            gameCollider = this.gameObject.GetComponents<Collider>();
            gameRenderer = this.gameObject.GetComponents<Renderer>();

            if (gameRenderer.Length == 0)
                gameRenderer = this.gameObject.GetComponentsInChildren<Renderer>();
            
            if (gameCollider.Length == 0)
                gameCollider = this.gameObject.GetComponentsInChildren<Collider>();
            
            //p = new Position();
            //p.Assign(transform.position);
            p = new Position(transform.position);
            //r = new Rotation();
            //r.Assign(transform.rotation);
            r = new Rotation(transform.rotation);
            s = new Scale(transform.localScale);
        }
        
        //TODO: should be private
        public void UpdateVisibility()
        {

            foreach (Collider c in gameCollider)
            {
                c.enabled = isActive;
            }

            foreach (Renderer r in gameRenderer)
            {
                if (!isActive || !isVisible)
                {
                    r.enabled = false;
                }
                else r.enabled = true;
            }
            
            if (canvas != null)
            {
                if (!isActive || !isVisible)
                {
                    canvas.enabled = false;
                }
                else canvas.enabled = true;            
            }
        }
        
        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            //Vector3 startMarker = gameObject.transform.position;
            Vector3 startMarker = gameObject.transform.localPosition;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startMarker, endMarker);
            //while (gameObject.transform.position != endMarker)
            while (gameObject.transform.localPosition != endMarker) {
                float distCovered = (Time.time - startTime) * speed;

                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.

                gameObject.transform.localPosition = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                //GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
                GetComponent<ECAObject>().p = new Position(gameObject.transform.localPosition);
                yield return null;
            }
            //GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
            GetComponent<ECAObject>().p = new Position(gameObject.transform.localPosition);
            isBusyMoving = false;
        }

        public void Update()
        {
            // check if object is within the camera's field of view.
            // the object must be active
            if (xrCamera)
            {
                Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(xrCamera);

                ECABoolean res = ECABoolean.NO;
                foreach (Renderer render in gameRenderer)
                {
                    if (GeometryUtility.TestPlanesAABB(frustumPlanes, render.bounds))
                    {
                        res = ECABoolean.YES;
                        break;
                    }
                }

                if(this.isInsideCamera != res)
                {
                    this.isInsideCamera = res;
                }
            }
        }
    }
}