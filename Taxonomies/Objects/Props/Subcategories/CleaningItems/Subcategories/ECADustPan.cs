using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.CleaningItems.Subcategories
{
    /// <summary>
    /// <b>ECADustPan</b> is a component that represents a virtual dustpan used in cleaning tasks within the environment.
    /// It interacts with objects equipped with an <see cref="ECADustBall"/> component and allows the collection and containment
    /// of dust balls that have been previously removed or swept by other cleaning tools.
    /// </summary>
    [ECARules4All("dustPan")]
    [RequireComponent(typeof(ECACleaningItem))]
    [DisallowMultipleComponent]
    public class ECADustPan : MonoBehaviour
    {
        [SerializeField] private Transform dustSpawnPoint; // The spawn point above the pan (assign in Inspector)
        [SerializeField] private List<GameObject> dustBallPool; // Pool of dust balls to activate one by one
        private int currentIndex = 0;
        
        /// <summary>
        /// <b>collects-dust</b> simulates the action of a dustpan collecting a dust ball from an object equipped
        /// with the <see cref="ECADustBall"/> component, generally a surface within the environment.
        /// This method is typically triggered after a <b>sweeps</b> action performed by an object equipped with an
        /// <see cref="ECABroom"/> component, often in combination with the <b>remove-dust</b> action
        /// of an <see cref="ECADustBall"/> component.
        /// When executed, it transfers the dust ball into the dustpan, updating its collected state
        /// </summary>
        /// <param name="dustBall">The <see cref="ECADustBall"/> object being collected by the dustpan.</param>
        [ECARelevance(true)]
        [Action(typeof(ECADustPan), "collects-dust", typeof(ECADustBall))]
        public void CollectsDust(ECADustBall dustBall)
        {
            Debug.Log(this.gameObject + " collects-dust from " + dustBall.gameObject.name);
            LogicShowNextDustBall();
        }

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
                Log.Warning("No more dust balls available in the pool.");
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
