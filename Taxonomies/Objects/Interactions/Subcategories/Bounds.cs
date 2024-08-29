using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories
{
    /// <summary>
    /// <b>Bounds</b> is an <see cref="Interaction"/> subclass that represents the scene bounds.
    /// </summary>
    [ECARules4All("bounds")]
    [RequireComponent(typeof(Interaction))] //gerarchia 
    [DisallowMultipleComponent]
    public class Bounds : ECAScript
    {
        /// <summary>
        /// <b>Scale</b> is the scale of the scene bounds.
        /// </summary>
        [StateVariable("scale", ECARules4AllType.Float)]
        public float scale
        {
            get => _scale;
            set
            {
                _scale = value;
                NotifyUpdate(nameof(scale), scale.ToString());
            }
        }
        [SerializeField]
        private float _scale;

        /// <summary>
        /// <b>Scales</b> sets the scale of the scene bounds.
        /// </summary>
        /// <param name="newScale">The new scale value.</param>
        [Action(typeof(Bounds), "scales-to", typeof(float))]
        public void Scales(float newScale)
        {
            scale = newScale;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void Start()
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
