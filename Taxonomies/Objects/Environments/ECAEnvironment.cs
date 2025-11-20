using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Objects.Props;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Objects.Environments
{
    /// <summary>
    /// <b>ECAEnvironment</b> is a component that represents non-interactable elements of the environment.
    /// Unlike props (<see cref="ECAProp"/>), environment items cannot be directly manipulated or interacted with by objects equipped with an <see cref="ECACharacter"/> component.
    /// </summary>
    [ECARules4All("environment")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class ECAEnvironment : MonoBehaviour
    {
    }

}