using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Scenes
{
    [ECARules4All("scene")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Scene : MonoBehaviour
    {
        //DOUBT: Aggiunto in questo file nome e posizione della scena di arrivo, può avere senso?
        //This component renames the GameObject to the Scene name, for "rule starting on start" purposes
        [StateVariable("name", ECARules4AllType.Text)]
        public string name
        {
            get => _name;
            set
            {
                _name = value;
                ECAScript.NotifyUpdate(this, nameof(name), name);
            }
        }
        [SerializeField]
        private string _name;

        [StateVariable("position", ECARules4AllType.Position)]
        public Position position
        {
            get => _position;
            set
            {
                _position = value;
                var v = new Dictionary<string, object>
                {
                    { "x", position.x },
                    { "y", position.y },
                    { "z", position.z },
                };
                ECAScript.NotifyUpdate(this, nameof(position), v);
            }
        }
        private Position _position;

        private void Start()
        {
            name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            gameObject.name = name;

        }

        [Action(typeof(Character), "teleports to", typeof(Scene))]
        public void _Teleports()
        {

        }
    }
}
