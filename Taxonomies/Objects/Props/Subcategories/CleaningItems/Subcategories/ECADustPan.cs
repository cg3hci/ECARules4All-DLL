using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECADustPan</b> is a virtual object that simulates the behavior of a dustpan used in cleaning tasks.
    /// It has a method for collecting dust balls (<see cref="ECADustBall"/>).
    /// </summary>
    [ECARules4All("dustPan")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECADustPan : MonoBehaviour
    {

        /// <summary>
        /// <b>CollectsDust</b> is a method that simulates the action of the dustpan collecting a dust ball.
        /// </summary>
        /// <param name="dustBall">The <see cref="ECADustBall"/> object being collected.</param>
        [ECARelevance(true)]
        [Action(typeof(ECADustPan), "collects-dust", typeof(ECADustBall))]
        public void CollectsDust(ECADustBall dustBall)
        {
            Debug.Log(this.gameObject + " collects-dust from " + dustBall.gameObject.name);
            LogicShowNextDustBall();
        }

        [SerializeField] private Transform dustSpawnPoint; // The spawn point above the pan (assign in Inspector)
        [SerializeField] private List<GameObject> dustBallPool; // Pool of dust balls to activate one by one
        private int currentIndex = 0;

        void Awake()
        {
            // Ensures that no dust balls are visible until collection actions occur.
            foreach (var dustBall in dustBallPool)
                dustBall.SetActive(false);
        }

        private void LogicShowNextDustBall()
        {
            // Reveals the next inactive dust ball in the pool by activating it.
            // This method simulates dust being visually collected into the dustpan.
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

        // Testing purposes
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
