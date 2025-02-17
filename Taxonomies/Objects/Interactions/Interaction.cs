using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Interactions
{
    /// <summary>
    /// <b>Interaction</b> represents entities in the scene that facilitate interaction with other objects or the environment.
    /// Unlike <see cref="ECARules4All.RuleEngine.Behaviour">Behaviours</see>, which define object-based rules and logic, 
    /// <b>Interaction</b> focuses on physical entities that are perceived as independent objects by the user. 
    /// These entities exist as standalone components within the environment, enhancing user engagement and interaction.
    /// </summary>
    [ECARules4All("interaction")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class Interaction : MonoBehaviour
    {

    }
}
