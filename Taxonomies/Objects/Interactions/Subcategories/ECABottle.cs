using System.Diagnostics;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using UnityEngine;
using ECARules4All_DLL.Utils;
using Debug = UnityEngine.Debug;

namespace ECARules4All_DLL.Taxonomies.Objects.custom
{   
    [ECARules4All("bottle")]
    [RequireComponent(typeof(ECAObject))]
    [RequireComponent(typeof(Interaction))]
    [RequireComponent(typeof(Interactable))]
    [DisallowMultipleComponent]
    public class ECABottle : MonoBehaviour
    {
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
            Debug.Log("Increase level of the Bottle to " + Level);
        }
        
        [ECARelevance(true)]
        [Action(typeof (ECABottle), "decrease", "level", "by", typeof(int))]
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
            Debug.Log("Decrease level of the Bottle to " + Level);

        }
        [ECARelevance(true)]
        [Action(typeof(ECABottle), "open cap")]
        public void OpenCap()
        {
            if (IsCapped == ECABoolean.NO) return;
            IsCapped = ECABoolean.NO;
            Debug.Log("the cap is open");
        }
        [ECARelevance(true)]
        [Action(typeof (ECABottle), "close cap")]
        public void CloseCap()
        {
            if (IsCapped == ECABoolean.YES) return;
            IsCapped = ECABoolean.YES;
            Debug.Log("the cap is closed");
        }
    }
}