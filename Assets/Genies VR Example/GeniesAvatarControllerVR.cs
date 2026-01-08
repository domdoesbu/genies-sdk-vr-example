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
        [SerializeField] private TextAsset _config;
        [SerializeField] private GeniesCharacterRetargeterForMeta _retargeter;
        [SerializeField] private MetaSourceDataProvider _metaSourceDataProvider;

        public void InitializeWithLoadedAvatar()
        {
            _retargeter.gameObject.SetActive(true);
            _retargeter.ConfigAsset = _config;
            _retargeter.enabled = true;
            _metaSourceDataProvider.enabled = true;
        }
    }
}
