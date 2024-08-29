using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Lock</b> is a <see cref="Behaviour"/> that locks the <see cref="ECAObject"/> it is attached to.
    /// It works in a similar way to the <see cref="Keypad"/> behaviour, but it needs to by unlock by other means (like a key).
    /// </summary>
    [ECARules4All("lock")]
    [RequireComponent(typeof(Behaviour))]
    [DisallowMultipleComponent]
    public class Lock : ECAScript
    {
        /// <summary>
        /// <b>locked</b> defines whether the lock is open or not.
        /// </summary>
        [StateVariable("locked", ECARules4AllType.Boolean)]
        public ECABoolean locked
        {
            get => _locked;
            set
            {
                _locked = value;
                NotifyUpdate(nameof(locked), locked.ToString());
            }
        }
        [SerializeField] 
        private ECABoolean _locked = new ECABoolean(ECABoolean.BoolType.YES);

        /// <summary>
        /// <b>Opens</b> sets the lock to open.
        /// </summary>
        [Action(typeof(Lock), "opens")]
        public void Opens()
        {
            //locked.Assign(ECABoolean.BoolType.NO);
            locked = new ECABoolean(ECABoolean.BoolType.NO);
        }
        
        /// <summary>
        /// <b>Closes</b> sets the lock to closed.
        /// </summary>
        [Action(typeof(Lock), "closes")]
        public void Closes()
        {
            //locked.Assign(ECABoolean.BoolType.YES);
            locked = new ECABoolean(ECABoolean.BoolType.YES);
        }
    }
}