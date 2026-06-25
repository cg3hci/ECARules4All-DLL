using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("chandelier")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECAInteractable))]
    public class ECAChandelier : ECAProp
    {
        [SerializeField]
        [Range(0, 100)]
        private int _brightness = 100;

        [Header("Impostazioni Luce")]
        [Tooltip("L'intensità massima delle Point Light in Unity quando brightness è a 100")]
        public float maxLightIntensity = 25f;

        [Header("Componenti Multipli (Opzionale)")]
        [Tooltip("Se lasciati vuoti, lo script li cercherà automaticamente in tutti gli oggetti figli")]
        public List<Light> candleLights = new List<Light>();
        
        public List<ParticleSystem> candleFlames = new List<ParticleSystem>();

        [ECARelevance(true)]
        [StateVariable("brightness", ECARules4AllType.Integer)] 
        public int brightness
        {
            get => _brightness;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, 100);
                if (_brightness == clampedValue) return;

                _brightness = clampedValue;
                ECAScript.NotifyUpdate(this, nameof(brightness), brightness.ToString());

                UpdateVisualEffects();
            }
        }

        protected void Awake()
        {
            if (candleLights.Count == 0)
            {
                candleLights = new List<Light>(GetComponentsInChildren<Light>());
            }

            if (candleFlames.Count == 0)
            {
                candleFlames = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
            }

            UpdateVisualEffects();
        }

        [ECARelevance(true)]
        [Action(typeof(ECAChandelier), "set brightness")]
        public void SetBrightness(int level)
        {
            brightness = level;
        }

        [ECARelevance(true)]
        [Action(typeof(ECAChandelier), "set brightness 100")]
        [ContextMenu("Set Brightness 100")]
        public void setBrightness_100()
        {
            SetBrightness(100);
            Debug.Log($"[ECAChandelier]: Brightness {brightness}");
        }
        
        [ECARelevance(true)]
        [Action(typeof(ECAChandelier), "set brightness 0")]
        [ContextMenu("Set Brightness 0")]
        public void setBrightness_0()
        {
            SetBrightness(0);
            Debug.Log($"[ECAChandelier]: Brightness {brightness}");
        }
        
        [ECARelevance(true)]
        [Action(typeof(ECAChandelier), "invert state")]
        [ContextMenu("Invert State")]
        public void invertState()
        {
            int level = brightness > 0 ? 0 : 100;
            SetBrightness(level);
            Debug.Log($"[ECAChandelier]: Brightness {brightness}");
        }

        private void UpdateVisualEffects()
        {
            float normalizedBrightness = _brightness / 100f;
            float actualIntensity = normalizedBrightness * maxLightIntensity;
            
            // Se la luminosità è maggiore di zero, il lampadario è considerato "acceso"
            bool shouldBeOn = _brightness > 0;

            // Aggiorna tutte le luci della corona
            foreach (var light in candleLights)
            {
                if (light != null)
                {
                    light.enabled = shouldBeOn;
                    light.intensity = actualIntensity;
                }
            }

            // Aggiorna tutte le particelle
            foreach (var flame in candleFlames)
            {
                if (flame != null)
                {
                    if (shouldBeOn && !flame.isPlaying) 
                    {
                        flame.Play();
                    }
                    else if (!shouldBeOn && flame.isPlaying)
                    {
                        flame.Stop();
                        flame.Clear();
                    }
                }
            }
        }
    }
}