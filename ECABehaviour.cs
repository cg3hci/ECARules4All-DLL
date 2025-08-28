using System;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL
{
    /// <summary>
    /// <b>ECABehaviour</b> serves as a foundational component required for all behavior implementations within the automation framework. 
    /// While only one instance of <see cref="ECABehaviour"/> is attached to a GameObject, it enables and supports specific behaviors such as Toggle or Switch,  
    /// which inherit from this class and define unique functionality.
    /// This class does not contain any specific functionality, but rather serves as a base class for all behavior implementations.
    /// </summary>
    [ECARules4All("behaviour")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ECAObject))]
    public class ECABehaviour : MonoBehaviour
    {
        
    }
}