using System;
using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming


namespace ECARules4All_DLL
{
    /// <summary>
    /// <b>ECAObject</b> is the base class for all virtual objects that can be used in the automations.
    /// All the other classes in this package inherit from this class or one of its subclasses.
    /// It supports properties such as position, rotation, scale, visibility, and activity, and provides methods for moving, rotating, scaling, and controlling visibility.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECATracker))]
    [ECARules4All("object")]
    public class ECAObject : MonoBehaviour
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

        private float deltaTimeIsRendered = 0;

        /// <summary>
        /// <b>description</b> describes in a few words what the object is and its role.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("description", ECARules4AllType.Text)]
        public string description
        {
            get => _description;
            set
            {
                _description = value;
                ECAScript.NotifyUpdate(this, nameof(description), _description);
            }
        }

        [SerializeField] private string _description;

        /// <summary>
        /// <b>p</b> represents the position of the virtual object in the 3D space. It's a vector with three components: x, y, and z.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("position", ECARules4AllType.Position)]
        public Position p
        {
            get => _p;
            set
            {
                _p = value;
                ECAScript.NotifyUpdate(this, nameof(p), _p);
            }
        }

        private Position _p;

        /// <summary>
        /// <b>r</b> represents the rotation of the object in the 3D space. It's a vector with three components: x, y, and z (euler angles).
        /// </summary>
        [StateVariable("rotation", ECARules4AllType.Rotation)]
        public Rotation r
        {
            get => _r;
            set
            {
                _r = value;
                ECAScript.NotifyUpdate(this, nameof(r), _r);
            }
        }

        private Rotation _r;

        /// <summary>
        /// <b>r</b> represents the scale of the object in the 3D space.
        /// </summary>
        [StateVariable("scale", ECARules4AllType.Scale)]
        public Scale s
        {
            get => _s;
            set
            {
                _s = value;
                ECAScript.NotifyUpdate(this, nameof(s), _s);
            }
        }

        private Scale _s;

        private Vector3 _originalPosition;
        private Quaternion _originalQuaternion;
        private Vector3 _originalScale;

