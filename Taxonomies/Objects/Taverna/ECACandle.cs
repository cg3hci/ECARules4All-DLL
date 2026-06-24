using System;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("candle")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECAInteractable))]
    public class ECACandle : ECAProp
    {
        [SerializeField]
        private ECABoolean _isLit = new ECABoolean(ECABoolean.BoolType.FALSE);

        [SerializeField] 
        private float _fuelLevel = 100.0f;

        [ECARelevance(true)]
        [StateVariable("isLit", ECARules4AllType.Boolean)]
        public ECABoolean isLit
        {
            get => _isLit;
            set
            {
                _isLit = value; 
                ECAScript.NotifyUpdate(this, nameof(isLit), isLit.ToString());
                
                UpdateVisualEffects();
            }
        }
        
        [ECARelevance(true)]
        [StateVariable("fuelLevel", ECARules4AllType.Float)]
        public float fuelLevel
        {
            get => _fuelLevel;
            set
            {
                _fuelLevel = value; 
                ECAScript.NotifyUpdate(this, nameof(fuelLevel), fuelLevel.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }
        
        [Header("Visual Effects")]
        public ParticleSystem fireParticle;
        
        public Light candleLight;

        protected void Awake()
        {
            if (candleLight == null)
            {
                candleLight = GetComponentInChildren<Light>();
            }

            UpdateVisualEffects();
        }

        [ECARelevance(true)]
        [Action(typeof(ECACandle), "lights up")]
        [ContextMenu("Lights Up")]
        public void LightUp()
        {
            isLit = new ECABoolean(ECABoolean.BoolType.TRUE);
        }
        
        [ECARelevance(true)]
        [Action(typeof(ECACandle), "blows out")]
        [ContextMenu("Blows Out")]
        public void BlowsOut()
        {
            isLit = new ECABoolean(ECABoolean.BoolType.FALSE); 
        }

        /// <summary>
        /// Update visual components in Unity (Lights and ps) from the value of the variable isLit
        /// </summary>
        private void UpdateVisualEffects()
        {
            bool shouldBeOn = isLit == ECABoolean.TRUE;

            if (candleLight != null)
            {
                candleLight.enabled = shouldBeOn;
            }

            if (fireParticle != null)
            {
                if (shouldBeOn && !fireParticle.isPlaying)
                {
                    fireParticle.Play();
                }
                else if (!shouldBeOn && fireParticle.isPlaying)
                {
                    fireParticle.Stop();
                }
            }
        }
    }
}