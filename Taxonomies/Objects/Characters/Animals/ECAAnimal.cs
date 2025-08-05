using System.Collections;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Characters.Animals
{
    /// <summary>
    /// Represents an animal character within the ECA rules framework. 
    /// An <b>Animal</b> is a specialized subclass of <see cref="ECACharacter"/> that embodies animal-like traits 
    /// and behaviors, enabling interactions and actions unique to animal entities.
    /// </summary>
    [ECARules4All("animal")]
    [RequireComponent(typeof(ECACharacter))]
    [DisallowMultipleComponent]
    public class ECAAnimal : MonoBehaviour
    {
        /// <summary>
        /// <b>Speaks</b> allows the animal to produce a sound or "speak" by playing an associated audio clip.
        /// The audio clip is identified by the provided string, which must correspond to a valid resource.
        /// </summary>
        /// <param name="s">The name of the audio resource to be played.</param>
        [Action(typeof(ECAAnimal), "speaks", typeof(string))]
        [ECARelevance(false)]
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