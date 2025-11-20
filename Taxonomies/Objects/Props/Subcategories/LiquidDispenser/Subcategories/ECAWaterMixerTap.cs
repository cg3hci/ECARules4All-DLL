using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser.Subcategories
{
    /// <summary>
    /// <b>ECAWaterMixerTap</b> is a component that represents a virtual water mixer tap within the environment.
    /// It can be turned to the left, idle (center), or right positions, corresponding respectively to warm,
    /// neutral, and cold water flow states.
    /// Several default automation rules are initialized at startup:
    /// - Turning left starts the flow of warm water.
    /// - Turning right starts the flow of cold water.
    /// - Returning to the idle (center) position stops the water flow.
    /// The component provides properties and actions for controlling its rotation state, managing the water flow,
    /// and responding to user interactions or automation triggers.
    /// </summary>
    [ECARules4All("waterMixerTap")]
    [RequireComponent(typeof(ECALiquidDispenser))]
    [DisallowMultipleComponent]
    public class ECAWaterMixerTap : MonoBehaviour
    {
        private ECALiquidSpawner _waterSpawner;
        public AudioSource audioSource;
        public GameObject tapHandle;
        public GameObject pivot;
        public float PERC_THR = 0.2f; // 10% of the distance as threshold
        private TapState _lastTapState = TapState.IDLE;

        private GameObject player_character => ECAPlayer_Singleton.Instance.playerGoRef;

        private void Awake()
        {
            _waterSpawner = this.GetComponent<ECALiquidDispenser>().liquidSpawner;
            if (_waterSpawner == null)
            {
                throw new Exception("LiquidSpawner is not assigned in the ECAWaterMixerTap script.");
            }
        }

        private void Start()
        {
            void AddRules()
            {
                Log.Debug("Rules in the system before all: " + RuleEngine.GetInstance().Rules().Count());

                var tapTurnsLeftAction = new Action(player_character, "turns-left", this.gameObject);
                var tapTurnsIdleAction = new Action(player_character, "turns-idle", this.gameObject);
                var tapTurnsRightAction = new Action(player_character, "turns-right", this.gameObject);

                var tapFlowsWarmWater = new Action(this.gameObject, "flows-warm-water");
                var tapFlowsColdWater = new Action(this.gameObject, "flows-cold-water");
                var tapStopsFlowingWater = new Action(this.gameObject, "stops-flowing-water");

                var rule_whenTurnsRightThenColdWaterFlows = Rule.TryCreateRule(
                    tapTurnsRightAction,
                    new List<Action> { tapFlowsColdWater }
                );
                RuleEngine.GetInstance().Add(rule_whenTurnsRightThenColdWaterFlows);
                Log.Debug("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenTurnsLeftThenWarmWaterFlows = Rule.TryCreateRule(
                    tapTurnsLeftAction,
                    new List<Action> { tapFlowsWarmWater }
                );
                RuleEngine.GetInstance().Add(rule_whenTurnsLeftThenWarmWaterFlows);
                Log.Debug("Rules in the system after two add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenTurnsMiddleThenStopsFlowingWater = Rule.TryCreateRule(
                    tapTurnsIdleAction,
                    new List<Action> { tapStopsFlowingWater }
                );
                RuleEngine.GetInstance().Add(rule_whenTurnsMiddleThenStopsFlowingWater);
                Log.Debug("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());
            }

            AddRules();
        }

        private void Update()
        {
            var state = CheckTapHandle();
            if (state != _lastTapState)
            {
                _lastTapState = state;
                if (_lastTapState == TapState.IDLE)
                {
                    Action action = new Action(player_character, "turns-idle", this.gameObject);
                    EventBus.GetInstance().Publish(action);
                    ECAScript.NotifyUpdate(this,
                        action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?                
                }
                else if (_lastTapState == TapState.LEFT)
                {
                    Action action = new Action(player_character, "turns-left", this.gameObject);
                    EventBus.GetInstance().Publish(action);
                    ECAScript.NotifyUpdate(this,
                        action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
                }
                else if (_lastTapState == TapState.RIGHT)
                {
                    Action action = new Action(player_character, "turns-right", this.gameObject);
                    EventBus.GetInstance().Publish(action);
                    ECAScript.NotifyUpdate(this,
                        action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
                }
            }

            // if (state == TapState.LEFT || state == TapState.RIGHT)
            // {
            //     if (!audioSource.isPlaying)
            //         audioSource.Play();
            //     if (state == TapState.LEFT)
            //     {
            //         liquidSpawner.SpawnWater(WaterSpawner.WaterType.Warm);
            //     }
            //     else
            //     {
            //         liquidSpawner.SpawnWater(WaterSpawner.WaterType.Cold);
            //     }
            // }
            // else
            // {
            //     if (audioSource.isPlaying)
            //         audioSource.Stop();
            //     liquidSpawner.StopWater();
            // }
        }

        /// <summary>
        /// <b>flows-warm-water</b> represents the action that causes an object equipped with an
        /// <see cref="ECAWaterMixerTap"/> component, typically a tap, to emit warm water. 
        /// </summary>
        [Action(typeof(ECAWaterMixerTap), "flows-warm-water")]
        [ECARelevance(true)]
        public void FlowsWarmWater()
        {
            Log.Debug("ECAWaterMixerTap: FlowsWarmWater called");
            if (!audioSource.isPlaying)
                audioSource.Play();
            Log.Debug("After If");
            Log.Debug("BEFORE Spawn Liquid");
            Log.Debug("Water Spawner " + _waterSpawner);
            Log.Debug("Liquid Spawner " + _waterSpawner.GetType());
            _waterSpawner.SpawnLiquid(ECALiquidSpawner.LiquidTemperature.Warm);
            Log.Debug("AFTER Spawn Liquid");
        }

        /// <summary>
        /// <b>FlowsColdWater</b> causes the tap to emit cold water.
        /// </summary>
        [Action(typeof(ECAWaterMixerTap), "flows-cold-water")]
        [ECARelevance(true)]
        public void FlowsColdWater()
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
            _waterSpawner.SpawnLiquid(ECALiquidSpawner.LiquidTemperature.Cold);
        }

        /// <summary>
        /// <b>StopFlowingWater</b> stops any water from flowing.
        /// </summary>
        [Action(typeof(ECAWaterMixerTap), "stops-flowing-water")]
        [ECARelevance(true)]
        public void StopFlowingWater()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            _waterSpawner.StopSpawningLiquid();
        }

        /// <summary>
        /// <b>TurnsLeft</b> is an action where a character turns the tap handle to the left.
        /// </summary>
        /// <param name="c">The character performing the action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "turns-left", typeof(ECAWaterMixerTap))]
        public void _TurnsLeft(ECACharacter c)
        {
        }

        /// <summary>
        /// <b>TurnsIdle</b> is an action where a character returns the tap handle to the center (idle) position.
        /// </summary>
        /// <param name="c">The character performing the action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "turns-idle", typeof(ECAWaterMixerTap))]
        public void _TurnsIdle(ECACharacter c)
        {
        }

        /// <summary>
        /// <b>TurnsRight</b> is an action where a character turns the tap handle to the right.
        /// </summary>
        /// <param name="c">The character performing the action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "turns-right", typeof(ECAWaterMixerTap))]
        public void _TurnsRight(ECACharacter c)
        {
        }

        public enum TapState
        {
            IDLE,
            LEFT,
            RIGHT
        }

        public TapState CheckTapHandle()
        {
            var vecPivotPosition = tapHandle.transform.position - pivot.transform.position;

            float threshold = PERC_THR * vecPivotPosition.magnitude; // 10% of the distance as threshold

            if (Mathf.Abs(vecPivotPosition.x) <= threshold)
            {
                return TapState.IDLE;
            }
            else if (vecPivotPosition.x < 0)
            {
                return TapState.LEFT;
            }
            else
            {
                return TapState.RIGHT;
            }
        }
    }
}