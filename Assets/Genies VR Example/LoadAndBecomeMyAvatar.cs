using System;
using System.Collections;
using System.Collections.Generic;
using Genies.Sdk;
using UnityEngine;

namespace Genies.VRExample
{
    public class LoadAndBecomeMyAvatar : MonoBehaviour
    {
        public ManagedAvatar LoadedAvatar;

        [SerializeField] private GeniesAvatarControllerVR _geniesAvatarControllerVR;

        private void Start()
        {
            if (!AvatarSdk.IsLoggedIn)
            {
                AvatarSdk.Events.UserLoggedIn += LoadAvatar;

                return;
            }

            LoadAvatar();
        }

        private async void LoadAvatar()
        {
            LoadedAvatar = await AvatarSdk.LoadUserAvatarAsync(parent: _geniesAvatarControllerVR.transform);

            _geniesAvatarControllerVR.InitializeWithLoadedAvatar(LoadedAvatar);

        }
    }
}
