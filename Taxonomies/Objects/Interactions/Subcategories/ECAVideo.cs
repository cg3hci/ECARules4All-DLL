using System.Linq;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Video;


namespace ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories
{
    /// <summary>
    /// <b>ECAVideo</b> is an <see cref="Interaction"/> that represents a video player.
    /// </summary>
    [ECARules4All("video")]
    [RequireComponent(typeof(Interaction))]
    [DisallowMultipleComponent]
    public class ECAVideo : MonoBehaviour
    {
        [SerializeField] private GameObject screen;
        
        private void Start()
        {
            this.TrySetCanvas("PlaneVideo");
            // SelectVideo(source); // Non deve essere fatto allo start perché altrimenti darebbe errore non appena viene aggiunto il componente

            volume = volume > maxVolume ? maxVolume : volume;
            volume = volume < 0.0f ? 0.0f : volume;
            ChangesVolume(volume);

            SelectVideo(source);

            if (stopped || paused)
                this.Stops();
            else
                this.Plays();
        }

        private void TrySetCanvas(string canvasName)
        {
            var checkCanvas = gameObject.transform.Find(canvasName);
            
            if (checkCanvas == null) // Se è null vuol dire che non esiste un figlio con quel nome
            { // Nota: Find("") restituisce il gameobject padre (quello a cui appartiene il transform)
                screen = gameObject;
            }
            else
            {
                screen = checkCanvas.gameObject;
            }
            
            _videoPlayer = screen.GetComponent<VideoPlayer>();
            
            if (_videoPlayer == null)
                _videoPlayer = screen.AddComponent<VideoPlayer>();

            _videoPlayer.isLooping = true;
        }
        
        public void SelectVideo(string videoName)
        {
            if (string.IsNullOrEmpty(videoName))
            {
                Debug.LogWarning("Hai inserito un url vuoto per il video!");
                return;
            }
            
            var oldName = _videoPlayer.url.Split('/').Last(); // Recupero il vecchio nome del file
            if (string.Equals(oldName, videoName)) 
                return; // Se il file non è cambiato esco dalla funzione
            
            if (_videoPlayer.isPlaying)
                Stops();
            
            _videoPlayer.url = TaxonomyUtils.getFileVideoByName(videoName);
            source = videoName;
        }
        
        #region ECA
        /// <summary>
        /// <b>Source</b> is the video source.
        /// </summary>
        [StateVariable("source", ECARules4AllType.Text)]
        [ECARelevance(true)]
        public string source
        {
            get => _source;
            set
            {
                _source = value;
                ECAScript.NotifyUpdate(this, nameof(source), _source);
            }
        }
        [SerializeField]
        private string _source;

        /// <summary>
        /// <b>Volume</b> is the video volume.
        /// </summary>
        [StateVariable("volume", ECARules4AllType.Float)]
        public float volume
        {
            get => _volume;
            set
            {
                _volume = value;
                ECAScript.NotifyUpdate(this, nameof(volume), _volume);
            }
        }
        [SerializeField]
        private float _volume;

        /// <summary>
        /// <b> MaxVolume</b> is the video max volume.
        /// </summary>
        [StateVariable("maxVolume", ECARules4AllType.Float)]
        public float maxVolume
        {
            get => _maxVolume;
            set
            {
                _maxVolume = value;
                ECAScript.NotifyUpdate(this, nameof(maxVolume), _maxVolume);
            }
        }
        [SerializeField]
        private float _maxVolume = 1.0f;
        
