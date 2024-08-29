using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.Weapons.Subcategories
{
    /// <summary>
    /// <b>Bullet</b>: this class it is a type of <see cref="Weapon"/> that is usually expelled from another object in the scene, usually a <see cref="Firearm"/> object
    /// </summary>
    [ECARules4All("bullet")]
    [RequireComponent(typeof(Weapon))]
    [DisallowMultipleComponent]
    public class Bullet : ECAScript
    {
        /// <summary>
        /// <b>Speed</b>: this is the speed of the bullet
        /// </summary>
        [StateVariable("speed", ECARules4AllType.Float)]
        public float speed
        {
            get => _speed;
            set
            {
                _speed = value;
                NotifyUpdate(nameof(speed), speed.ToString());
            }
        }
        [SerializeField]
        private float _speed;
        //TODO FUTURE: eventualmente avere un'evento per quando colpisce qualcosa


        private void OnCollisionEnter(Collision other)
        {
            Destroy(gameObject);
        }
    }
}