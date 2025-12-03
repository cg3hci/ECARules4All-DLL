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
    /// <b>ECABottle</b> is a component that represents a virtual bottle capable of containing and dispensing
    /// liquid within the environment. It maintains internal state variables such as <c>capOpen</c> and <c>flipped</c>,
    /// and interacts with objects equipped with an <see cref="ECALiquidDispenser"/> component to
    /// manage liquid flow and spawning. The bottle provides actions for flipping, opening or closing the cap,
    /// and starting or stopping the liquid flow. Several default automation rules are initialized at startup:
    /// - Flipping the bottle downward while the cap is open causes liquid to flow.
    /// - Flipping the bottle upward stops the liquid flow.
    /// - Closing the cap stops the liquid flow.
    /// - Opening the cap while the bottle is flipped downward resumes the liquid flow.
    /// </summary>
    [ECARules4All("bottle")]
    [RequireComponent(typeof(ECALiquidDispenser))]
    [DisallowMultipleComponent]
    public class ECABottle : MonoBehaviour
    {
        // public AudioSource audioSource;
        private GameObject player_character => ECAPlayer_Singleton.Instance.playerGoRef;
        private ECACharacter player_ecaCharacter_ref => ECAPlayer_Singleton.Instance.playerEcaCharacterRef;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            void AddRules()
            {
                Log.Debug("Rules in the system before all: " + RuleEngine.GetInstance().Rules().Count());

                var flipsDownAction = new Action(this.gameObject, "flips-down");
                var flipsUpAction = new Action(this.gameObject, "flips-up");
                var capOpensAction = new Action(player_character, "opens-cap", this.gameObject);
                var capClosesAction = new Action(player_character, "closes-cap", this.gameObject);

                var capIsOpenCondition = new SimpleCondition(this.gameObject, "capOpen", "is", ECABoolean.YES);
                var bottleIsFlippedCondition = new SimpleCondition(this.gameObject, "flipped", "is", ECABoolean.YES);

                var dropsLiquidAction = new Action(this.gameObject, "drops-liquid");
                var stopsDroppingLiquidAction = new Action(this.gameObject, "stops-dropping");

                var rule_whenFlipDownIfCapOpenThenLiquidDrops = Rule.TryCreateRule(
                    flipsDownAction,
                    capIsOpenCondition,
                    new List<Action> { dropsLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenFlipDownIfCapOpenThenLiquidDrops);
                Log.Debug("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenFlipUpThenWaterStopsDropping = Rule.TryCreateRule(
                    flipsUpAction,
                    new List<Action> { stopsDroppingLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenFlipUpThenWaterStopsDropping);
                Log.Debug("Rules in the system after two add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenClosesCapThenWaterStopsDropping = Rule.TryCreateRule(
                    capClosesAction,
                    new List<Action> { stopsDroppingLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenClosesCapThenWaterStopsDropping);
                Log.Debug("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());

                var rule_whenOpensCapIfFlippedDownThenWaterDrops = Rule.TryCreateRule(
                    capOpensAction,
                    bottleIsFlippedCondition,
                    new List<Action> { dropsLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenOpensCapIfFlippedDownThenWaterDrops);
                Log.Debug("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());
            }

            AddRules();
        }

        [Range(0f, 1f)]
        private float
            _angleOpenBottle = 0.9f; // value between 0 and 1, where 1 means the bottle is perfectly upside down

        private ECALiquidSpawner _waterSpawner;
        public GameObject capGO;

        private void Awake()
        {
            _waterSpawner = this.GetComponent<ECALiquidDispenser>().liquidSpawner;
            if (_waterSpawner == null)
            {
                throw new Exception("ERROR: liquidSpawner == null");
            }

            flipped = Vector3.Dot(gameObject.transform.up, Vector3.down) > _angleOpenBottle
                ? ECABoolean.YES
                : ECABoolean.NO;

            if (capGO == null)
            {
                throw new Exception("ERROR: capGO == null");
            }

            if (_capOpen != ECABoolean.YES && _capOpen != ECABoolean.NO)
            {
                throw new Exception("ERROR: capOpen must be either ECABoolean.YES or ECABoolean.NO");
            }

            // capGO.GetComponent<Renderer>().enabled = _capOpen == ECABoolean.NO; // if the cap is open, make sure the cap MeshRenderer is enabled
            UpdateCapVisibility();
            // capGO.SetActive(_capOpen == ECABoolean.YES); // if the cap is open, make sure the cap GameObject is active
        }

        private void UpdateCapVisibility()
        {
            if (capGO == null) return; // if capGO is not set, do nothing
            capGO.GetComponent<Renderer>().enabled =
                _capOpen == ECABoolean.NO; // if the cap is open, make sure the cap MeshRenderer is enabled
        }

        /// <summary>
        /// <b>capOpen</b> indicates whether the cap of the bottle is open (<c>YES</c>) or closed (<c>NO</c>).
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("capOpen", ECARules4AllType.Boolean)]
        public ECABoolean capOpen
        {
            get => _capOpen;
            set
            {
                _capOpen = value;
                ECAScript.NotifyUpdate(this, nameof(capOpen), capOpen.ToString());
            }
        }

        [SerializeField] private ECABoolean _capOpen = new ECABoolean(ECABoolean.BoolType.NO);

        /// <summary>
        /// <b>flipped</b> indicates whether the bottle is currently flipped upside down (<c>YES</c>) or upright (<c>NO</c>).
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("flipped", ECARules4AllType.Boolean)]
        public ECABoolean flipped
        {
            get => _flipped;
            set
            {
                _flipped = value;
                Log.Debug("CHANGE");
                ECAScript.NotifyUpdate(this, nameof(flipped), flipped.ToString());
            }
        }

        [SerializeField] private ECABoolean _flipped = new ECABoolean(ECABoolean.BoolType.NO);
        
        /// <summary>
        /// <b>opens-cap</b> represents the action of opening the cap of a bottle equipped with an <see cref="ECABottle"/>
        /// component.
        /// When executed, it sets the internal state variable <c>capOpen</c> to <c>true</c>
        /// and, if the bottle is flipped downward, triggers the <b>drops-liquid</b> action to start the liquid flow.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "opens-cap", typeof(ECABottle))]
        public void _OpenCap(ECACharacter c)
        {
            if (capOpen == ECABoolean.YES) return; // if already open, do nothing
            capOpen = ECABoolean.YES;
            // capGO.SetActive(true);
            UpdateCapVisibility();
            Log.Debug("the cap is open");
        }

        /// <summary>
        /// <b>closes-cap</b> represents the action of closing the cap of a bottle equipped with
        /// an <see cref="ECABottle"/> component.
        /// When executed, it sets the internal state variable <c>capOpen</c> to <c>false</c>
        /// and triggers the <b>stops-dropping</b> action to halt any ongoing liquid flow.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECACharacter), "closes-cap", typeof(ECABottle))]
        public void _CloseCap(ECACharacter c)
        {
            if (capOpen == ECABoolean.NO) return; // if already closed, do nothing
            capOpen = ECABoolean.NO;
            // capGO.SetActive(false)
            UpdateCapVisibility();
            Log.Debug("the cap is closed");
        }

        /// <summary>
        /// <b>flips-down</b> represents the action of turning a bottle equipped with an <see cref="ECABottle"/> component
        /// upside down. When executed, it updates the internal state variable <c>flipped</c> to <c>true</c>
        /// and, if the cap is open, triggers the <b>drops-liquid</b> action to start the liquid flow.
        /// </summary>
        [Action(typeof(ECABottle), "flips-down")]
        [ECARelevance(true)]
        public void FlipsDown()
        {
            Log.Debug("FlipsDown called");
            flipped = ECABoolean.YES;
            // Action action = new Action(this.gameObject, "flips-down");
            // EventBus.GetInstance().Publish(action);
            // ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        }

        /// <summary>
        /// <b>flips-up</b> represents the action of turning a bottle equipped with an <see cref="ECABottle"/> component
        /// to an upright position. When executed, it updates the internal state variable <c>flipped</c> to <c>false</c>
        /// and stops any ongoing liquid flow if the bottle was previously pouring.
        /// </summary>
        [Action(typeof(ECABottle), "flips-up")]
        [ECARelevance(true)]
        public void FlipsUp()
        {
            Log.Debug("FlipsUp called");
            flipped = ECABoolean.NO; // Update the flipped state
            // Action action = new Action(this.gameObject, "flips-up");
            // EventBus.GetInstance().Publish(action);
            // ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        }


        /// <summary>
        /// <b>drops liquid</b> represents the action that initiates the release of liquid from the spawner
        /// associated with an object equipped with an <see cref="ECABottle"/> component.
        /// When executed, it starts the spawning of liquid particles or drops within the environment.
        /// </summary>
        [Action(typeof(ECABottle), "drops-liquid")]
        [ECARelevance(true)]
        public void DropsLiquid()
        {
            Log.Debug("DropsLiquid called");
            _waterSpawner.SpawnLiquid(ECALiquidSpawner.LiquidTemperature.Ambient);
        }

        /// <summary>
        /// <b>stops dropping</b> represents the internal action that stops the flow of liquid from an object
        /// equipped with an <see cref="ECABottle"/> component.
        /// When executed, it halts the spawning of liquid particles or drops and updates the dispenser’s internal state
        /// to indicate that the liquid flow has stopped.
        /// </summary>
        [Action(typeof(ECABottle), "stops-dropping")]
        [ECARelevance(true)]
        public void StopsDropping()
        {
            Debug.Log("StopsDropping called");
            _waterSpawner.StopSpawningLiquid();
        }

        /// <summary>
        /// Toggles the cap state between open and closed, and publishes the appropriate action.
        /// </summary>
        public void ToggleCap()
        {
            if (capOpen == ECABoolean.YES)
            {
                _CloseCap(player_ecaCharacter_ref);
                Action action = new Action(player_character, "closes-cap", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
            else
            {
                _OpenCap(player_ecaCharacter_ref);
                Action action = new Action(player_character, "opens-cap", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }

        private void Update()
        {
            var isCurrFlipped = Vector3.Dot(gameObject.transform.up, Vector3.down) > _angleOpenBottle
                ? ECABoolean.YES
                : ECABoolean.NO;
            
            if (isCurrFlipped == flipped) return;

            // Log.Debug("[ECABootle - Update] isCurrFlipped " + isCurrFlipped + " vs flipped: " + flipped +
            //           " .... isCurrFlipped == flipped? " + (isCurrFlipped == flipped));
            if (isCurrFlipped == ECABoolean.YES)
            {
                //Log.Debug("[ECABootle - Update] FlipsDown");
                FlipsDown();
                Action action = new Action(this.gameObject, "flips-down");
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
            else
            {
                //Log.Debug("[ECABootle - Update] FlipsUp");
                FlipsUp();
                Action action = new Action(this.gameObject, "flips-up");
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }

            //Log.Debug("[ECABootle - Update] End Update Bottle");
        }

        private void OnGUI()
        {
            // make 2 buttons: one for opening the cap and one for closing the cap
            if (GUI.Button(new Rect(10, 10, 150, 30), "Open Cap"))
            {
                _OpenCap(player_ecaCharacter_ref);
                var action = new Action(player_character, "opens-cap", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?      
            }

            if (GUI.Button(new Rect(10, 50, 150, 30), "Close Cap"))
            {
                _CloseCap(player_ecaCharacter_ref);
                var action = new Action(player_character, "closes-cap", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }

            if (GUI.Button(new Rect(10, 90, 150, 30), "Toggle Cap"))
            {
                ToggleCap();
            }
        }
    }
}