using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// blabla
    /// </summary>
    [ECARules4All("oilStain")]
    [RequireComponent(typeof(ECABehaviour))]
    [DisallowMultipleComponent]
    public class ECAOilStain : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourceAllWashed;
        [SerializeField] private AudioSource audioSourceWash;
        [SerializeField] private List<GameObject> _oilStains;

        [Range(0, 100)] private int _percentage = 0;

        private int _washesNeeded = 0;

        /// <summary>
        /// <b>washesCounter</b> TODO.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("washesCounter", ECARules4AllType.Integer)]
        public int washesCounter
        {
            get => _washesCounter;
            set
            {
                _washesCounter = value;
                _washesNeeded = value;
                ECAScript.NotifyUpdate(this, nameof(washesCounter), washesCounter.ToString());
            }
        }

        [SerializeField] private int _washesCounter = 1;

        /// <summary>
        /// <b>allWashed</b> TODO.
        /// </summary>
        [ECARelevance(true)]
        [StateVariable("allWashed", ECARules4AllType.Boolean)]
        public ECABoolean allWashed
        {
            get => _allWashed;
            set
            {
                _allWashed = value;
                ECAScript.NotifyUpdate(this, nameof(allWashed), allWashed.ToString());
            }
        }

        [SerializeField] private ECABoolean _allWashed = new ECABoolean(ECABoolean.BoolType.NO);


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="v">The new counter value.</param>
        [Action(typeof(ECAOilStain), "changes", "washesCounter", "to", typeof(int))]
        [ECARelevance(true)]
        public void ChangesWashesCounter(int v)
        {
            if (v < 0)
            {
                v = 0;
            }

            //
            this.washesCounter = v;
        }


        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECACleaningRag), "increasingly-removes-stain", typeof(ECAOilStain))]
        public void _IncreasinglyWashes(ECACleaningRag cleaningRag)
        {
            Debug.Log("Washes by one rag. Cleaning rag: " + cleaningRag.gameObject.name);
            InternLogicIncreasinglyWashes();
        }

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECAMop), "increasingly-removes-stain", typeof(ECAOilStain))]
        public void _IncreasinglyWashes(ECAMop mop)
        {
            Debug.Log("Washes by one mop. Mop: " + mop.gameObject.name);
            InternLogicIncreasinglyWashes();
        }

        private void InternLogicIncreasinglyWashes()
        {
            _washesNeeded--;
            if (audioSourceWash != null)
            {
                audioSourceWash.Play();
            }

            if (_washesNeeded <= 0)
            {
                if (audioSourceAllWashed != null)
                {
                    audioSourceAllWashed.Play();
                }

                allWashed = ECABoolean.YES;
                Debug.Log("All washed!");

                //TODO Deactivate all oil stains
                foreach (var oilStain in _oilStains)
                {
                    if (oilStain != null)
                    {
                        oilStain.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("Washes needed: " + _washesNeeded);
                //TODO Make the logic
                // 1. Search for the first oil stain active
                if (_oilStains.Count == 0)
                {
                    Debug.LogWarning("There are no dusty balls.");
                }
                else
                {
                    bool foundDustyBall = false;
                    foreach (var t in _oilStains)
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