using System.Collections;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("candle")]
    [DisallowMultipleComponent]
    public class ECACandle : ECAProp
    {
        [SerializeField]
        private ECABoolean _isLit = ECABoolean.FALSE;

        [SerializeField] 
        private float _fuelLevel = 5.0f;

        public float burnRate = 2.0f;
        
        private Coroutine _burnCoroutine;

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

                ManageBurnRoutine();
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
            if (fuelLevel > 0)
                isLit = ECABoolean.TRUE;
            else
            {
                Debug.LogError($"[{gameObject.name}] Impossibile accendere la candela: la cera è stata tutta consumata");
            }
        }
        
        [ECARelevance(true)]
        [Action(typeof(ECACandle), "blows out")]
        [ContextMenu("Blows Out")]
        public void BlowsOut()
        {
            isLit =ECABoolean.FALSE; 
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

        private void ManageBurnRoutine()
        {
            bool shouldBeOn = isLit == ECABoolean.TRUE;

            if (shouldBeOn && fuelLevel > 0)
            {
                if (_burnCoroutine == null)
                    _burnCoroutine = StartCoroutine(BurnRoutine());
            }
            else
            {
                if (_burnCoroutine != null)
                {
                    StopCoroutine(_burnCoroutine);
                    _burnCoroutine = null;
                }
            }
        }

        private IEnumerator BurnRoutine()
        {
            while (isLit == ECABoolean.TRUE && fuelLevel > 0)
            {
                yield return new WaitForSeconds(2f);
                
                fuelLevel -= burnRate;

                if (fuelLevel <= 0)
                {
                    Debug.LogError($"[{gameObject.name}] Cera esaurita!");
                    fuelLevel = 0;
                    BlowsOut();
                }
            }
        }
    }
}