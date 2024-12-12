using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scenes_Scene = ECARules4All_DLL.Taxonomies.Objects.Scenes.Scene;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    /// <summary>
    /// <b>Transition</b> is a <see cref="Behaviour"/> that is used to trigger a transition to another scene.
    /// </summary>
    [ECARules4All("transition")]
    [RequireComponent(typeof(Behaviour))]
    [DisallowMultipleComponent]
    public class Transition : MonoBehaviour
    {
        /// <summary>
        /// <b>Reference</b> is the Unity Scene to transition to.
        /// </summary>
        [StateVariable("reference", ECARules4AllType.Identifier)]
        [ECARelevance(false)]
        public Scenes_Scene reference
        {
            get => _references;
            set
            {
                _references = value;
                ECAScript.NotifyUpdate(this, nameof(reference), reference.ToString());
            }
        }
        [SerializeField]
        private Scenes_Scene _references;

        /// <summary>
        /// <b>Teleports</b> changes the current scene to the scene referenced by <see cref="reference"/>.
        /// </summary>
        /// <param name="reference"></param>
        [Action(typeof(Transition), "teleports to", typeof(Scenes_Scene))]
        [ECARelevance(false)]
        public void Teleports(Scenes_Scene reference)
        {
            if (reference.name != SceneManager.GetActiveScene().name)
            {
                SceneManager.LoadScene(reference.name);
            }

            //DOUBT: come identificare il giocatore nella scena? è giusto che sia Transition e non ECAobject?
            //Giocatore.posizione = reference.position.GetPosition();
            //TODO: test per controllare che la scena esista
        }
    }
}