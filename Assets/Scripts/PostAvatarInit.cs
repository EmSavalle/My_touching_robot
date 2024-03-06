using UnityEngine;

public class PostAvatarInit : MonoBehaviour
{
    public void PostAvatarConfiguration(GameObject avatar)
    {
        SkinnedMeshRenderer[] renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach(SkinnedMeshRenderer r in renderers)
            r.updateWhenOffscreen = true;
    }
}
