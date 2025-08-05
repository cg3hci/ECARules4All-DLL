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
    /// Defines a custom ECA object representing a bottle, with properties and actions
    /// such as fill level and whether the cap is open or closed.
    /// </summary>
    [ECARules4All("bottle")]
    [RequireComponent(typeof(ECALiquidDispenser))]
    [DisallowMultipleComponent]
    public class ECABottle : MonoBehaviour
    {
        // public AudioSource audioSource;
        private GameObject player_character =>
            GameObject.FindObjectOfType<Character>()
                .gameObject; //TODO J 2nd July '25: Salvare la reference al personaggio del giocatore in un campo privato, per evitare di cercarlo ogni volta.

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            void AddRules()
            {
                Debug.Log("Rules in the system before all: " + RuleEngine.GetInstance().Rules().Count());

                var flipsDownAction = new Action(this.gameObject, "flips-down");
                var flipsUpAction = new Action(this.gameObject, "flips-up");
                var capOpensAction = new Action(player_character, "opens-cap", this.gameObject);
                var capClosesAction = new Action(player_character, "closes-cap", this.gameObject);

                var capIsOpenCondition = new SimpleCondition(this.gameObject, "capOpen", "is", ECABoolean.YES);
                var bottleIsFlippedCondition = new SimpleCondition(this.gameObject, "flipped", "is", ECABoolean.YES);

                var dropsLiquidAction = new Action(this.gameObject, "drops-liquid");
                var stopsDroppingLiquidAction = new Action(this.gameObject, "stops-dropping");

                // var test1 = Rule.TryCreateRule(
                //     flipsDownAction,
                //     // capIsOpenCondition,
                //     new List<Action> { dropsLiquidAction}
                // );
                // RuleEngine.GetInstance().Add(test1);
                // Debug.Log("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());
                //
                // var test2 = Rule.TryCreateRule(
                //     flipsUpAction,
                //     new List<Action> { stopsDroppingLiquidAction }
                // );
                // RuleEngine.GetInstance().Add(test2);

                var rule_whenFlipDownIfCapOpenThenLiquidDrops = Rule.TryCreateRule(
                    flipsDownAction,
                    capIsOpenCondition,
                    new List<Action> { dropsLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenFlipDownIfCapOpenThenLiquidDrops);
                Debug.Log("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenFlipUpThenWaterStopsDropping = Rule.TryCreateRule(
                    flipsUpAction,
                    new List<Action> { stopsDroppingLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenFlipUpThenWaterStopsDropping);
                Debug.Log("Rules in the system after two add: " + RuleEngine.GetInstance().Rules().Count());


                var rule_whenClosesCapThenWaterStopsDropping = Rule.TryCreateRule(
                    capClosesAction,
                    new List<Action> { stopsDroppingLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenClosesCapThenWaterStopsDropping);
                Debug.Log("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());

                var rule_whenOpensCapIfFlippedDownThenWaterDrops = Rule.TryCreateRule(
                    capOpensAction,
                    bottleIsFlippedCondition,
                    new List<Action> { dropsLiquidAction }
                );
                RuleEngine.GetInstance().Add(rule_whenOpensCapIfFlippedDownThenWaterDrops);
                Debug.Log("Rules in the system after one add: " + RuleEngine.GetInstance().Rules().Count());
            }

            AddRules();
        }

        [Range(0f, 1f)]
        private float
            _angleOpenBottle = 0.9f; // value between 0 and 1, where 1 means the bottle is perfectly upside down

        private LiquidSpawner _waterSpawner;
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
        /// <b>capOpen</b> TODO.
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
        /// <b>flipped</b> TODO.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("flipped", ECARules4AllType.Boolean)]
        public ECABoolean flipped
        {
            get => _flipped;
            set
            {
                // if (value != _flipped)
                // {
                _flipped = value;
                Debug.Log("CHANGE");
                ECAScript.NotifyUpdate(this, nameof(flipped), flipped.ToString());
                // Debug.Log("Rules in the system: " + RuleEngine.GetInstance().Rules().Count());
                // }
            }
        }

        [SerializeField] private ECABoolean _flipped = new ECABoolean(ECABoolean.BoolType.NO);


        /// <summary>
        /// Opens the bottle cap, if it is currently closed.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(Character), "opens-cap", typeof(ECABottle))]
        public void _OpenCap()
        {
            if (capOpen == ECABoolean.YES) return; // if already open, do nothing
            capOpen = ECABoolean.YES;
            // capGO.SetActive(true);
            UpdateCapVisibility();
            Debug.Log("the cap is open");
        }

        /// <summary>
        /// Closes the bottle cap, if it is currently open.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(Character), "closes-cap", typeof(ECABottle))]
        public void _CloseCap()
        {
            if (capOpen == ECABoolean.NO) return; // if already closed, do nothing
            capOpen = ECABoolean.NO;
            // capGO.SetActive(false)
            UpdateCapVisibility();
            Debug.Log("the cap is closed");
        }

        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(ECABottle), "flips-down")]
        [ECARelevance(true)]
        public void FlipsDown()
        {
            Debug.Log("FlipsDown called");
            flipped = ECABoolean.YES;
            // Action action = new Action(this.gameObject, "flips-down");
            // EventBus.GetInstance().Publish(action);
            // ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        }

        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(ECABottle), "flips-up")]
        [ECARelevance(true)]
        public void FlipsUp()
        {
            Debug.Log("FlipsUp called");
            flipped = ECABoolean.NO; // Update the flipped state
            // Action action = new Action(this.gameObject, "flips-up");
            // EventBus.GetInstance().Publish(action);
            // ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        }


        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(ECABottle), "drops-liquid")]
        [ECARelevance(true)]
        public void DropsLiquid()
        {
            Debug.Log("DropsLiquid called");
            _waterSpawner.SpawnLiquid(LiquidSpawner.LiquidTemperature.Ambient);
        }

        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(ECABottle), "stops-dropping")]
        [ECARelevance(true)]
        public void StopsDropping()
        {
            Debug.Log("StopsDropping called");
            _waterSpawner.StopSpawningLiquid();
        }

        public void ToggleCap()
        {
            if (capOpen == ECABoolean.YES)
            {
                _CloseCap();
                Action action = new Action(player_character, "closes-cap", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
            else
            {
                _OpenCap();
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
            // Debug.Log("PPPPP Start Update Bottle");
            // Debug.Log("PPPPP Vector3.Dot(gameObject.transform.up, Vector3.down): " + Vector3.Dot(gameObject.transform.up, Vector3.down));
            // Debug.Log("PPPPP CurrFlipped: " + isCurrFlipped + " vs flipped:" + flipped + " .... isCurrFlipped == flipped? " + (isCurrFlipped == flipped));
            if (isCurrFlipped == flipped) return;

            Debug.Log("PPPP isCurrFlipped " + isCurrFlipped + " vs flipped: " + flipped +
                      " .... isCurrFlipped == flipped? " + (isCurrFlipped == flipped));
            if (isCurrFlipped == ECABoolean.YES)
            {
                Debug.Log("PPPPP FlipsDown");
                FlipsDown();
                Action action = new Action(this.gameObject, "flips-down");
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
            else
            {
                Debug.Log("PPPPP FlipsUp");
                FlipsUp();
                Action action = new Action(this.gameObject, "flips-up");
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }

            Debug.Log("PPPPP End Update Bottle");
            Debug.Log("PPPPP");
        }

        private void OnGUI()
        {
            // make 2 buttons: one for opening the cap and one for closing the cap
            if (GUI.Button(new Rect(10, 10, 150, 30), "Open Cap"))
            {
                _OpenCap();
                var action = new Action(player_character, "opens-cap", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?      
            }

            if (GUI.Button(new Rect(10, 50, 150, 30), "Close Cap"))
            {
                _CloseCap();
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
    
    /*/// <summary>
    /// Defines a custom ECA object representing a bottle, with properties and actions
    /// such as fill level and whether the cap is open or closed.
    /// </summary>
    [ECARules4All("bottle")]
    [RequireComponent(typeof(ECAObject))]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(Interactable))]
    [DisallowMultipleComponent]
    public class ECABottle : MonoBehaviour
    {
        /// <summary>
        /// <b>Level</b>: Represents the current fill level of the bottle (0–100).
        /// </summary>
        [StateVariable("fillLevel", ECARules4AllType.Integer)] 
        [ECARelevance(true)] 
        public int Level
        {
            get => _level;
            private set
            {
                int newValue = Mathf.Clamp(value, 0, 100);
                if (_level != newValue)
                {
                    _level = newValue;
                    ECAScript.NotifyUpdate(this, nameof(Level), _level.ToString());
                }
            }
        }

        [SerializeField]
        [Range(0,100)]
        private int _level = 100;

        /// <summary>
        /// <b>IsCapped</b>: Indicates whether the bottle is currently capped.
        /// </summary>
        [StateVariable("IsCapped", ECARules4AllType.Boolean)]
        [ECARelevance(true)]
        public ECABoolean IsCapped
        {
            get => _isCapped;
            private set
            {
                if (_isCapped != value)
                {
                    _isCapped = value;
                    ECAScript.NotifyUpdate(this, nameof(IsCapped), _isCapped.ToString());
                }
            }
        }

        [SerializeField] private ECABoolean _isCapped = new ECABoolean(true);
        
        /// <summary>
        /// Increases the fill level of the bottle by a specified amount, 
        /// if the cap is open.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABottle), "increase", "level", "by", typeof(int))]
        public void Fill(int amount)
        {
            if (amount <= 0) return;

            if (IsCapped == ECABoolean.YES)
            {
                Debug.Log("The ECABottle is capped");
                return;
            }
            Level += amount;
        }

        /// <summary>
        /// Decreases the fill level of the bottle by a specified amount, 
        /// if the cap is open and the bottle is not empty.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABottle), "decrease", "level", "by", typeof(int))]
        public void Empty(int amount)
        {
            if (IsCapped == ECABoolean.YES)
            {
                Debug.Log("The ECABottle is capped");
                return;
            }
            if (Level == 0)
            {
                Debug.Log("The ECABottle is empty");
                return;
            }

            Level -= amount;
        }

        /// <summary>
        /// Opens the bottle cap, if it is currently closed.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABottle), "open cap")]
        public void OpenCap()
        {
            if (IsCapped == ECABoolean.NO) return;
            IsCapped = ECABoolean.NO;
            Debug.Log("the cap is open");
        }

        /// <summary>
        /// Closes the bottle cap, if it is currently open.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABottle), "close cap")]
        public void CloseCap()
        {
            if (IsCapped == ECABoolean.YES) return;
            IsCapped = ECABoolean.YES;
            Debug.Log("the cap is closed");
        }
    }*/
}