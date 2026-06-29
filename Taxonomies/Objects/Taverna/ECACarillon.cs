using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    /// <summary>
    /// ECACarillon represents a music box/gramophone. 
    /// It manages playback state, visual particles, and physical animations (crank and disc rotation).
    /// </summary>
    [ECARules4All("carillon")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECASound))]
    public class ECACarillon : ECAProp
    {
        private ECASound _sound;

        [SerializeField] private ParticleSystem _notesParticles;
        [SerializeField] private Transform _crankTransform; // La manovella
        [SerializeField] private Transform _discTransform;  // Il disco
        
        [SerializeField] private float _crankSpeed = 150f;
        
        [SerializeField] private float _discSpeed = 45f;

        private void Awake()
        {
            _sound = GetComponent<ECASound>();
        }

        private void Update()
        {
            // Se la musica sta suonando, facciamo girare i pezzi del grammofono!
            if (_isPlaying == ECABoolean.TRUE)
            {
                if (_crankTransform != null)
                {
                    // Ruota la manovella sul suo asse X locale
                    _crankTransform.Rotate(Vector3.right * _crankSpeed * Time.deltaTime, Space.Self);
                }

                if (_discTransform != null)
                {
                    // Ruota il disco sul suo asse Y locale
                    _discTransform.Rotate(Vector3.up * _discSpeed * Time.deltaTime, Space.Self);
                }
            }
        }

        [StateVariable("isPlaying", ECARules4AllType.Boolean)]
        [ECARelevance(true)]
        public ECABoolean isPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                ECAScript.NotifyUpdate(this, nameof(isPlaying), isPlaying.ToString());
            }
        }
        [SerializeField] private ECABoolean _isPlaying = ECABoolean.FALSE;

        [Action(typeof(ECACarillon), "Play")]
        [ECARelevance(true)]
        [ContextMenu("play")]
        public void Play()
        {
            isPlaying = ECABoolean.TRUE;
            if (_sound != null) _sound.Plays();
            
            if (_notesParticles != null && !_notesParticles.isPlaying)
            {
                _notesParticles.Play();
            }
        }

        [Action(typeof(ECACarillon), "Stop")]
        [ECARelevance(true)]
        [ContextMenu("stop")]
        public void Stop()
        {
            isPlaying = ECABoolean.FALSE;
            if (_sound != null) _sound.Stops();
            
            if (_notesParticles != null)
            {
                _notesParticles.Stop();
            }
        }
    }
}