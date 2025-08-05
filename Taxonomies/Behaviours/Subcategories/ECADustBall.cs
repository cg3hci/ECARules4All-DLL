using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>dustBall</b> is a <see cref="ECABehaviour">Behaviour</see> that represents dust on surfaces.
    /// </summary>
    [ECARules4All("dustBall")]
    [RequireComponent(typeof(ECABehaviour))]
    [DisallowMultipleComponent]
    public class ECADustBall : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourceAllSwept;
        [SerializeField] private AudioSource audioSourceSweep;
        [SerializeField] List<GameObject> _dustyBalls;

        private int _sweepsNeeded = 0;
        
        /// <summary>
        /// <b>waterDrops</b> TODO.
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

        [SerializeField] private int _sweepsCounter = 1;

        /// <summary>
        /// <b>allSwept</b> TODO.
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

        /// <summary>
        /// TODO
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
        }

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAScottex), "increasingly-removes-dust", typeof(ECADustBall))]
        public void _IncreasinglySweeps(ECAScottex scottex)
        {
            Debug.Log("Sweep by one scottex: " + scottex.gameObject.name);
            InternLogicIncreasinglySweeps();
        }

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECABroom), "increasingly-removes-dust", typeof(ECADustBall))]
        public void _IncreasinglySweeps(ECABroom broom)
        {
            Debug.Log("Sweep by one broom: " + broom.gameObject.name);
            InternLogicIncreasinglySweeps();
        }

        private void InternLogicIncreasinglySweeps()
        {
            _sweepsNeeded--;
            if (audioSourceSweep != null)
            {
                audioSourceSweep.Play();
            }

            if (_sweepsNeeded <= 0)
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
            }
        }
    }
}