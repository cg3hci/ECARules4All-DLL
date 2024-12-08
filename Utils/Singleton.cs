using JetBrains.Annotations;
using UnityEngine;

namespace ECARules4All_DLL.Utils
{
    // Before: public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    
    /// <summary>
    /// Optionally, specify a base class != MonoBehaviour but that still inherits from MonoBehaviour (e.g. GenericUIMenu)
    /// The Singleton works still on T and not on TBase (there can be multiple references to TBase)
    ///</summary>
    /// 
    /// <example>
    /// <code>
    /// public class A : MonoBehaviour { ... }
    /// public class B : A { ... }
    /// public class BSingleton : Singleton &lt;B, A> { }
    ///
    /// void SomeCode() {
    ///     BSingleton.Instance.DoSomething();
    /// }
    /// </code>
    /// A is the base class. It is not a Singleton.
    /// B is the derived class. We want it a Singleton, but there is a step more
    /// BSingleton is the Singleton of B (but not of A)
    /// </example>
    public abstract class Singleton<T, TBase> : Singleton where T : TBase where TBase : MonoBehaviour
    {
        #region  Fields
        [CanBeNull]
        private static T _instance;

        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        [SerializeField]
        private bool _persistent = true;
        #endregion

        #region  Properties
        [NotNull]
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }
                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1)
                            return _instance = instances[0];
                        Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        return _instance = instances[0];
                    }

                    Debug.Log($"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"({nameof(Singleton)}){typeof(T)}")
                               .AddComponent<T>();
                }
            }
        }
        #endregion

        #region  Methods
        private void Awake()
        {
            if (_persistent)
                DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        protected virtual void OnAwake() { }
        #endregion
    }

    // Default Singleton where the base type is not specified (MonoBehaviour)
    public abstract class Singleton<T> : Singleton<T, MonoBehaviour> where T : MonoBehaviour { } // new
    
    public abstract class Singleton : MonoBehaviour
    {
        #region  Properties
        public static bool Quitting { get; private set; }
        #endregion

        #region  Methods
        private void OnApplicationQuit()
        {
            Quitting = true;
        }
        #endregion
    }
}