
using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Systems
{
    /// <summary>
    /// <b>ECASystem</b> represents the virtual system within the automation system.
    /// It notifies when the virtual system starts, allowing the execution of rules at the start of the application.
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
            Debug.Log("I'm ready to start up the eca system");
            StartsUpTest();
            
            var action = new Action(this.gameObject, "starts-up");
            
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            
            Debug.Log("I have started up the eca system");
        }

        /// <summary>
        /// <b>StartsUpTest</b> is a method that triggers the startup process of the system.
        /// It can be used as trigger for automations.
        /// </summary>
        [Action(typeof(ECASystem), "starts-up")]
        [ECARelevance(true)]
        public void StartsUpTest()
        {
            Debug.Log("StartsUpTest ColdWater");
        }
    }

}
