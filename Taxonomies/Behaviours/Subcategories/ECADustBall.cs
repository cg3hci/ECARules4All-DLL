using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{

    /// <summary>
    /// The EcaDustBall component represents the presence of dust on a virtual object, typically a surface equipped with an see <see cref="ECASurface" /> component.
    /// Dust can be swept away using specialized objects such as brooms, rags, or scottex equipped with an (<see cref="ECACleaningItem"/>) component.
    /// After being swept, the object updates its state through the `allSwept` property.
    /// </summary>
    [ECARules4All("dustBall")]
    [RequireComponent(typeof(ECABehaviour))]
    [DisallowMultipleComponent]
    public class ECADustBall : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourceAllSwept;
        [SerializeField] private AudioSource audioSourceSweep;
        [SerializeField] List<GameObject> _dustyBalls;

        /*private int _sweepsNeeded = 0;
        
        /// <summary>
        /// <b>sweepsCounter</b> defines how many times the dust ball needs to be swept before it's considered clean.
        /// Each sweep reduces this counter until it reaches zero, triggering a "fully swept" state.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("sweepsCounter", ECARules4AllType.Integer)]
        public int sweepsCounter
        {
            get => _sweepsCounter;
            set
            {
                _sweepsCounter = value;
                _sweepsNeeded = value;
                ECAScript.NotifyUpdate(this, nameof(sweepsCounter), sweepsCounter.ToString());
            }
        }

        [SerializeField] private int _sweepsCounter = 1;*/

        /// <summary>
        /// <b>allSwept</b> indicates whether all the dust has been successfully removed from the object.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("allSwept", ECARules4AllType.Boolean)]
        public ECABoolean allSwept
        {
            get => _allSwept;
            set
            {
                _allSwept = value;
                ECAScript.NotifyUpdate(this, nameof(allSwept), allSwept.ToString());
            }
        }

        [SerializeField] private ECABoolean _allSwept = new ECABoolean(ECABoolean.BoolType.NO);

        /*/// <summary>
        /// <b>Changes</b> the value of the sweeps counter to a specified integer.
        /// This method ensures the new value is not negative and updates the internal sweep logic accordingly.
        /// </summary>
        /// <param name="v">The new counter value.</param>
        [Action(typeof(ECADustBall), "changes", "sweepsCounter", "to", typeof(int))]
        [ECARelevance(true)]
        public void ChangesSweepsCounter(int v)
        {
            if (v < 0)
            {
                v = 0;
            }

            //
            this.sweepsCounter = v;
        }*/

        /// <summary>
        /// <b>removes-dust-with-scottex</b> defines the sweeping action performed by an object equipped with an <see cref="ECAScottex"/> component.
        /// When the action is executed, the dust balls are removed. Typically, this is the result of a <b>wipes</b> event performed by an object equipped with an <see cref="ECAScottex"/>.
        /// </summary>
        /// <param name="scottex">The object equipped with an ECAScottex component responsible for performing the sweeping action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAScottex), "removes-dust-with-scottex", typeof(ECADustBall))]
        public void _IncreasinglySweeps(ECAScottex scottex)
        {
            Log.Debug("Removes dust by scottex: " + scottex.gameObject.name);
            InternLogicIncreasinglySweeps();
        }

        /// <summary>
        /// <b>removes-dust-with-broom</b> defines the sweeping action performed by an object that has an ECABroom component.
        /// When the action is executed, the dust balls are removed. Typically, this is the result of a <b>washes</b>
        /// event performed by an object that has an ECABroom.
        /// After removal, nearby objects equipped with an ECADustPan component may automatically execute
        /// their <b>collects-dust</b> action to gather the detached dust ball.
        /// </summary>
        /// <param name="broom">The object that has a ECABroomcomponent responsible for performing the sweeping action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECABroom), "removes-dust-with-broom", typeof(ECADustBall))]
        public void _IncreasinglySweeps(ECABroom broom)
        {
            Log.Debug("Removes dust by broom: " + broom.gameObject.name);
            InternLogicIncreasinglySweeps();
        }

        private void InternLogicIncreasinglySweeps()
        {
            //_sweepsNeeded--;
            if (audioSourceSweep != null)
            {
                audioSourceSweep.Play();
            }
        
            /*if (_sweepsNeeded <= 0)
            {
                if (audioSourceAllSwept != null)
                {
                    audioSourceAllSwept.Play();
                }

                allSwept = ECABoolean.YES;
                Debug.Log("All Swept!");

                // deactivate all dusty balls
                foreach (var dustyBall in _dustyBalls)
                {
                    if (dustyBall != null)
                    {
                        dustyBall.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("Sweeps needed: " + _sweepsNeeded);

                //TODO Make the logic
                // 1. Search for the first dirty ball active
                if (_dustyBalls.Count == 0)
                {
                    Debug.LogWarning("There are no dusty balls.");
                }
                else
                {
                    bool foundDustyBall = false;
                    foreach (var t in _dustyBalls)
                    {
                        if (t != null && t.activeInHierarchy)
                        {
                            Debug.Log("Found a dusty ball to sweep: " + t.name);

                            // 2. Update the material/remove the dirty part: e.g.
                            //      other.gameObject.GetComponent<Renderer>().material = CleanedCellMaterial;
                            t.SetActive(false); // Example of cleaning the ball

                            foundDustyBall = true;
                            break; // Exit after cleaning one dusty ball
                        }
                    }

                    if (!foundDustyBall)
                    {
                        Debug.LogWarning("No active dusty balls found to sweep.");
                    }
                }
            }*/
            
            allSwept = ECABoolean.YES;
            Log.Debug("All Swept!");

            // deactivate all dusty balls
            foreach (var dustyBall in _dustyBalls)
            {
                if (dustyBall != null)
                {
                    dustyBall.SetActive(false);
                }
            }
        }
    }
}