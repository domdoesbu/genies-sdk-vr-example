using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Genies.VRExample
{
    /// <summary>
    /// Show or hide hands and controllers based on whether the avatar is loaded.
    /// </summary>
    public class HandAndControllerVisibilityUpdater : MonoBehaviour
    {
        [SerializeField] private bool _hideHandsAndControllersWhenAvatarIsSpawned = true;

        [SerializeField] private GeniesAvatarControllerVR _geniesAvatarControllerVR;

        [SerializeField] private List<GameObject> _handsAndControllersToHide = new List<GameObject>();

        private void Update()
        {
            if (_hideHandsAndControllersWhenAvatarIsSpawned && _geniesAvatarControllerVR != null)
            {
                foreach (GameObject go in _handsAndControllersToHide )
                {
                    go.SetActive(!_geniesAvatarControllerVR.IsAvatarLoaded);
                }
            }
        }
    }
}