        /// <summary>
        /// <b>visible</b> indicates whether the object is visible. The allowed values are either "yes" or "no".
        /// If invisible, the object is not rendered but remains interactive for collisions.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("visible", ECARules4AllType.Boolean)]
        public ECABoolean isVisible // = new ECABoolean(ECABoolean.BoolType.YES);
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                ECAScript.NotifyUpdate(this, nameof(isVisible), isVisible.ToString());
            }
        }

        [SerializeField] private ECABoolean _isVisible = new ECABoolean(ECABoolean.BoolType.YES);

        /// <summary>
        /// <b>active</b> indicates whether the object is active. The allowed values are either "yes" or "no".
        /// When inactive, the object is not rendered and does not interact with other objects.
        /// </summary>
        [StateVariable("active", ECARules4AllType.Boolean)]
        public ECABoolean isActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                ECAScript.NotifyUpdate(this, nameof(isActive), isActive.ToString());
            }
        }

        [SerializeField] private ECABoolean _isActive = new ECABoolean(ECABoolean.BoolType.YES);

        /// <summary>
        /// <b>isInsideCamera</b> indicates whether the object is currently within the camera's field of view. This property is automatically updated at runtime.
        /// </summary>
        [StateVariable("isInsideCamera", ECARules4AllType.Boolean)]
        public ECABoolean isInsideCamera
        {
            get => _isInsideCamera;
            set
            {
                _isInsideCamera = value;
                ECAScript.NotifyUpdate(this, nameof(isInsideCamera), isInsideCamera.ToString());
            }
        }

        private ECABoolean _isInsideCamera = new ECABoolean(ECABoolean.BoolType.NO);

        private Canvas canvas;
        private Camera xrCamera;

        private void Awake()
        {
            gameCollider = this.gameObject.GetComponents<Collider>();
            gameRenderer = this.gameObject.GetComponents<Renderer>();
            canvas = this.GetComponent<Canvas>();

            if (gameRenderer.Length == 0)
                gameRenderer = this.gameObject.GetComponentsInChildren<Renderer>();
            if (gameRenderer.Length == 0)
            {
                Debug.LogError(
                    $"{gameObject.name} has not renderer component. This is important for insideCamera property.");
            }

            if (gameCollider.Length == 0)
                gameCollider = this.gameObject.GetComponentsInChildren<Collider>();

            p = new Position(transform.position);
            r = new Rotation(transform.localRotation);
            s = new Scale(transform.localScale);

            // I don't want to know what should be done if there are multiple cameras in the scene
            xrCamera = Camera.main;
            if (xrCamera == null)
            {
                throw new Exception(
                    "No camera found in the scene. This is important for the isInsideCamera property!!");
            }
        }

        private void Start()
        {
            _originalPosition = gameObject.transform.position;
            _originalQuaternion = gameObject.transform.localRotation;
            _originalScale = gameObject.transform.localScale;
            UpdateVisibility();
        }

        /// <summary>
        /// <b>Moves</b> (to) is a method that moves the object to a specified position in the 3D space.
        /// </summary>
        /// <param name="newPos">The target position to move to.</param>
        [Action(typeof(ECAObject), "moves to", typeof(Position))]
        public void Moves(Position newPos)
        {
            float speed = 1.0F;
            Vector3 endMarker = new Vector3(newPos.x, newPos.y, newPos.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Moves</b> (on) is a method that moves the object along a specified path, following sequential points.
        /// </summary>
        /// <param name="path">An array of positions the object will follow, one after another.</param>
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
        /// <b>Rotates</b> sets the object's rotation to a specified value in the 3D space.
        /// </summary>
        /// <param name="newRot">The target rotation expressed as a vector with three components: x, y, and z.</param>
        [Action(typeof(ECAObject), "rotates around", typeof(Rotation))]
        public void Rotates(Rotation newRot)
        {
            //r.Assign(newRot);
            r = new Rotation(newRot);
            transform.Rotate(r.x, r.y, r.z); // todo verify rotation
        }

        /// <summary>
        /// <b>Looks</b> adjusts the object's rotation to face a specified target object.
        /// </summary>
        /// <param name="o">The target GameObject to look at.</param>
        [Action(typeof(ECAObject), "looks at", typeof(GameObject))]
        public void Looks(GameObject o)
        {
            ECAObject looked = o.GetComponent<ECAObject>();
            Physics.Linecast(transform.position, o.transform.position, out var hit);
            if (o.name == hit.collider.gameObject.name && looked.isActive)
            {
                transform.LookAt(looked.gameObject.transform);
                //r.Assign(transform.rotation);
                r = new Rotation(transform.rotation);
            }
        }

        /// <summary>
        /// <b>Scales</b> sets the object's scale to a specified value.
        /// </summary>
        /// <param name="newScale">The new scale value fo the object. The scale is a vector with three components: x, y, and z.</param>
        [Action(typeof(ECAObject), "scales to", typeof(Scale))]
        public void Scales(Scale newScale)
        {
            //r.Assign(newRot);
            s = new Scale(newScale);
            transform.localScale = new Vector3(s.x, s.y, s.z);
        }

        /// <summary>
        /// Restores the object's original position, rotation, and scale to their initial values.
        /// </summary>
        [Action(typeof(ECAObject), "restores original settings")]
        public void MovesOriginalPosition()
        {
            // float speed = 3.0F;
            //StartCoroutine(MoveObject(speed, _originalPosition));
            gameObject.transform.position = _originalPosition;
            p = new Position(_originalPosition);
            gameObject.transform.localRotation = _originalQuaternion;
            r = new Rotation(_originalQuaternion);
            gameObject.transform.localScale = _originalScale;
            s = new Scale(_originalScale);
        }

        /// <summary>
        /// <b>Shows</b> maakes the object visible if it is not already.
        /// </summary>
        [Action(typeof(ECAObject), "shows")]
        [ECARelevance(true)]
        public void Shows()
        {
            //isVisible.Assign(ECABoolean.BoolType.YES);
            isVisible = new ECABoolean(ECABoolean.BoolType.YES);
            UpdateVisibility();
        }

        /// <summary>
        /// <b>Hides</b> makes the object invisible if it is not already.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAObject), "hides")]
        public void Hides()
        {
            //isVisible.Assign(ECABoolean.BoolType.NO);
            isVisible = new ECABoolean(ECABoolean.BoolType.NO);
            UpdateVisibility();
        }

        /// <summary>
        /// <b>Activates</b> makes the object both interactable and visible.
        /// </summary>
        [Action(typeof(ECAObject), "activates")]
        public void Activates()
        {
            //isActive.Assign(ECABoolean.BoolType.YES);
            isActive = new ECABoolean(ECABoolean.BoolType.YES);
            UpdateVisibility();
        }

        /// <summary>
        /// <b>Deactivates</b> makes the object invisible and non-interactable.
        /// </summary>
        [Action(typeof(ECAObject), "deactivates")]
        public void Deactivates()
        {
            //isActive.Assign(ECABoolean.BoolType.NO);
            isActive = new ECABoolean(ECABoolean.BoolType.NO);
            UpdateVisibility();
        }

        /// <summary>
        /// <b>ShowsHides</b> changes the visibility state of the object based on a parameter. The parameter can be either "yes" or "no".
        /// </summary>
        /// <param name="yesNo">The new visibility state.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAObject), "changes", "visible", "to", typeof(YesNo))]
        public void ShowsHides(ECABoolean yesNo)
        {
            isVisible = yesNo;
            UpdateVisibility();
        }

        /// <summary>
        /// <b>ActivatesDeactivates</b> changes the active state of the object based on a parameter. The parameter can be either "yes" or "no".
        /// </summary>
        /// <param name="yesNo">The new active state.</param>
        [Action(typeof(ECAObject), "changes", "active", "to", typeof(YesNo))]
        public void ActivatesDeactivates(ECABoolean yesNo)
        {
            isActive = yesNo;
            UpdateVisibility();
        }

        //TODO: should be private
        public void UpdateVisibility()
        {
            foreach (Collider c in gameCollider)
            {
                c.enabled = isActive;
            }

            foreach (Renderer rend in gameRenderer)
            {
                if (!isActive || !isVisible)
                {
                    rend.enabled = false;
                }
                else rend.enabled = true;
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
            Vector3 startMarker = gameObject.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startMarker, endMarker);
            //while (gameObject.transform.position != endMarker)
            while (gameObject.transform.position != endMarker)
            {
                float distCovered = (Time.time - startTime) * speed;

                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.

                gameObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                //GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
                GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
                yield return null;
            }

            //GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
            GetComponent<ECAObject>().p = new Position(gameObject.transform.position);
            isBusyMoving = false;
        }

        private void Update()
        {
            // check if object is within the camera's field of view.
            // the object must be active
            if (xrCamera && Time.time - deltaTimeIsRendered > 0.5f)
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

                if (this.isInsideCamera != res)
                {
                    this.isInsideCamera = res;
                    deltaTimeIsRendered = Time.time;
                }
            }
        }
    }
}