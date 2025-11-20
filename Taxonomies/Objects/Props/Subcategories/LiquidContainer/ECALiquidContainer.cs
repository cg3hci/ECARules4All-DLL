using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidContainer
{
    /// <summary>
    /// <b>ECALiquidContainer</b> is a component that represents a virtual container capable of holding different
    /// types of virtual liquids and tracking their fill levels within the environment.
    /// </summary>
    [ECARules4All("liquidContainer")]
    [RequireComponent(typeof(ECAProp))]
    [DisallowMultipleComponent]
    public class ECALiquidContainer : MonoBehaviour
    {
        public FillStep startStep; // represents the initial fill step and position of the container.
        public FillStep endStep;  // represents the final fill step and position of the container.

        public GameObject fillstepListParent; // Parent GameObject for the fill steps, if needed for organization in the hierarchy

        [SerializeField] public int numberOfFillSteps = 5; // how many discrete fill levels exist between start and end, including them.
        private List<FillStep> fillSteps = new List<FillStep>();  // Internal list to store generated fill steps
        // public bool renderSteps = true; // If true, render the fill steps in the inspector

        public GameObject waterCircleInstance; // A 2D visual indicator (blue circle) representing the current water fill level.
        public float liquidPerLevel = 0.04f; // Amount of liquid required to move from one fill level to the next.

        private void Start()
        {
            if (waterCircleInstance == null)
            {
                throw new Exception("PrefabWater is not assigned in the Bucket script.");
            }

            if (startStep == null || endStep == null)
            {
                throw new Exception("StartStep and EndStep must be assigned in the Bucket script.");
            }

            void GenerateFillSteps()
            {
                fillSteps.Clear();

                if (numberOfFillSteps < 2 || startStep == null || endStep == null)
                {
                    Debug.LogWarning("Invalid fill step configuration.");
                    return;
                }

                for (int i = 0; i < numberOfFillSteps; i++)
                {
                    float t = i / (float)(numberOfFillSteps - 1); // normalized between 0 and 1

                    // Interpolate both level and position
                    int interpolatedLevel = Mathf.RoundToInt(Mathf.Lerp(startStep.level, endStep.level, t));

                    Vector3 interpolatedPosition =
                        Vector3.Lerp(startStep.levelTransform.position, endStep.levelTransform.position, t);

                    GameObject tempObj = new GameObject("GeneratedFillStep_" + i);
                    tempObj.transform.SetParent(fillstepListParent?.transform ??
                                                transform); // Set parent if fillstepListParent is assigned
                    tempObj.transform.position = interpolatedPosition;

                    FillStep step = new FillStep
                    {
                        level = interpolatedLevel,
                        levelTransform = tempObj.transform
                    };

                    fillSteps.Add(step);
                }
            }

            GenerateFillSteps();
            startStep.levelTransform.gameObject.SetActive(false);
            endStep.levelTransform.gameObject.SetActive(false);


            waterCircleInstance.SetActive(false);
            
            void AddDummyRules()
            {
                Debug.Log("[ECALiquidcontainer - AddDummyRules] ECALiquidcontainer Rules in the system before all: " +
                          RuleEngine.GetInstance().Rules().Count());

                ECALiquidDispenser
                    liquidDispenser =
                        GameObject.Find("PrefabTap")
                            .GetComponent<
                                ECALiquidDispenser>(); // Assuming there's only one liquid dispenser in the scene
                var fillsinAction = new Action(liquidDispenser.gameObject, "fills-in", this.gameObject);

                var isVisibleCondition = new SimpleCondition(this.gameObject, "visible", "is", ECABoolean.YES);
                var isHiddenCondition = new SimpleCondition(this.gameObject, "visible", "is", ECABoolean.NO);

                var hidesAction = new Action(this.gameObject, "hides");
                var showsAction = new Action(this.gameObject, "shows");

                var rule_whenFillsIn_ifVisible_thenHides = Rule.TryCreateRule(
                    fillsinAction,
                    isVisibleCondition,
                    new List<Action> { hidesAction }
                );
                RuleEngine.GetInstance().Add(rule_whenFillsIn_ifVisible_thenHides);
                Debug.Log("[ECALiquidcontainer - AddDummyRules] ECALiquidcontainer Rules in the system after one add: " +
                          RuleEngine.GetInstance().Rules().Count());

                var rule_whenFillsIn_ifHidden_thenShows = Rule.TryCreateRule(
                    fillsinAction,
                    isHiddenCondition,
                    new List<Action> { showsAction }
                );
                RuleEngine.GetInstance().Add(rule_whenFillsIn_ifHidden_thenShows);
                Debug.Log("[ECALiquidcontainer - AddDummyRules] ECALiquidcontainer Rules in the system after one add: " +
                          RuleEngine.GetInstance().Rules().Count());
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            var liquidDropRef = other.gameObject.GetComponent<ECALiquidDrop>();
            if (liquidDropRef != null)
            {
                HandleWaterTouching(liquidDropRef);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var liquidDropRef = other.gameObject.GetComponent<ECALiquidDrop>();
            if (liquidDropRef != null)
            {
                HandleWaterTouching(liquidDropRef);
            }
        }

        private void HandleWaterTouching(ECALiquidDrop liquidRef)
        {
            //////// Update counters based on liquid type ////////
            var lqType = liquidRef.owner.liquidSpawner.GetLiquidType();
            if (lqType == ECALiquidSpawner.LiquidType.Water)
            {
                waterDrops += 1;
            }
            else if (lqType == ECALiquidSpawner.LiquidType.Degreaser)
            {
                degreaserDrops += 1;
            }
            else if (lqType == ECALiquidSpawner.LiquidType.BatteryKiller)
            {
                batteryKillerDrops += 1;
            }
            else if (lqType == ECALiquidSpawner.LiquidType.Amuchina)
            {
                amuchinaDrops += 1;
            }
            
            ////////  Update rendered water level ////////
            var totalLiquidIn = waterDrops + degreaserDrops + batteryKillerDrops + amuchinaDrops;
            int currentLevel = Mathf.FloorToInt(totalLiquidIn / liquidPerLevel);

            // Find the highest FillStep whose level is <= currentLevel
            FillStep bestStep = null;
            foreach (var step in fillSteps)
            {
                Log.Debug(
                    $"Checking currentLevel >= step.level = {step.level} >= {currentLevel} = {currentLevel >= step.level}");
                if (currentLevel >= step.level)
                {
                    bestStep = step;
                }
                else
                {
                    break; // assuming fillSteps are sorted by level
                }
            }

            if (bestStep != null)
            {
                // if disabled, enable waterCircleInstance
                // Move water visual to the fill level's transform
                if (!waterCircleInstance.activeSelf)
                {
                    Log.Debug("Enabling waterCircleInstance");
                    waterCircleInstance.SetActive(true);
                }
                waterCircleInstance.transform.position = bestStep.levelTransform.position;
            }

            ////////  Update temperature  ////////
            float UpdateBucketTemperature(float oldBucketTemperature, float dropTemperature, int existingDropCount)
            {
                Log.Debug("dropTemperature = " + dropTemperature);
                if (existingDropCount <= 0)
                {
                    // If this is the first drop, the bucket's temperature becomes the drop's temperature
                    return dropTemperature;
                }
                float newTemp = ((oldBucketTemperature * existingDropCount) + dropTemperature) /
                                (existingDropCount + 1);
                // Snap to dropTemperature if within 0.5°C
                if (Mathf.Abs(newTemp - dropTemperature) <= 0.5f)
                {
                    return dropTemperature;
                }

                return newTemp;
            }

            temperature = UpdateBucketTemperature(temperature, liquidRef.temperature, totalLiquidIn - 1);

            ////////  Notify the new action  ////////
            var action = new Action(liquidRef.owner.liquidSpawner.liquidDispenser.gameObject, "fills-in",
                this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
        }
	
        /// <summary>
        /// <b>waterDrops</b> counts how many water drops have been added to the container.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("waterDrops", ECARules4AllType.Integer)]
        public int waterDrops
        {
            get => _waterDrops;
            set
            {
                _waterDrops = value;
                ECAScript.NotifyUpdate(this, nameof(waterDrops), waterDrops.ToString());
            }
        }
        [SerializeField] private int _waterDrops = 0;
        
        /// <summary>
        /// <b>degreaserDrops</b> counts how many degreaser drops have been added to the container.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("degreaserDrops", ECARules4AllType.Integer)]
        public int degreaserDrops
        {
            get => _degreaserDrops;
            set
            {
                _degreaserDrops = value;
                ECAScript.NotifyUpdate(this, nameof(degreaserDrops), degreaserDrops.ToString());
            }
        }
        [SerializeField] private int _degreaserDrops = 0;
        
        /// <summary>
        /// <b>batteryKillerDrops</b> counts how many battery killer drops have been added to the container.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("batteryKillerDrops", ECARules4AllType.Integer)]
        public int batteryKillerDrops
        {
            get => _batteryKillerDrops;
            set
            {
                _batteryKillerDrops = value;
                ECAScript.NotifyUpdate(this, nameof(batteryKillerDrops), batteryKillerDrops.ToString());
            }
        }
        [SerializeField] private int _batteryKillerDrops = 0;

        /// <summary>
        /// <b>amuchinaDrops</b> counts how many amuchina drops have been added to the container.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("amuchinaDrops", ECARules4AllType.Integer)]
        public int amuchinaDrops
        {
            get => _amuchinaDrops;
            set
            {
                _amuchinaDrops = value;
                ECAScript.NotifyUpdate(this, nameof(amuchinaDrops), amuchinaDrops.ToString());
            }
        }
        [SerializeField] private int _amuchinaDrops = 0;


        /// <summary>
        /// <b>temperature</b> represents the current temperature of the liquid mixture inside the container.
        /// It is updated dynamically as new liquid drops with different temperatures are added.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("temperature", ECARules4AllType.Float)]
        public float temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                ECAScript.NotifyUpdate(this, nameof(temperature), temperature.ToString());
            }
        }
        [SerializeField] private float _temperature = 0;

        /// <summary>
        /// <b>fills-in</b> represents the action performed when an object equipped with an <see cref="ECALiquidContainer"/> component
        /// is filled by an object equipped with an <see cref="ECALiquidDispenser"/> component.
        /// </summary>
        /// <param name="dispenser">The object equipped with an <see cref="ECALiquidDispenser"/> component that fills the container.</param>
        [ECARelevance(true)]
        [Action(typeof(ECALiquidDispenser), "fills-in", typeof(ECALiquidContainer))]
        public void _FillsIn(ECALiquidDispenser dispenser)
        {
            //TODO Make the logic
            Debug.Log("FillsIn by one drop");
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 30), "FillsInTrigger"))
            {
                ECALiquidDispenser
                    liquidDispenser =
                        GameObject.Find("PrefabTap")
                            .GetComponent<
                                ECALiquidDispenser>(); // Assuming there's only one liquid dispenser in the scene
                Debug.Log("Liquid Dispenser found: " + liquidDispenser.gameObject.name);
                var action = new Action(liquidDispenser.gameObject, "fills-in", this.gameObject);
                EventBus.GetInstance().Publish(action);
                ECAScript.NotifyUpdate(this,
                    action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            }
        }
    }
}

/// <summary>
/// <b>FillStep</b> represents a discrete fill level within the container,
/// including the fill level integer and the transform marking the position in 3D space.
/// </summary>
[System.Serializable]
public class FillStep
{
    public int level;
    public Transform levelTransform;
}