using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// TODO ADD
    /// </summary>
    [ECARules4All("dustPan")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECADustPan : MonoBehaviour
    {

        /// <summary>
        /// TODO.
        /// </summary>
        [ECARelevance(true)]
        [Action(typeof(ECADustPan), "collects-dust", typeof(ECADustBall))]
        public void CollectsDust(ECADustBall dustBall)
        {
            Debug.Log(this.gameObject + " collects-dust from " + dustBall.gameObject.name);
            LogicShowNextDustBall();
        }


        // [SerializeField] private GameObject dustBallPrefab;        // Assign in Inspector
        [SerializeField] private Transform dustSpawnPoint; // Assign the spawn point above the pan

        [SerializeField] private List<GameObject> dustBallPool;
        private int currentIndex = 0;

        void Awake()
        {
            // Create and disable all dust balls
            foreach (var dustBall in dustBallPool)
                dustBall.SetActive(false);
        }

        private void LogicShowNextDustBall()
        {
            if (currentIndex < dustBallPool.Count)
            {
                var nextDust = dustBallPool[currentIndex];
                nextDust.SetActive(true);
                currentIndex++;
            }
            else
            {
                Debug.LogWarning("No more dust balls available in the pool.");
            }
        }

        private void OnGUI()
        {
            // make the button top right
            if (GUILayout.Button("LogicShowNextDustBall", GUILayout.Width(150), GUILayout.Height(50)))
            {
                LogicShowNextDustBall();
            }
        }
    }
}
