using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("fireplace")]
    [RequireComponent(typeof(ECAInteraction))]
    [DisallowMultipleComponent]
    public class ECAFireplace : ECAObject
    {
        [Header("Fireplace Properties (HASS)")]
        [ECARelevance(true)]
        [StateVariable("isLit", ECARules4AllType.Boolean)]
        public ECABoolean isLit
        {
            get => _isLit;
            set
            {
                _isLit = value;
                ECAScript.NotifyUpdate(this, nameof(isLit), isLit.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _isLit = new ECABoolean(ECABoolean.BoolType.OFF);

        [StateVariable("intensity", ECARules4AllType.Integer)]
        public int intensity
        {
            get => _intensity;
            set
            {
                _intensity = value;
                ECAScript.NotifyUpdate(this, nameof(intensity), intensity.ToString());
                UpdateVisualEffects();
                SyncWithLinkedLight();
            }
        }
        [SerializeField]
        private int _intensity = 0;

        [StateVariable("smokeLevel", ECARules4AllType.Float)]
        public float smokeLevel
        {
            get => _smokeLevel;
            set
            {
                _smokeLevel = value;
                ECAScript.NotifyUpdate(this, nameof(smokeLevel), smokeLevel.ToString());
            }
        }
        [SerializeField]
        private float _smokeLevel = 0f;

        [Header("Visual Effects Components")]
        public ParticleSystem smokeParticles;
        
        private ECALight linkedEcaLight;

        protected void Awake()
        {
            linkedEcaLight = GetComponentInChildren<ECALight>(); 
            UpdateVisualEffects();
        }

        /// Azione per accendere il camino (Imposta intensità a 20 di default)
        [ECARelevance(true)]
        [Action(typeof(ECAFireplace), "ignite")]
        [ContextMenu("Ignite")]
        public void Ignite()
        {
            isLit = new ECABoolean(ECABoolean.BoolType.ON);
            intensity = 20;
            smokeLevel = 33.3f;
        }

        /// Azione per spegnere completamente il camino
        [ECARelevance(true)]
        [Action(typeof(ECAFireplace), "extinguish")]
        [ContextMenu("Extinguish")]
        public void Extinguish()
        {
            isLit = new ECABoolean(ECABoolean.BoolType.OFF);
            intensity = 0;
            smokeLevel = 0f;
        }

        /// Azione per impostare l'intensità del camino
        [Action(typeof(ECAFireplace), "sets", "intensity", "to", typeof(int))]
        public void SetIntensity(int level)
        {
            isLit = new ECABoolean(level > 0 ? ECABoolean.BoolType.ON : ECABoolean.BoolType.OFF);
            intensity = level;
            smokeLevel = level * 33.3f;
        }
        
        private void UpdateVisualEffects()
        {
            if (smokeParticles == null) return;

            if (intensity > 0)
            {
                if (!smokeParticles.isPlaying) smokeParticles.Play();
                
                var emission = smokeParticles.emission;
                emission.rateOverTime = intensity * 2f;
            }
            else
            {
                if (smokeParticles.isPlaying) smokeParticles.Stop();
            }
        }

        private void SyncWithLinkedLight()
        {
            if (linkedEcaLight == null) return;

            linkedEcaLight.Turns(isLit);
            linkedEcaLight.SetsIntensity(intensity);
        }
    }
}