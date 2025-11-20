using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Utils
{
    /// <summary>
    /// </summary>
    public class ECALiquidDrop : MonoBehaviour
    {
        public bool doDestroyOnCollision = true; // Option to destroy on collision
        public bool doDestroyAfterLongDelay = true; // Option to destroy after a delay
        public float destroyDelay = 5f; // Time before destruction if not collided
        public ECALiquidDispenser owner = null; // Reference to the owner liquid dispenser
        public float temperature = 20f; // Temperature of the liquid drop in Celsius

        //TODO Aggiungere enum for LiquidType
        private void Start()
        {
            if (doDestroyAfterLongDelay)
            {
                Destroy(gameObject, destroyDelay);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (doDestroyOnCollision)
            {
                //TODO J 08-05-25 Let's see if we need this
                // // Check if the other object has the WaterDrop component
                // if (collision.gameObject.GetComponent<WaterLiquid>() != null)
                // {
                //     // It's another water drop — do nothing
                //     return;
                // }

                // Otherwise, destroy this water drop
                Destroy(gameObject);
            }
        }
    }
}