using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// Represents a time-based <see cref="Behaviour"/> that helps triggering actions after specified durations.
    /// The Timer class provides functionality to configure, start, pause, stop, and reset a timer, as well as to emit events when specific time milestones are reached or elapsed.
    /// </summary>
    [ECARules4All("timer")]
    [RequireComponent(typeof(Behaviour))]
    [DisallowMultipleComponent]
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// <b>Duration</b> specifies the total duration for which the timer will run.
        /// </summary>
        [StateVariable("duration", ECARules4AllType.Float)] 
        [ECARelevance(false)]
        public float duration
        {
            get => _duration;
            set
            {
                _duration = value;
                ECAScript.NotifyUpdate(this, nameof(duration), duration.ToString());
            }
        }
        [SerializeField] 
        private float _duration;
        
        /// <summary>
        /// <b>Current</b> represents the current time of the timer, meaning the elapsed time from the start. It dynamically updates as the timer counts down.
        /// </summary>
        [StateVariable("current-time", ECARules4AllType.Float)] 
        [ECARelevance(false)]
        public float current
        {
            get => _current;
            set
            {
                _current = value;
                ECAScript.NotifyUpdate(this, nameof(current), current.ToString());
            }
        }
        [SerializeField] 
        private float _current;
        
        /// <summary>
        /// <b>Active</b> is a boolean that indicates whether the timer is currently running.
        /// </summary>
        private bool active;
        /// <summary>
        /// <b>Elapsed</b> counts the time elapsed since the timer was started.
        /// </summary>
        private int elapsed;

        /// <summary>
        /// <b>ChangesDuration</b> sets the total duration of the timer with a non-negative value.
        /// </summary>
        /// <param name="amount">The new duration value for the timer.</param>
        [Action(typeof(Timer), "changes","duration", "to", typeof(float))]
        [ECARelevance(false)]
        public void ChangeDuration(float amount)
        {
            if (amount > 0.0f)
                duration = amount;
        }
        
        /// <summary>
        /// <b>ChangeCurrentTime</b> sets the elapsed time of the timer. It ensures that the new value is within the valid range [0, duration].
        /// </summary>
        /// <param name="amount"> The new </param>
        [Action(typeof(Timer), "changes","current-time", "to", typeof(float))]
        [ECARelevance(false)]
        public void ChangeCurrentTime(float amount)
        {
            if (amount < 0.0f)
                current = 0.0f;
            else if (amount > duration)
                current = duration;
            else current = amount;
            
        }

        /// <summary>
        /// <b>Starts</b> activates the timer to begin counting down, resuming its operation from the last paused state.
        /// </summary>
        [Action(typeof(Timer), "starts")]
        [ECARelevance(false)]
        public void Starts()
        {
            active = true;
        }

        /// <summary>
        /// <b>Stops</b> deactivates the timer, resetting the elapsed time to zero.
        /// </summary>
        [Action(typeof(Timer), "stops")]
        [ECARelevance(false)]
        public void Stops()
        {
            active = false;
        }
        
        //TODO: verb "pauses" present in grammar but no function in documentation
        /// <summary>
        /// <b>Pauses</b> deactivates the timer, leaving the elapsed time unchanged.
        /// </summary>
        [Action(typeof(Timer), "pauses")]
        [ECARelevance(false)]
        public void Pauses()
        {
            active = false;
        }
        
        // /// <summary>
        // /// <b>Elapses</b> emits an event when the timer has elapsed a set amount of time.
        // /// </summary>
        // /// <param name="seconds">The amount of time elapsed from the start.</param>
        // [Action(typeof(Timer), "elapses-timer", typeof(int))]
        // public void Elapses(int seconds)
        // {
        //     Action action = new Action(this.gameObject, "elapses-timer", seconds);
        //     EventBus.GetInstance().Publish(action);
        //     NotifyUpdate(action);
        // }
        
        /// <summary>
        /// <b>Reaches</b> emits an event when the timer reaches a specified elapsed time. It can be used to trigger actions at predefined points in the elapsed timeline.
        /// </summary>
        /// <param name="seconds">The elapsed time at which the event is triggered.</param>
        [Action(typeof(Timer), "reaches", typeof(int))]
        [ECARelevance(false)]
        public void Reaches(int seconds)
        {
            Action action = new Action(this.gameObject, "reaches", seconds);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
        }

        /// <summary>
        /// <b>Resets</b> resets the timer to its maximum duration and deactivates it.
        /// </summary>
        [Action(typeof(Timer), "resets")]
        [ECARelevance(false)]
        public void Resets()
        {
            active = false;
            current = duration;
        }

        
        private void Update()
        {
            if (active)
            {
                current -= Time.deltaTime;
                if (current < 0)
                {
                    active = false;
                    current = 0;
                }

                elapsed = (int)(duration - current);
                // Elapses(elapsed);
                Reaches((int)current);
            }
        }
    }
}