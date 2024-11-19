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
    /// <summary>
    /// <b>ECASound</b> is a <see cref="ECABehaviour"/> that works like a media player, and it is specific to audio files.
    /// </summary>
    [ECARules4All("sound")]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(AudioSource))]
    [JsonObject(MemberSerialization.OptIn)]
    [DisallowMultipleComponent]
    public class Sound : ECAScript
    {
        private string status = "";
        
        /// <summary>
        /// <b>Source</b> is the audio source that will be used to play the audio.
        /// </summary>
        [StateVariable("source", ECARules4AllType.Text)]
        public string source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyUpdate(nameof(source), source);
            }
        }
        [SerializeField] private string _source;

        /// <summary>
        /// <b>Volume</b> is the volume of the audio.
        /// </summary>
        [StateVariable("volume", ECARules4AllType.Float)]
        public float volume
        {
            get => _volume;
            set
            {
                _volume = value;
                NotifyUpdate(nameof(volume), volume.ToString());
            }
        }

        [SerializeField] private float _volume;

        /// <summary>
        /// <b>MaxVolume</b> is the maximum volume the audio can reach.
        /// </summary>
        [StateVariable("maxVolume", ECARules4AllType.Float)]
        public float maxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value;
                NotifyUpdate(nameof(maxVolume), maxVolume.ToString());
            }
        }

        [SerializeField] private float _maxVolume;

        /// <summary>
        /// <b>duration</b> is the duration of the audio.
        /// </summary>
        [StateVariable("duration", ECARules4AllType.Float)]
        public float duration
        {
            get => _duration;
            set
            {
                _duration = value;
                NotifyUpdate(nameof(duration), duration.ToString());
            }
        }

        [SerializeField] private float _duration;

        /// <summary>
        /// <b>currentTime</b> is the current time of the audio.
        /// </summary>
        [StateVariable("currentTime", ECARules4AllType.Float)]
        public float currentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                NotifyUpdate(nameof(currentTime), currentTime.ToString());
            }
        }

        [SerializeField] private float _currentTime;

        /// <summary>
        /// <b>isPlaying</b> is a boolean that indicates if the audio is playing.
        /// </summary>
        [StateVariable("playing", ECARules4AllType.Boolean)]
        public ECABoolean playing
        {
            get => _playing;
            set
            {
                _playing = value;
                NotifyUpdate(nameof(playing), playing.ToString());
            }
        }

        [SerializeField] private ECABoolean _playing = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b> Paused </b> is a boolean that indicates if the audio is paused.
        /// </summary>
        [StateVariable("paused", ECARules4AllType.Boolean)]
        public ECABoolean paused
        {
            get => _paused;
            set
            {
                _paused = value;
                NotifyUpdate(nameof(paused), paused.ToString());
            }
        }

        [SerializeField] private ECABoolean _paused = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b> Stopped </b> is a boolean that indicates if the audio is stopped.
        /// </summary>
        [StateVariable("stopped", ECARules4AllType.Boolean)]
        public ECABoolean stopped
        {
            get => _stopped;
            set
            {
                _stopped = value;
                NotifyUpdate(nameof(stopped), stopped.ToString());
            }
        }

        [SerializeField] private ECABoolean _stopped = new ECABoolean(ECABoolean.BoolType.YES);
        
        /// <summary>
        /// <b> AudioSource </b> is the audio _audioSource that will be used to play the audio.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// <b> SourcePath </b> is the path of the audio file.
        /// </summary>
        private string sourcePath;

        /// <summary>
        /// <b>Plays</b> starts the audio.
        /// </summary>
        [Action(typeof(Sound), "plays")]
        public void Plays()
        {
            this.playing.Assign(ECABoolean.BoolType.YES);
            this.stopped.Assign(ECABoolean.BoolType.NO);
            this.paused.Assign(ECABoolean.BoolType.NO);
            _audioSource.Play();
        }

        /// <summary>
        /// <b>Pauses</b> pauses the audio.
        /// </summary>
        [Action(typeof(Sound), "pauses")]
        public void Pauses()
        {
            this.playing.Assign(ECABoolean.BoolType.NO);
            this.stopped.Assign(ECABoolean.BoolType.NO);
            this.paused.Assign(ECABoolean.BoolType.YES);
            _audioSource.Pause();
        }

        /// <summary>
        /// <b>Stops</b> stops the audio.
        /// </summary>
        [Action(typeof(Sound), "stops")]
        public void Stops()
        {
            this.playing.Assign(ECABoolean.BoolType.NO);
            this.stopped.Assign(ECABoolean.BoolType.YES);
            this.paused.Assign(ECABoolean.BoolType.NO);
            _audioSource.Stop();
        }

        /// <summary>
        /// <b>ChangesVolume</b> changes the volume of the audio to the given value.
        /// <para>If the value is greater than the maximum volume, the volume will be set to the maximum volume; if
        /// the value is less than 0, the volume will be set to 0.</para>
        /// </summary>
        /// <param name="v">The new volume setting.</param>
        [Action(typeof(Sound), "changes", "volume", "to", typeof(float))]
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
        /// <b>ChangesSource</b> changes the source of the audio to the given path.
        /// <p>The path must be relative to the project's user-accessible Inventory folder.</p>
        /// <p>If the path is not valid, the audio will not be played.</p>
        /// </summary>
        /// <param name="newSource">The new audio file path.</param>
        [Action(typeof(Sound), "changes", "source", "to", typeof(string))]
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
                   UnityWebRequestMultimedia.GetAudioClip(TaxonomyUtils.getFileAudioByName(fileName), AudioType.UNKNOWN))
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