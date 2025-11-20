using System;
using ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Utils
{
    /// <summary>
    /// </summary>
    public class ECALiquidSpawner : MonoBehaviour
    {
        public ECALiquidDispenser liquidDispenser = null; // Reference to the liquid dispenser component

        public enum LiquidType
        {
            Water,
            Amuchina,
            BatteryKiller,
            Degreaser
        }

        public enum LiquidTemperature
        {
            Cold,
            Ambient,
            Warm
        }

        [SerializeField] private LiquidType liquidType; // You are not allowed to change this outside the inspector
        public LiquidType GetLiquidType() => liquidType; // Public getter for the liquid type
        private LiquidTemperature _liquidTemperature;


        private bool _doSpawnLiquid = false;


        public void SpawnLiquid(LiquidTemperature liquidTemperature)
        {
            Debug.Log("SpawnLiquid1");
            _liquidTemperature = liquidTemperature;
            Debug.Log("SpawnLiquid2");
            _doSpawnLiquid = true;
            Debug.Log("SpawnLiquid3");
        }

        public void StopSpawningLiquid() => _doSpawnLiquid = false;

        [SerializeField] private GameObject dropPrefabTempCold;
        [SerializeField] private GameObject dropPrefabTempAmbient;
        [SerializeField] private GameObject dropPrefabTempWarm;


        private float _randomMax = 0.2f;
        private const float _counter = 3;
        private float i = 0;
        [SerializeField] private Transform spawnDirection;

        private void Awake()
        {
            if (spawnDirection == null)
            {
                throw new Exception("SpawnDirection is not assigned in the WaterSpawner script.");
            }
        }

        private void Update()
        {
            if (_doSpawnLiquid)
            {
                i++;

                if (i < _counter)
                {
                    return;
                }

                i = 0;


                // Offset spawn position along the forward direction of spawnDirection
                Vector3 forwardOffset = spawnDirection.forward * UnityEngine.Random.Range(0f, _randomMax);
                Vector3 randomPosition = transform.position + forwardOffset;

                // Instantiate the water drop prefab at the random position
                GameObject waterDrop = null;
                ECALiquidDrop waterDropRef;
                if (_liquidTemperature == LiquidTemperature.Cold)
                {
                    waterDrop = Instantiate(dropPrefabTempCold, randomPosition, Quaternion.identity);
                    waterDropRef = waterDrop.GetComponent<ECALiquidDrop>();
                    waterDropRef.temperature = 10f;
                }
                else if (_liquidTemperature == LiquidTemperature.Ambient)
                {
                    waterDrop = Instantiate(dropPrefabTempAmbient, randomPosition, Quaternion.identity);
                    waterDropRef = waterDrop.GetComponent<ECALiquidDrop>();
                    waterDropRef.temperature = 20f;
                }
                else if (_liquidTemperature == LiquidTemperature.Warm)
                {
                    waterDrop = Instantiate(dropPrefabTempWarm, randomPosition, Quaternion.identity);
                    waterDropRef = waterDrop.GetComponent<ECALiquidDrop>();
                    waterDropRef.temperature = 30f;
                }
                else throw new Exception("Unknown liquid temperature: " + _liquidTemperature);

                // Set the parent of the water drop to this object
                waterDrop.transform.SetParent(transform);
                waterDropRef.owner = liquidDispenser; // Set the owner reference

                // Apply velocity in the direction of spawnDirection
                Rigidbody rb = waterDrop.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false; // Disable gravity
                    rb.velocity = spawnDirection.forward * 9f; // Set speed
                }
            }
        }

        //TODO Destroy water drop after some time
        // On collision, detroy this object
        // private void OnCollisionEnter(Collision collision)
        // {
        //     if (collision.gameObject.CompareTag("WaterDrop"))
        //     {
        //         Destroy(collision.gameObject);
        //     }
        // }
    }
}