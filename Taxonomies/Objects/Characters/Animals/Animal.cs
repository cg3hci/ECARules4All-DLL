using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Characters.Animals
{
    /// <summary>
    /// <b>Animal</b> is a subclass of Character. It is used to represent the animals character.
    /// </summary>
    [ECARules4All("animal")]
    [RequireComponent(typeof(Character))]
    [DisallowMultipleComponent]
    public class Animal : ECAScript
    {
        /// <summary>
        /// <b>Speaks</b>: allows to send a message to the player
        /// </summary>
        /// <param name="s">The string that recalls the audioclip</param>
        [Action(typeof(Animal), "speaks", typeof(string))]
        public void Speaks(string s)
        {
            AudioSource audio = this.gameObject.GetComponent<AudioSource>();
            AudioClip resource = (AudioClip)Resources.Load(s);
            if (resource != null)
            {
                audio.clip = resource;
                audio.Play();
            }
        }
    }
}