using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using Serilog;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <b>Sound</b> is a behavior that functions as a media player specifically designed for audio files.
    /// It extends <see cref="ECABehaviour"/> to provide audio-related functionalities such as playback, volume control, and source management.
    [ECARules4All("sound")]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(AudioSource))]
    [JsonObject(MemberSerialization.OptIn)]
    [DisallowMultipleComponent]
    public class Sound : MonoBehaviour
    {
        private string status = "";

        /// <summary>
        /// <b>Source</b> is the audio filename that serves as the source for playback.
        /// </summary>
        [StateVariable("source", ECARules4AllType.Text)]
        [ECARelevance(true)]
        public string source
        {
            get => _source;
            set
            {
                _source = value;
                ECAScript.NotifyUpdate(this, nameof(source), source);
            }
        }
        [SerializeField] private string _source;

        /// <summary>
        /// <b>Volume</b> is the current volume level of the audio.
        /// Accepts values between 0 and the maximum volume, defined by <see cref="maxVolume"/>.
        /// </summary>
        [StateVariable("volume", ECARules4AllType.Float)]
        [ECARelevance(true)]
        public float volume
        {
            get => _volume;
            set
            {
                _volume = value;
                ECAScript.NotifyUpdate(this, nameof(volume), volume.ToString());
            }
        }
        [SerializeField] private float _volume;

        /// <summary>
        /// <b>MaxVolume</b> is the maximum volume level the audio can reach.
        /// </summary>
        [StateVariable("maxVolume", ECARules4AllType.Float)]
        public float maxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value; //TODO What if the volume is greater than the maxVolume?
                ECAScript.NotifyUpdate(this, nameof(maxVolume), maxVolume.ToString());
            }
        }
        [SerializeField] private float _maxVolume;

        /// <summary>
        /// <b>currentTime</b> is the current playback position in seconds. 
        /// Tracks the progression of the audio clip.
        /// </summary>
        [StateVariable("currentTime", ECARules4AllType.Float)]
        [ECARelevance(false)]
        public float currentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                ECAScript.NotifyUpdate(this, nameof(currentTime), currentTime.ToString());
            }
        }
        [SerializeField] private float _currentTime;

        /// <summary>
        /// <b>playing</b> indicates whether the audio is currently playing. The value is either "yes" or "no". If paused or stopped are "yes", playing will be "no".
        /// </summary>
        [StateVariable("playing", ECARules4AllType.Boolean)]
        [ECARelevance(true)]
        public ECABoolean playing
        {
            get => _playing;
            set
            {
                _playing = value;
                ECAScript.NotifyUpdate(this, nameof(playing), playing.ToString());
            }
        }
        [SerializeField] private ECABoolean _playing = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b>paused</b> indicates whether the audio playback is paused. The value is either "yes" or "no". When playing again, the audio will resume from the paused time.
        /// </summary>
        [StateVariable("paused", ECARules4AllType.Boolean)]
        [ECARelevance(false)]
        public ECABoolean paused
        {
            get => _paused;
            set
            {
                _paused = value;
                ECAScript.NotifyUpdate(this, nameof(paused), paused.ToString());
            }
        }
        [SerializeField] private ECABoolean _paused = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b> Stopped </b> indicates whether the audio playback is stopped. The value is either "yes" or "no". When playing again, the audio will start from the beginning.
        /// </summary>
        [StateVariable("stopped", ECARules4AllType.Boolean)]
        [ECARelevance(true)]
        public ECABoolean stopped
        {
            get => _stopped;
            set
            {
                _stopped = value;
                ECAScript.NotifyUpdate(this, nameof(stopped), stopped.ToString());
            }
        }
        [SerializeField] private ECABoolean _stopped = new ECABoolean(ECABoolean.BoolType.YES);

        /// <summary>
        /// <b>AudioSource</b> is the audio _audioSource that will be used to play the audio.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// <b> SourcePath </b> is the path of the audio file.
        /// </summary>
        private string sourcePath;

        /// <summary>
        /// <b>Plays</b> starts the audio playback.
        /// Updates the state variables playing, stopped, and paused to reflect that playback is active.
        /// </summary>
        [Action(typeof(Sound), "plays")]
        [ECARelevance(true)]
        public void Plays()
        {
            this.playing= new ECABoolean(ECABoolean.BoolType.YES);
            this.stopped = new ECABoolean(ECABoolean.BoolType.YES);
            this.paused = new ECABoolean(ECABoolean.BoolType.YES);
            _audioSource.Play();
        }

        /// <summary>
        /// <b>Pauses</b> pauses the audio playback.
        /// Maintains the current playback time (currentTime) for resuming later (by calling Plays).
        /// </summary>
        [Action(typeof(Sound), "pauses")]
        [ECARelevance(false)]
        void Pauses()
        {
            this.playing= new ECABoolean(ECABoolean.BoolType.NO);
            this.stopped= new ECABoolean(ECABoolean.BoolType.NO);
            this.paused= new ECABoolean(ECABoolean.BoolType.YES);
            _audioSource.Pause();
        }

        /// <summary>
        /// <b>Stops</b> stops the audio playback and resets the playback time (currentTime) to the beginning.
        /// </summary>
        [Action(typeof(Sound), "stops")]
        [ECARelevance(true)]
        public void Stops()
        {
            this.playing= new ECABoolean(ECABoolean.BoolType.NO);
            this.stopped= new ECABoolean(ECABoolean.BoolType.YES);
            this.paused= new ECABoolean(ECABoolean.BoolType.NO);
            _audioSource.Stop();
        }

        /// <summary>
        /// <b>ChangesVolume</b> changes the volume of the audio to a given value.
        /// <para>Ensures the value remains within the range of 0 to <see cref="maxVolume"/></para>
        /// </summary>
        /// <param name="v">The new volume value.</param>
        [Action(typeof(Sound), "changes", "volume", "to", typeof(float))]
        [ECARelevance(true)]
        public void ChangesVolume(float v)
        {
            if (v > maxVolume)
            {
                v = maxVolume;
            }

            if (v < 0)
            {
                v = 0;
            }

            this.volume = v;
            _audioSource.volume = this.volume;
        }

        /// <summary>
        /// <b>ChangesSource</b> changes the audio filename source to the given filename.
        /// <p>Validates the path and dynamically loads the audio for playback.</p>
        /// </summary>
        /// <param name="newSource">The new audio filename.</param>
        [Action(typeof(Sound), "changes", "source", "to", typeof(string))]
        [ECARelevance(true)]
        public void ChangesSource(string newSource)
        {
            var objName = gameObject.name;

            if (string.IsNullOrEmpty(newSource))
                Log.Warning($"Hai inserito un url vuoto per l'audio in {objName}!");

            StartCoroutine(this.LoadAudioFromURL(newSource, (status) =>
            {
                Log.Information($"PlayAudio sull'oggetto {objName} terminata con stato: {status}");

                if (status != "ok")
                    throw new Exception(
                        $"Si è verificato un errore nel caricamento dell'audio {newSource} per l'oggetto {objName}");

                // Update the list of objects in the scene
                // instance?.GetObjectInfoByName(objName);
            }));
        }

        public IEnumerator LoadAudioFromURL(string fileName, Action<string> result)
        {
            _audioSource.Stop(); // Interrompe la riproduzione dell'audio corrente

            using (UnityWebRequest uwr =
                   UnityWebRequestMultimedia.GetAudioClip(TaxonomyUtils.getFileAudioByName(fileName),
                       AudioType.UNKNOWN))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Log.Error(
                        "Si è verificato un errore nel caricamento del file audio. Verifica che il file audio " +
                        "richiesto esista e si trovi nella relativa cartella.");
                    status = "error";
                }
                else
                {
                    source = fileName;
                    _audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                    _audioSource.time = 0;
                    if (!stopped && !paused && playing)
                    {
                        /* TODO Questo serve perché ri-mettere in play l'eventuale clip,
                        // essendo questa funzione (ChangeSource) invocata in maniera asincrona (IEnumerator),
                        // non siamo certi che venga effettivamente eseguita prima della Play()
                        // Esecuzione Ideale: ChangeSource ->  Play
                        // Problema: ChangeSource è asincrono e potrebbe succedere questo
                        //                    Play -> ChangeSource
                        // Ma visto che il cambio di Clip (in ChangeSource) stoppa la riproduzione audio, la clip in sostanza non va mai in play */
                        _audioSource.Play(); // Avvia la riproduzione del nuovo file audio
                    }

                    status = "ok";
                }

                result(status);
            }

            yield return null;
        }

        private void Start()
        {
            // _hook = GameObject.Find("Hook").GetComponent<HookManager>(); // TODO JACO AGILE: Perchè prendiamo l'hook?
            maxVolume = 1.0f;
            _audioSource = GetComponent<AudioSource>();

            ChangesSource(source);

            volume = volume > maxVolume ? maxVolume : volume;
            volume = volume < 0.0f ? 0.0f : volume;
            _audioSource.volume = volume;

            _audioSource.playOnAwake = (!stopped && !paused && playing);

            if (stopped || paused)
                this.Stops();
            else
                this.Plays();
        }
    }
}