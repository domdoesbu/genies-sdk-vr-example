using System;
using System.Collections;
using System.Collections.Generic;
using Genies.Sdk;
using Meta.XR.Movement.Retargeting;
using UnityEngine;

namespace Genies.VRExample
{
    public class GeniesAvatarControllerVR : MonoBehaviour
    {
        public void InitializeWithLoadedAvatar(ManagedAvatar loadedAvatar)
        {
            // Attach the Character Retargeter
            var retargeter = gameObject.AddComponent<CharacterRetargeter>();
        }
    }
}
