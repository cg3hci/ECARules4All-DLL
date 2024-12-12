using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories
{
    /// <summary>
    /// <b>ECALight</b> represents a controllable light source in the environment.
    /// The ECALight class extends <see cref="Interaction"/> to manage light properties such as intensity, color, and if it's on.
    /// </summary>
    [ECARules4All("light")]
    [RequireComponent(typeof(Interaction), typeof(Light))] //gerarchia 
    [DisallowMultipleComponent]
    public class ECALight : MonoBehaviour
    {
        /// <summary>
        /// <b>intensity</b> represents the brightness level of the light source. It cannot exceed the maximum intensity value.
        /// </summary>
        [StateVariable("intensity", ECARules4AllType.Float)]
        public float intensity
        {
            get => _intensity;
            set
            {
                _intensity = value;
                ECAScript.NotifyUpdate(this, nameof(intensity), intensity.ToString());
            }
        }
        [SerializeField]
        private float _intensity = 1;

        /// <summary>
        /// <b>maxIntensity</b> specifies the upper limit for the light's brightness. It ensures that the light's intensity does not exceed a predefined threshold.
        /// </summary>
        [StateVariable("maxIntensity", ECARules4AllType.Float)]
        public float maxIntensity
        {
            get => _maxIntensity;
            set
            {
                _maxIntensity = value;
                ECAScript.NotifyUpdate(this, nameof(maxIntensity), maxIntensity.ToString());
            }
        }
        [SerializeField]
        private float _maxIntensity = 10;

        /// <summary>
        /// <b>color</b> represents the color of the light source. The value is a string that represents the color name (e.g., "red", "blue", "green").
        /// </summary>
        [StateVariable("color", ECARules4AllType.Color)]
        public Color color
        {
            get => _color;
            set
            {
                _color = value;
                ECAScript.NotifyUpdate(this, nameof(color), color.ToString());
            }
        }
        [SerializeField]
        private Color _color = new Color(1, 0.95686271f, 0.8392157f, 1);

        /// <summary>
        /// <b>on</b> indicates whether the light source is currently active or inactive. The accepted values are "on" or "off".
        /// </summary>
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

        /// <summary>
        /// <b>ComponentLight</b> is the GameObject's light component.
        /// </summary>
        private Light componentLight;

        /// <summary>
        ///  <b>Turns</b> toggles the light source on or off based on the specified value ("on" or "off"), enabling or disabling illumination.
        /// </summary>
        /// <param name="newStatus">The desired state of the light source (on or off).</param>
        [Action(typeof(ECALight), "turns", typeof(ECABoolean))]
        public void Turns(ECABoolean newStatus)
        {
            on = newStatus;
            componentLight.enabled = on;
        }

        /// <summary>
        /// <b>IncreasesIntensity</b> increases the brightness of the light source by a specified non-negative amount.
        /// If the resulting intensity exceeds the maximum allowed value, it is capped at <b>maxIntensity</b>.
        /// </summary>
        /// <param name="amount">The value to add to the current intensity.</param>
        [Action(typeof(ECALight), "increases", "intensity", "by", typeof(float))]
        public void IncreasesIntensity(float amount)
        {
            if (intensity + amount < maxIntensity)
            {
                intensity += amount;
                GetComponent<Light>().intensity = intensity;
            }
            else
            {
                intensity = maxIntensity;
                GetComponent<Light>().intensity = intensity;
            }
        }

        /// <summary>
        /// <b> DecreasesIntensity</b> reduces the brightness of the light source by a specified non-negative amount.
        /// If the resulting intensity drops below zero, it is set to zero to avoid negative values.
        /// </summary>
        /// <param name="amount">The value to subtract from the current intensity.</param>
        [Action(typeof(ECALight), "decreases", "intensity", "by", typeof(float))]
        public void DecreasesIntensity(float amount)
        {
            if (intensity - amount > 0)
            {
                intensity -= amount;
                GetComponent<Light>().intensity = intensity;
            }
            else
            {
                intensity = 0;
                GetComponent<Light>().intensity = intensity;
            }
        }

        /// <summary>
        /// <b>SetsIntensity</b> sets the brightness of the light source to a specific value.
        /// If the specified intensity exceeds <b>maxIntensity</b>, it is limited to the maximum allowed value.
        /// </summary>
        /// <param name="i">The new intensity value to assign to the light source.param>
        [Action(typeof(ECALight), "sets", "intensity", "to", typeof(float))]
        public void SetsIntensity(float i)
        {
            if (i >= maxIntensity)
            {
                GetComponent<Light>().intensity = maxIntensity;
            }
            else
            {
                GetComponent<Light>().intensity = i;
            }
        }

        /// <summary>
        /// <b>SetsColor</b> updates the light's color to the specified value. The allowed values are predefined color names (e.g., "red", "blue", "green").
        /// </summary>
        /// <param name="inputColor">The desired color to apply to the light source.</param>
        [Action(typeof(ECALight), "changes", "color", "to", typeof(ECAColor))]
        public void ChangesColor(ECAColor inputColor)
        {
            color = inputColor.Color;
            componentLight.color = color;
        }

        public void Awake()
        {
            componentLight = GetComponent<Light>();
            componentLight.color = color;
            intensity = intensity > maxIntensity ? maxIntensity : intensity;
            componentLight.intensity = intensity;
            componentLight.enabled = on;
        }
    }
}
