using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Inria.Avatar.AvatarReady;

public class AutomaticAvatarLoad : MonoBehaviour
{
    public bool loadOnPlay;
    public string AssetBundleName;
    public bool overrideGalleryURL;
    public string galleryURL;

    void Awake()
    {
        if (overrideGalleryURL)
        {
            AvatarGalleryApi.AvatarGalleryURL = galleryURL;
        }
    }

    void Start()
    {
        if (!loadOnPlay) return;

        AvatarReadySetup target = gameObject.GetComponent<AvatarReadySetup>();

        TextAsset fileAsText = Resources.Load(AssetBundleName) as TextAsset;

        if (fileAsText == null)
        {
            StartCoroutine(AvatarReadyRuntimeImportHelper.ApplyAvatar(target, AssetBundleName, () =>
            {
                // Deal with calibration
                CalibrationUi calibrationUi = FindObjectOfType<CalibrationUi>();
                if (calibrationUi != null)
                {
                    calibrationUi.OnRefreshAvatarsDropdownButtonClick();
                    calibrationUi.OnApplyCalibrationButtonClick();
                }
            }));
        }
        else
        {
            StartCoroutine(LoadAvatarFromResources(target, fileAsText, AssetBundleName, () =>
            {
                // Deal with calibration
                CalibrationUi calibrationUi = FindObjectOfType<CalibrationUi>();
                if (calibrationUi != null)
                {
                    calibrationUi.OnRefreshAvatarsDropdownButtonClick();
                    calibrationUi.OnApplyCalibrationButtonClick();
                }
            }));
        }
    }

    /// <summary>
    /// Method that loads an avatar from an assetbundle from the project (asset in a Resource folder) 
    /// </summary>
    /// <param name="avatarReadyTarget"></param>
    /// <param name="fileAsText"></param>
    /// <param name="avatarName"></param>
    /// <param name="OnAvatarLoaded"></param>
    /// <returns></returns>
    IEnumerator LoadAvatarFromResources(AvatarReadySetup avatarReadyTarget, TextAsset fileAsText, string avatarName, Action OnAvatarLoaded = null)
    {
        avatarReadyTarget.GetComponent<AvatarReadyRuntimeLink>().OnAvatarStartLoad.Invoke();
 
        byte[] bundleData = fileAsText.bytes.Clone() as byte[];
        AssetBundleCreateRequest avatarAssetBundleRequest = AssetBundle.LoadFromMemoryAsync(bundleData);
        yield return avatarAssetBundleRequest;
           
        AssetBundle avatarAssetBundle = avatarAssetBundleRequest.assetBundle;
        if (avatarAssetBundle == null)
        {
            Debug.LogError("No AssetBundle for " + avatarName);
            yield break;
        }

        yield return null;

        // Look for avatar prefab
        AssetBundleRequest avatarObjsRequest = avatarAssetBundle.LoadAllAssetsAsync<GameObject>();
        yield return avatarObjsRequest;

        GameObject[] objs = avatarObjsRequest.allAssets.Cast<GameObject>().ToArray();
        if (objs.Length == 0)
        {
            Debug.LogError("No GameObject on the AssetBundle of " + avatarName);
            yield break;
        }
        if (objs.Length > 1)
        {
            Debug.LogWarning("More than one GameObject on the AssetBundle of " + avatarName + ", picking the first.");
        }
        GameObject avatarPrefab = objs.First();

        // Dispose AssetBundle after
        OnAvatarLoaded += () => avatarAssetBundle.Unload(false);

        yield return AvatarReadyRuntimeImportHelper.ApplyAvatar(avatarReadyTarget, avatarPrefab, OnAvatarLoaded);
    }
}
