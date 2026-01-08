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

        private ManagedAvatar _avatar;
        private Material[] _originalSharedMaterials;

        private Material _invisible;

        public void InitializeWithLoadedAvatar(ManagedAvatar avatar)
        {
            _avatar = avatar;

            _retargeter.gameObject.SetActive(true);
            _retargeter.ConfigAsset = _config;
            _retargeter.enabled = true;
            _metaSourceDataProvider.enabled = true;

            HideHead();
        }

        private void HideHead()
        {
            // Show or hide the avatar's head to prevent rendering issues in VR.
            SkinnedMeshRenderer avatarRenderer = _avatar.ModelRoot.GetComponentInChildren<SkinnedMeshRenderer>();

            if (avatarRenderer == null)
            {
                Debug.LogError("Could not find SkinnedMeshRenderer on avatar to show/hide head.");
                return;
            }

            Material[] materials = avatarRenderer.sharedMaterials;

            if (_originalSharedMaterials == null)
            {
                _originalSharedMaterials = materials;
            }

            if (_invisible == null)
            {
                _invisible = CreateInvisibleMaterial();
            }

            // Find any materials with the words "eye", "hair", or "race" in their names and toggle their visibility.
            for (int i = 0; i < materials.Length; i++)
            {
                string materialName = materials[i].name.ToLower();
                if (materialName.Contains("eye") || materialName.Contains("hair") || materialName.Contains("race")) // "race" is used in the inner mouth for some reason
                {
                    materials[i] = _invisible;
                }
            }

            avatarRenderer.materials = materials;

        }

        private Material CreateInvisibleMaterial()
        {
            var shader = Shader.Find("Hidden/Genies/NoDraw_NoWrite_URP");
            var mat = new Material(shader);
            return mat;
        }
    }
}
