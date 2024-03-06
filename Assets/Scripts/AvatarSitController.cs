using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RootMotion.FinalIK;
using Inria.Avatar.AvatarReady;

/// <summary>
/// Component which manages the avatar in a seated configuration
/// </summary>
public class AvatarSitController : MonoBehaviour
{
    public Transform head;
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform pelvis;

    public Transform CenterPosition;
    public Transform TrackingOrigin;

    private Vector3 lastHeadPos;
    private Vector3 lastPelvisPos;
    private Quaternion originalPelvisOrientation;


#if PHOTON_UNITY_NETWORKING
    Photon.Pun.PhotonView view;
#endif

    private void Start()
    {
        //Disable the controller till the anchors are enabled
        enabled = false;
    }

    #region public
    public void SitDown(GameObject avatar)
    {
        if(!enabled) InitAnchors();

        if (!enabled) return; //Anchors missing

        VRIK ik = avatar.GetComponent<VRIK>();
        ik.solver.spine.chestClampWeight = 1.0f;

#if PHOTON_UNITY_NETWORKING

		//This information is updated only by the owner of the avatar.
		
        view = GetComponentInParent<Photon.Pun.PhotonView>();

        if (view != null && view.IsMine)
        {
#endif
            //Match the X,Z position of the head with CenterPosition
            Vector3 translation = CenterPosition.position - head.position;
            translation.y = 0;
            TrackingOrigin.Translate(translation);

            //Align forward vectors of the head and the target
            Vector3 targetOrientation = CenterPosition.forward;
            Vector3 currentOrientation = head.forward;
            currentOrientation.y = 0;
            currentOrientation.Normalize();

            TrackingOrigin.RotateAround(head.position, Vector3.Cross(currentOrientation, targetOrientation),
                Vector3.Angle(currentOrientation, targetOrientation));
            
            pelvis.rotation = originalPelvisOrientation;

#if PHOTON_UNITY_NETWORKING
        }

        if (view != null && !view.IsMine)
        {
            enabled = false;
        }
#endif

#if AVATARREADY_FINALIK
        //Sit the avatar
        avatar.GetComponent<SitFinalIKFeature>().Sit(pelvis, leftFoot, rightFoot);
#endif
    }
#endregion

#region callbacks

    void InitAnchors()
    {
        if(leftFoot == null) leftFoot = FindTransform("LeftFootAnchor");
        if (rightFoot == null) rightFoot = FindTransform("RightFootAnchor");
        if(pelvis == null) pelvis = FindTransform("PelvisAnchor");

        if (head == null || leftFoot == null || rightFoot == null || pelvis == null || CenterPosition == null || TrackingOrigin == null)
        {
            Debug.LogError("[AvatarSitController] Transforms are not properly configured.");
        }
        else
        {
            originalPelvisOrientation = pelvis.rotation;
            lastHeadPos = head.position;
            lastPelvisPos = pelvis.position;
            enabled = true;
        }
    }

    Transform FindTransform(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null) return obj.transform;
        else return null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 from = lastHeadPos - lastPelvisPos;
        Vector3 to = head.position - pelvis.position;

        // Compute the cross product to compute the axis of rotation Vector3.Cross
        Vector3 axis = Vector3.Cross(from.normalized, to.normalized);

        //Compute the angle between previous and the current frame Vector3.Angle
        float angle = Vector3.Angle(from, to);

        if (angle > 0.1f)
        {
            lastHeadPos = head.position;
            //Last pelvis position is needed if a global transform is applied to the rig
            lastPelvisPos = pelvis.position;

            //Compute the quaternion that encodes the rotation of "angle" along the "axis" 
            pelvis.rotation *= Quaternion.AngleAxis(angle, axis);
        }
    }
#endregion
}