        /// <summary>
        /// <b>Playing</b> defines whether the video is playing.
        /// </summary>
        [StateVariable("playing", ECARules4AllType.Boolean)]
        [ECARelevance(true)]
        public ECABoolean playing 
        {
            get => _playing;
            set
            {
                _playing = value;
                ECAScript.NotifyUpdate(this, nameof(playing), _playing);
            }
        }
        [SerializeField]
        private ECABoolean _playing = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b>Paused</b> defines whether the video is paused.
        /// </summary>
        [StateVariable("paused", ECARules4AllType.Boolean)] 
        public ECABoolean paused 
        {
            get => _paused;
            set
            {
                _paused = value;
                ECAScript.NotifyUpdate(this, nameof(paused), _paused);
            }
        }
        [SerializeField]
        private ECABoolean _paused = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b>Stopped</b> defines whether the video is stopped.
        /// </summary>
        [StateVariable("stopped", ECARules4AllType.Boolean)] 
        public ECABoolean stopped 
        {
            get => _stopped;
            set
            {
                _stopped = value;
                ECAScript.NotifyUpdate(this, nameof(stopped), _stopped);
            }
        }
        [SerializeField]
        private ECABoolean _stopped = new ECABoolean(ECABoolean.BoolType.YES);

        /// <summary>
        /// <b>Player</b> is the video player to control through the script.
        /// </summary>
        private VideoPlayer _videoPlayer;

        /// <summary>
        /// <b>Plays</b> starts the video.
        /// </summary>
        [Action(typeof(ECAVideo), "plays")]
        public void Plays()
        {
            this.playing= new ECABoolean(ECABoolean.BoolType.YES);
            this.stopped = new ECABoolean(ECABoolean.BoolType.NO);
            this.paused = new ECABoolean(ECABoolean.BoolType.NO);
            _videoPlayer.Play();
        }

        /// <summary>
        /// <b>Pauses</b> pauses the video.
        /// </summary>
        [Action(typeof(ECAVideo), "pauses")]
        public void Pauses()
        {
            this.playing= new ECABoolean(ECABoolean.BoolType.NO);
            this.stopped= new ECABoolean(ECABoolean.BoolType.NO);
            this.paused= new ECABoolean(ECABoolean.BoolType.YES);
            _videoPlayer.Pause();
        }

        /// <summary>
        /// <b>Stops</b> stops the video.
        /// </summary>
        [Action(typeof(ECAVideo), "stops")]
        public void Stops()
        {
            this.playing= new ECABoolean(ECABoolean.BoolType.NO);
            this.stopped= new ECABoolean(ECABoolean.BoolType.YES);
            this.paused= new ECABoolean(ECABoolean.BoolType.NO);
            _videoPlayer.Stop();
        }

        /// <summary>
        /// <b>ChangesVolume</b> changes the video volume to the given value.
        /// If the value is greater than the max volume, the volume is set to the max volume.
        /// If the value is lower than 0, the volume is set to 0.
        /// </summary>
        /// <param name="v">The new video volume. </param>
        [Action(typeof(ECAVideo), "changes", "volume", "to", typeof(float))]
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

            volume = v;
            _videoPlayer.SetDirectAudioVolume(0, volume);
            //trackindex is set to 0, but there may be more than 1 audio track
        }

        /// <summary>
        /// <b>ChangesSource</b> changes the video source to the given value.
        /// The new path must be relative to the user-accessible Inventory folder.
        /// </summary>
        /// <param name="newSource">The path for the new video file.</param>
        [Action(typeof(ECAVideo), "changes", "source", "to", typeof(string))]
        public void ChangesSource(string newSource)
        {
            SelectVideo(newSource);
        }
        
        /*[Action(typeof(ECAVideo), "changes source randomly")]
        public void ChangesSourceRandomly()
        {
            #if UNITY_EDITOR
                var list = new List<string>() { "nature_4k.mp4", "AdvVideo.mp4", "AdvVideo2.mp4" }; 
                SelectVideo(list[new Random().Next(list.Count)]);
            #else
            StartCoroutine(TaxonomyUtils.GetServerFilesInFolder("videos", list =>
            {
                SelectVideo(list[new Random().Next(list.Count)]);
            }));        
            #endif
        }*/


        // private void Update()
        // {
        //     if (playing)
        //     {
        //         currentTime = (float) _videoPlayer.time;
        //     }
        // }
        
        #endregion

    }
}