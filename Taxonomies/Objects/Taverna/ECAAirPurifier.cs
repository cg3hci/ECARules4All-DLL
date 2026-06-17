using ECARules4All_DLL.Taxonomies.Objects.Interactions;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Taverna
{
    [ECARules4All("airpurifier")]
    [RequireComponent(typeof(ECAInteraction))]
    [DisallowMultipleComponent]
    public class ECAAirPurifier : ECAObject
    {
        [ECARelevance(true)]
        [StateVariable("isPoweredOn", ECARules4AllType.Boolean)]
        public ECABoolean isPoweredOn
        {
            get => _isPoweredOn;
            set
            {
                _isPoweredOn = value;
                ECAScript.NotifyUpdate(this, nameof(isPoweredOn), isPoweredOn.ToString());
            }
        }
        [SerializeField]
        private ECABoolean _isPoweredOn = new ECABoolean(ECABoolean.BoolType.OFF);

        [StateVariable("fanSpeed", ECARules4AllType.Integer)]
        public int fanSpeed
        {
            get => _fanSpeed;
            set
            {
                _fanSpeed = value;
                ECAScript.NotifyUpdate(this, nameof(fanSpeed), fanSpeed.ToString());
            }
        }
        [SerializeField]
        private int _fanSpeed = 0;

        /// Attiva il purificatore d'aria.
        [ECARelevance(true)]
        [Action(typeof(ECAAirPurifier), "turnOn")]
        public void TurnOn()
        {
            isPoweredOn = new ECABoolean(ECABoolean.BoolType.ON);
            fanSpeed = fanSpeed==0? 1: fanSpeed; // Imposta una velocità minima di avvio se era a 0
        }

        /// Spegne il purificatore d'aria e azzera la velocità della ventilatore
        [ECARelevance(true)]
        [Action(typeof(ECAAirPurifier), "turnOff")]
        public void TurnOff()
        {
            isPoweredOn = new ECABoolean(ECABoolean.BoolType.OFF);
            fanSpeed = 0;
        }
        
        /// Imposta una velocità specifica per la ventola.
        [Action(typeof(ECAAirPurifier), "sets", "speed", "to", typeof(int))]
        public void SetSpeed(int speed)
        {
            fanSpeed = speed;
            isPoweredOn = new ECABoolean(fanSpeed > 0 ? ECABoolean.BoolType.ON : ECABoolean.BoolType.OFF);
        }
    }
}