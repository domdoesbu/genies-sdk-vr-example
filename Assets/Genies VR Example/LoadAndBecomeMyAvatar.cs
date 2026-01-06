using System;
using System.Collections;
using System.Collections.Generic;
using Genies.Sdk;
using UnityEngine;

namespace Genies.VRExample
{
    public class LoadAndBecomeMyAvatar : MonoBehaviour
    {
        public event Action<bool> OnAvatarLoaded;

        public ManagedAvatar LoadedAvatar;

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
            LoadedAvatar = await AvatarSdk.LoadUserAvatarAsync();

            if (LoadedAvatar != null)
            {
                OnAvatarLoaded?.Invoke(true);
            }
        }
    }
}
