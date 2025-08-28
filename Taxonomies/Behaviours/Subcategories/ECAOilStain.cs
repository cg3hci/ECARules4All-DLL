using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>ECAOilStain</b> is a <see cref="ECABehaviour">ECABehaviour</see> that attaches oil stains to a virtual object, ideally a  <see cref="ECASurface">Surface</see>.
    /// It can be "washed" using other objects such as mops or cleaning rags, and define the number of washes needed until all stains are removed.
    /// Once fully washed, the object updates its state and optionally plays audio feedback.
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
        /// <b>washesCounter</b> defines how many times the oil stains needs to be washed before it's considered clean.
        /// Each wash reduces this counter until it reaches zero, triggering a "fully washed" state.
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
        /// <b>allWashed</b> indicates whether all the stains have been successfully removed from the object.
        /// It becomes true once the washes counter reaches zero.
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
        /// <b>Changes</b> the value of the washes counter to a specified integer.
        /// This method ensures the new value is not negative and updates the internal wash logic accordingly.
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
        /// <b>increasingly-removes-stain</b> simulates a washing action by a <see cref="ECACleaningRag"/>, decreasing by one the number of washes needed.
        /// When enough washes are performed, the oil stains are considered clean.
        /// </summary>
        /// <param name="cleaningRag">The cleaning rag object performing the wash.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACleaningRag), "increasingly-removes-stain", typeof(ECAOilStain))]
        public void _IncreasinglyWashes(ECACleaningRag cleaningRag)
        {
            Debug.Log("Washes by one rag. Cleaning rag: " + cleaningRag.gameObject.name);
            InternLogicIncreasinglyWashes();
        }

        /// <summary>
        /// <b>increasingly-removes-stain</b> simulates a washing action by a <see cref="ECAMop"/>, decreasing by one the number of washes needed.
        /// When enough washes are performed, the oil stains are considered clean.
        /// </summary>
        /// <param name="mop">The mop object performing the wash.</param>
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
                    Debug.LogWarning("There are no oil stains.");
                }
                else
                {
                    bool foundOilStain = false;
                    foreach (var t in _oilStains)
                    {
                        if (t != null && t.activeInHierarchy)
                        {
                            Debug.Log("Found an oil stain to wash: " + t.name);

                            // 2. Update the material/remove the dirty part: e.g.
                            //      other.gameObject.GetComponent<Renderer>().material = CleanedCellMaterial;
                            t.SetActive(false); // Example of cleaning the oil stain

                            foundOilStain = true;
                            break; // Exit after cleaning one oil stain
                        }
                    }

                    if (!foundOilStain)
                    {
                        Debug.LogWarning("No active oil stains found to wash.");
                    }
                }
            }
        }
    }
}