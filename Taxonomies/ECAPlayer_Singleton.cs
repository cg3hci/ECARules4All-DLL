using System;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Taxonomies.Utils
{
// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]
    public class ECAPlayer_Singleton : Singleton<ECAPlayer_Singleton>
    {
        public GameObject playerGoRef;
        public Transform playerTransform;
        public ECACharacter playerEcaCharacterRef;

        void Start()
        {
            if (playerGoRef == null)
            {
                throw new Exception("YOU MUST ASSIGN THE PLAYER TRANSFORM TO THE PLAYER OBJECT");
            }

            if (playerTransform == null)
            {
                throw new Exception("YOU MUST ASSIGN THE PLAYER TRANSFORM TO THE PLAYER OBJECT");
            }

            if (playerEcaCharacterRef == null)
            {
                throw new Exception("YOU MUST ASSIGN THE ECA CHARACTER REFERENCE TO THE PLAYER OBJECT");
            }
        }
    }
}