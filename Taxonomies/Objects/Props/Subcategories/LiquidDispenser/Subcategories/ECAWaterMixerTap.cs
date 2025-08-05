using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser.Subcategories
{
    /// <summary>
    /// Defines a custom ECA object representing a TODO
    /// TODO: Add more properties and actions as needed.
    /// </summary>
    [ECARules4All("waterMixerTap")]
    [RequireComponent(typeof(ECALiquidDispenser))]
    [DisallowMultipleComponent]
    public class ECAWaterMixerTap : MonoBehaviour
    {
        private LiquidSpawner _waterSpawner;
        public AudioSource audioSource;
        public GameObject tapHandle;
        public GameObject pivot;
        public float PERC_THR = 0.2f; // 10% of the distance as threshold
        private TapState _lastTapState = TapState.IDLE;

        private GameObject player_character =>
            GameObject.FindObjectOfType<Character>()
                .gameObject; //TODO J 2nd July '25: Salvare la reference al personaggio del giocatore in un campo privato, per evitare di cercarlo ogni volta.

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
                Debug.Log("Rules in the system before all: " + RuleEngine.GetInstance().Rules().Count());

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
                Debug.Log("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenTurnsLeftThenWarmWaterFlows = Rule.TryCreateRule(
                    tapTurnsLeftAction,
                    new List<Action> { tapFlowsWarmWater }
                );
                RuleEngine.GetInstance().Add(rule_whenTurnsLeftThenWarmWaterFlows);
                Debug.Log("Rules in the system after two add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenTurnsMiddleThenStopsFlowingWater = Rule.TryCreateRule(
                    tapTurnsIdleAction,
                    new List<Action> { tapStopsFlowingWater }
                );
                RuleEngine.GetInstance().Add(rule_whenTurnsMiddleThenStopsFlowingWater);
                Debug.Log("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());
            }

            AddRules();
        }

        private void Update()
        {
            var state = CheckTapHandle();
            if (state != _lastTapState)
            {
                // Debug.Log("Tap state changed from " + _lastTapState + " to " + state);
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
                    // [Action(typeof(Character), "turns-left", typeof(ECAWaterMixerTap))]
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
            else
            {
                // Debug.Log("Tap state remains " + state);
                return;
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

        // private void OnGUI()
        // {
        // if (GUI.Button(new Rect(10, 10, 150, 30), "Turn Left"))
        // {
        //     // [Action(typeof(Character), "turns-left", typeof(ECAWaterMixerTap))]
        //     Action action = new Action(player_character, "turns-left", this.gameObject);
        //     Debug.Log("Action getType " + action.GetActionType());
        //     EventBus.GetInstance().Publish(action);
        //     ECAScript.NotifyUpdate(this,
        //         action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        // }
        // }

        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(ECAWaterMixerTap), "flows-warm-water")]
        [ECARelevance(true)]
        public void FlowsWarmWater()
        {
            Debug.Log("ECAWaterMixerTap: FlowsWarmWater called");
            if (!audioSource.isPlaying)
                audioSource.Play();
            Debug.Log("After If");
            Debug.Log("BEFORE Spawn Liquid");
            Debug.Log("Water Spawner " + _waterSpawner);
            Debug.Log("Liquid Spawner " + _waterSpawner.GetType());
            _waterSpawner.SpawnLiquid(LiquidSpawner.LiquidTemperature.Warm);
            Debug.Log("AFTER Spawn Liquid");
        }

        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(ECAWaterMixerTap), "flows-cold-water")]
        [ECARelevance(true)]
        public void FlowsColdWater()
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
            _waterSpawner.SpawnLiquid(LiquidSpawner.LiquidTemperature.Cold);
        }

        /// <summary>
        /// TODO
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
        /// <b>Nome metodo</b> TODO
        /// This is a passive action, so the Food type is not in the subject of the action, but on the object.
        /// </summary>
        /// <param name="c">TODO</param>
        [ECARelevance(true)]
        [Action(typeof(Character), "turns-left", typeof(ECAWaterMixerTap))]
        public void _TurnsLeft(Character c)
        {
        }

        /// <summary>
        /// <b>Nome metodo</b> TODO
        /// This is a passive action, so the Food type is not in the subject of the action, but on the object.
        /// </summary>
        /// <param name="c">TODO</param>
        [ECARelevance(true)]
        [Action(typeof(Character), "turns-idle", typeof(ECAWaterMixerTap))]
        public void _TurnsIdle(Character c)
        {
        }

        /// <summary>
        /// <b>Nome metodo</b> TODO
        /// This is a passive action, so the Food type is not in the subject of the action, but on the object.
        /// </summary>
        /// <param name="c">TODO</param>
        [ECARelevance(true)]
        [Action(typeof(Character), "turns-right", typeof(ECAWaterMixerTap))]
        public void _TurnsRight(Character c)
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
            // Debug.Log("ABCDEFGHILMNO Checking tap handle. Rotation(x,y,z)= " + tapHandle.transform.rotation);
            // Debug.Log("ABCDEFGHILMNO Checking tap handle. Rotation Euler (x,y,z)= " +
            //           tapHandle.transform.rotation.eulerAngles);
            // Debug.Log("ABCDEFGHILMNO Checking tap handle. LocalRotation(x,y,z)= " + tapHandle.transform.localRotation);
            // Debug.Log("ABCDEFGHILMNO Checking tap handle. LocalRotation Euler(x,y,z)= " +
            //           tapHandle.transform.localRotation.eulerAngles);

            var vecPivotPosition = tapHandle.transform.position - pivot.transform.position;
            // Debug.Log("ABCDEFGHILMNO Vector Pivot --> Position: " + vecPivotPosition);


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