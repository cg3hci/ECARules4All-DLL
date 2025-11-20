
using System.Collections;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Systems
{
    /// <summary>
    /// <b>ECASystem</b> is a component that represents the virtual automation system within the environment.
    /// It handles system-level initialization events and notifies when the virtual system starts,
    /// enabling the execution of ECA rules and actions defined to occur at application startup.
    /// </summary>
    [ECARules4All("system")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class ECASystem : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(StartUpCoroutine());
        }
        
        private IEnumerator StartUpCoroutine() 
        {
            // Wait for the ECAObject to be created and initialized
            yield return new WaitForSeconds(0.1f); 
            Log.Debug("I'm ready to start up the eca system");
            StartsUpTest();
            
            var action = new Action(this.gameObject, "starts up");
            
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            
            Log.Debug("I have started up the eca system");
        }

        /// <summary>
        /// <b>starts up</b> represents the action that triggers the startup process of the virtual system.
        /// When executed, it signals that the system has initialized successfully and can serve as a trigger
        /// for ECA automation rules that should run at application startup.
        /// </summary>
        [Action(typeof(ECASystem), "starts up")]
        [ECARelevance(true)]
        public void StartsUpTest()
        {
            Log.Debug("StartsUpTest ColdWater");
        }
    }

}
