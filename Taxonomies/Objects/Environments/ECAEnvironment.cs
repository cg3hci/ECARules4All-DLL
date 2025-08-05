using System.Collections;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Environments
{
    /// <summary>
    /// <b>ECAEnvironment</b> represents a generic class for environment items.
    /// On contrary with props (<see cref="ECAProp"/>), environment items are not interactable by characters.
    /// </summary>
    [ECARules4All("environment")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class ECAEnvironment : MonoBehaviour
    {
    }

}