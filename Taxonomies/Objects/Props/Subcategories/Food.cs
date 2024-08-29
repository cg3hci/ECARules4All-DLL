using System;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories
{
    /// <summary>
    /// <b>Food</b> is a class that represents something that can be eaten.
    /// </summary>
    [ECARules4All("food")]
    [RequireComponent(typeof(Prop))]
    [DisallowMultipleComponent]
    public class Food : ECAScript
    {
        /// <summary>
        /// <b>Weight</b>: is the weight of the food.
        /// </summary>
        [StateVariable("weight", ECARules4AllType.Float)]
        public float weight
        {
            get => _weight;
            set
            {
                _weight = value;
                NotifyUpdate(nameof(weight), weight.ToString());
            }
        }
        [SerializeField]
        private float _weight;

        /// <summary>
        /// <b>Expiration</b>: is the expiration date of the food.
        /// </summary>
        [StateVariable("expiration", ECARules4AllType.Time)]
        public DateTime expiration
        {
            get => _expiration;
            set
            {
                _expiration = value;
                NotifyUpdate(nameof(expiration), expiration.ToString());
            }
        }
        [SerializeField]
        private DateTime _expiration = DateTime.MaxValue;

        /// <summary>
        /// <b>Description</b>: is the description of the food.
        /// </summary>
        [StateVariable("description", ECARules4AllType.Text)]
        public string description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyUpdate(nameof(description), description);
            }
        }
        [SerializeField]
        private string _description;

        /// <summary>
        /// <b>Eaten</b>: is true if the food has been eaten.
        /// </summary>
        [StateVariable("eaten", ECARules4AllType.Boolean)]
        public bool eaten
        {
            get => _eaten;
            set
            {
                _eaten = value;
                NotifyUpdate(nameof(eaten), eaten.ToString());
            }
        }
        [SerializeField]
        private bool _eaten;

        /// <summary>
        /// <b>_Eats</b> is the method that is called when the food is eaten. This is a passive action, so the Food type
        /// is not in the subject of the action, but on the object.
        /// </summary>
        /// <param name="c">The character that eats the food</param>
        [Action(typeof(Character), "eats", typeof(Food))]
        public void _Eats(Character c)
        {
            if (expiration > DateTime.Now)
            {
                //GetComponent<ECAObject>().isActive.Assign(ECABoolean.BoolType.NO);
                GetComponent<ECAObject>().isActive = new ECABoolean(ECABoolean.BoolType.NO);
                GetComponent<ECAObject>().UpdateVisibility();
                eaten = true;
            }
        }
    }
}