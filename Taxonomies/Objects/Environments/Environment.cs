using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Environments
{
    /// <summary>
    /// Defines a  TODO.
    /// </summary>
    [ECARules4All("environment")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Environment : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(StartUpCoroutine());
        }
        
        private IEnumerator StartUpCoroutine() 
        {
            // Wait for the ECAObject to be created and initialized
            yield return new WaitForSeconds(0.1f); 
            Debug.Log("I'm ready to start up the environment");
            StartsUpTest();
            
            var action = new Action(this.gameObject, "starts-up");
            
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
            
            Debug.Log("I have started up the environment");
        }

        /// <summary>
        /// TODO
        /// </summary>
        [Action(typeof(Environment), "starts-up")]
        [ECARelevance(true)]
        public void StartsUpTest()
        {
            Debug.Log("StartsUpTest ColdWater");
        }
    }

}
