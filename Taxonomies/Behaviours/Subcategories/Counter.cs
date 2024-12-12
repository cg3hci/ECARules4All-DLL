using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Counter</b> is a <see cref="Behaviour">Behaviour</see> that enables the object to keep track of countable events
    /// <example> Player steps, interaction count</example>
    /// </summary>
    [ECARules4All("counter")]
    [RequireComponent(typeof(Behaviour))]
    [DisallowMultipleComponent]
    public class Counter : MonoBehaviour
    {
        /// <summary>
        /// <b>count</b> is the current count of the counter
        /// </summary>
        [StateVariable("count", ECARules4AllType.Float)]
        public float count
        {
            get => _count;
            set
            {
                _count = value;
                ECAScript.NotifyUpdate(this, nameof(count), count.ToString());
            }
        }
        [SerializeField] private float _count;

        /// <summary>
        /// <b>Changes</b> changes the count of the counter
        /// </summary>
        /// <param name="amount"> the amount to set </param>
        [Action(typeof(Counter), "changes","count", "to", typeof(float))]
        public void ChangesCount(float amount)
        {
            count = amount;
        }
    }
}