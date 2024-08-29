using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Switch</b> is a <see cref="Behaviour"/> that can be use to let an object have an on/off state, useful for
    /// objects like lights, doors, etc.
    /// </summary>
    [ECARules4All("switch")]
    [RequireComponent(typeof(Behaviour))]
    [DisallowMultipleComponent]
    public class Switch : ECAScript
    {
        /// <summary>
        /// <b>On</b> is the state of the switch.
        /// </summary>
        [StateVariable("on", ECARules4AllType.Boolean)]
        public ECABoolean on
        {
            get => _on;
            set
            {
                _on = value;
                NotifyUpdate(nameof(on), on.ToString());
            }
        }
        [SerializeField] 
        private ECABoolean _on;

        /// <summary>
        /// <b>Turns</b> defines if the switch is on or off.
        /// </summary>
        /// <param name="on">The new state of the switch.</param>
        [Action(typeof(Switch), "turns", typeof(ECABoolean))]
        public void Turns(ECABoolean on)
        {
            this.on = on;
        }
    }
}