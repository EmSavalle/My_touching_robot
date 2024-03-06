using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    [AvatarReadyName("Seated Calibration", "Scales the avatar to match the height in a seated configuration")]
    [AvatarReadySupportedSystem(typeof(IKAnimationSystem))]
    [AvatarReadyCalibrationData(chairHeightKey, "Height of the chair (in meters)")]

    public class SeatedCalibration : CalibrationTechnique
    {
        public GameObject chair;

        private const string chairHeightKey = "chairHeight";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            Assert.IsNotNull(animator);
        }

        public override IEnumerator CalibrateAvatar(AvatarReady avatarReady, CalibrationProfile calibrationProfile)
        {
            if (calibrationProfile == null)
                calibrationProfile = CalibrationProfilesManager.Instance.SessionProfile;

            float chairHeight;

            if (!calibrationProfile.ContainsData(chairHeightKey))
            {
                chair = GameObject.Find("ChairHeight");

                if (chair == null)
                    chairHeight = 0.45f;
                else
                    chairHeight = chair.transform.position.y;
            }
            else
            {
                chairHeight = calibrationProfile.Get(chairHeightKey);
            }

            yield return new WaitForSeconds(1.0f);

            float distance = (GetAvatarPelvisPos().y - 0.1f) - chairHeight;
            float globalScale = 1.0f;

            Transform boneRoot = animator.GetBoneTransform(HumanBodyBones.Hips).parent;

            while (Mathf.Abs(distance) > 0.01f)
            {
                globalScale += 0.005f * Mathf.Sign(distance);
                boneRoot.localScale = Vector3.one * globalScale;

                yield return new WaitForEndOfFrame();

                distance = (GetAvatarPelvisPos().y - 0.1f) - chairHeight;
            }
        }

        public override float GetData(AvatarReady avatarReady, string dataKey)
        {
            switch (dataKey)
            {
              //  case chairHeightKey:
              //      return avatarReady.TrackerDescriptors[HumanBodyBones.Head].Tracker.position.y;// TODO relative to camerarig y ?
                default:
                    throw new KeyNotFoundException(dataKey);
            }
        }

        Vector3 GetAvatarCenterEyePos()
        {
            Vector3 tposeAvatarLeftEyePosition = animator.GetBoneTransform(HumanBodyBones.LeftEye).position;
            Vector3 tposeAvatarRightEyePosition = animator.GetBoneTransform(HumanBodyBones.RightEye).position;
            
            return (tposeAvatarLeftEyePosition + tposeAvatarRightEyePosition) / 2.0f;
        }

        Vector3 GetUserCenterEyePos(AvatarReady avatarReady)
        {
            return avatarReady.TrackerDescriptors[HumanBodyBones.Head].Tracker.position;
        }

        Vector3 GetAvatarPelvisPos()
        {
            return animator.GetBoneTransform(HumanBodyBones.Hips).position;
        }
    }
}
