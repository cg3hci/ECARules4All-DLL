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
    public class Switch : MonoBehaviour
    {
        /// <summary>
        /// <b>On</b> is the state of the switch.
        /// </summary>
        [StateVariable("on", ECARules4AllType.Boolean)]
        [ECARelevance(false)]
        public ECABoolean on
        {
            get => _on;
            set
            {
                _on = value;
                ECAScript.NotifyUpdate(this, nameof(on), on.ToString());
            }
        }
        [SerializeField] 
        private ECABoolean _on;

        /// <summary>
        /// <b>Turns</b> defines if the switch is on or off.
        /// </summary>
        /// <param name="on">The new state of the switch.</param>
        [Action(typeof(Switch), "turns", typeof(ECABoolean))]
        [ECARelevance(false)]
        public void Turns(ECABoolean on)
        {
            this.on = on;
        }
        
        /// <summary>
        /// <b>Toggle status</b> toggles the switch state.
        /// </summary>
        [Action(typeof(Switch), "toggle status")]
        [ECARelevance(false)]
        public void Toggle()
        {
            this.on = ECABoolean.Invert(this.on);
        }
    }
}