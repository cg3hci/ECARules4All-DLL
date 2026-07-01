using System;
using System.Collections;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("carillon")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECASound))]
    public class ECACarillon : ECAProp
    {
        private ECASound _sound;

        [Header("Visual Effects Components")]
        [SerializeField] private ParticleSystem _notesParticles;
        [SerializeField] private Transform _crankTransform;
        [SerializeField] private Transform _discTransform;
        
        [Header("Settings")]
        [SerializeField] private float _crankSpeed = 150f;
        [SerializeField] private float _discSpeed = 45f;

        private void Awake()
        {
            _sound = GetComponent<ECASound>();
        }

        private void Start()
        {
            StartCoroutine(InitVisualState());
        }

        private IEnumerator InitVisualState()
        {
            yield return null;
            UpdateVisualEffects();
        }
        private void Update()
        {
            if (_isPlaying == ECABoolean.TRUE)
            {
                if (_crankTransform != null)
                    _crankTransform.Rotate(Vector3.right * _crankSpeed * Time.deltaTime, Space.Self);

                if (_discTransform != null)
                    _discTransform.Rotate(Vector3.up * _discSpeed * Time.deltaTime, Space.Self);
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
                UpdateVisualEffects();
            }
        }
        [SerializeField] private ECABoolean _isPlaying = ECABoolean.FALSE;

        /// <summary>
        /// Gestisce l'allineamento tra stato logico e componenti grafici/audio
        /// </summary>
        private void UpdateVisualEffects()
        {
            bool shouldPlay = (_isPlaying == ECABoolean.TRUE);

            if (_notesParticles != null)
            {
                if (shouldPlay && !_notesParticles.isPlaying) _notesParticles.Play();
                else if (!shouldPlay && _notesParticles.isPlaying) _notesParticles.Stop();
            }

            if (_sound != null)
            {
                if (shouldPlay) _sound.Plays();
                else _sound.Stops();
            }
        }
        
        /// <summary>
        /// starts play music from the carillon
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECACarillon), "play")]
        [ContextMenu("play")]
        public void Play()
        {
            isPlaying = ECABoolean.TRUE;
        }
        
        /// <summary>
        /// stops the music from the carillon
        /// </summary>
        [Action(typeof(ECACarillon), "stop")]
        [ECARelevance(true)]
        [ContextMenu("stop")]
        public void Stop()
        {
            isPlaying = ECABoolean.FALSE;
        }
    }
}