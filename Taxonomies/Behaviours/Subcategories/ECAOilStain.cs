using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Objects.Environments.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>ECAOilStain</b> is a component that attaches oil stains to a virtual object, ideally a surface equipped with an <see cref="ECASurface"/> component.
    /// It can be 'washed' using other objects such as mops or cleaning rags.
    /// Once fully washed, the object updates its state.
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

        /*private int _washesNeeded = 0;

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

        [SerializeField] private int _washesCounter = 1;*/

        /// <summary>
        /// <b>allWashed</b> indicates whether all the stains have been successfully removed from the object.
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


        /*/// <summary>
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
        }*/
        
        /// <summary>
        /// <b>removes-stain</b> defines the sweeping action performed by an object that has an <see cref="ECACleaningRag"/> component.
        /// When the action is executed, the stains are removed. Typically, this is the result of a <b>washes</b> event performed by an object that has an <see cref="ECACleaningRag"/>.
        /// </summary>
        /// <param name="cleaningRag">The object that has a <see cref="ECACleaningRag"/> component responsible for performing the washing action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECACleaningRag), "removes-stain", typeof(ECAOilStain))]
        public void _IncreasinglyWashes(ECACleaningRag cleaningRag)
        {
            Debug.Log("Washes by one rag. Cleaning rag: " + cleaningRag.gameObject.name);
            InternLogicIncreasinglyWashes();
        }

        /// <summary>
        /// <b>removes-stain</b> defines the sweeping action performed by an object equipped with an <see cref="ECAMop"/> component.
        /// When the action is executed, the stains are removed. Typically, this is the result of a <b>washes</b> event performed by an object that has an <see cref="ECAMop"/>..
        /// </summary>
        /// <param name="mop">The object equipped with an ECAMop component responsible for performing the washing action.</param>
        [ECARelevance(true)]
        [Action(typeof(ECAMop), "removes-stain", typeof(ECAOilStain))]
        public void _IncreasinglyWashes(ECAMop mop)
        {
            Debug.Log("Washes by one mop. Mop: " + mop.gameObject.name);
            InternLogicIncreasinglyWashes();
        }

        private void InternLogicIncreasinglyWashes()
        {
            //_washesNeeded--;
            if (audioSourceWash != null)
            {
                audioSourceWash.Play();
            }
            
            /*if (_washesNeeded <= 0)
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
            }*/
            
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
    }
}