using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Globalization;
using UnityEngine.XR;
using System.Linq;
using System.IO;
using Leap.Unity;

public class URTest : MonoBehaviour
{

    [Header("Save Position")]
    public bool save;
    public bool load;
    public bool saveAlignementData;
    public bool loadAlignementData;
    public string filePos, fileJoints, fileAlign;
    public bool hideScreen;
    private bool isHided;


    [Header("Action")]
    public bool calibrateHand;
    public bool calibratePressure;
    public bool goSafe;
    public bool goHover;
    public bool move = false;



    [Header("Game Objects")]
    public GameObject robotObject;
    public UR5 robot;
    public FakeUR fake;
    public GameObject target;
    public GameObject instTarget;
    public GameObject actuator;
    public GameObject fakeActuator;
    public GameObject safeSpace;
    public GameObject hoverSpace;
    public GameObject fakehoverSpace;
    public GameObject screenHider;


    public float movementTime = 1;



    /* public GameObject tcp;
     public double unityX;
     public double unityY;
     public double unityZ;
     public double unityToRobotX;
     public double unityToRobotY;
     public double unityToRobotZ;*/

    [Header("Target Position")]
    private double npx;
    private double npy;
    private double npz;

    [Header("Simple move")]
    public GameObject limits;
    public float moveDelay = 1f;
    public float speed = 0.1f;




    [Header("Teach")]
    private bool isTeaching = false;
    public bool teach;

    private DateTime lastMove;
    private List<UnityEngine.XR.InputDevice> gameControllers;


    [Header("Alignement")]
    public bool alignAvatar = false;
    public bool alignLeap = false;
    private bool isLeapAligned = false;
    public bool alignRobot = false;
    public bool hoverPos;

    public float tableYOffset = 0f;
    private bool calibratingHand = false;
    private GameObject leftHand, rightHand;


    [Header("Robot positions")]
    public GameObject handTouchPos;
    public GameObject TouchDetector;
    public GameObject prefabHandTouch;
    public GameObject prefabTouchDetector;
    public float offsetHandY;
    private GameObject locationBase, locationMid;

    public bool safePos;
    private bool hassafePos;
    private bool hashoverPos;
    private GameObject noPos, lightPos, hardPos;
    private Vector3 vnoPos, vlightPos, vhardPos;
    private bool hasnoPos, haslightPos, hashardPos;
    private bool manualSet = true;
    private int noPosPressure, lightPosPressure, hardPosPressure;
    public float safeYPos = 0.85f;
    public float unsafeYPos = 0.6f;

    [Header("Alignement params")]
    public GameObject alignment;
    public GameObject alignmentPointRobot;
    public List<GameObject> movableRobot;

    public GameObject alignmentPointLeap;
    public List<GameObject> movableLeap;
    public GameObject actualAlignmentPointLeap;//Avatar left hand

    public GameObject alignmentPointAvatar;
    public List<GameObject> movableAvatar;
    public GameObject actualAlignment;
    public GameObject controllerAlignment;



    public float simulatedOffsetx, simulatedOffsety, simulatedOffsetz;

    [Header("Testing coroutine")]
    public string moveCouroutineStep;
    public bool movingCoroutine;
    public bool pCphy, pCrec;
    public string pCtype;

    [Header("Forces & speeds")]
    public double Fx;
    public double Fy;
    public double Fz;
    private double Mx;
    private double My;
    private double Mz;
    private double Sx;
    private double Sy;
    private double Sz;
    private double Ssx;
    private double Ssy;
    private double Ssz;

    [Header("Robot Position")]
    public double x;
    public double y;
    public double z;
    public double robotUnityX;
    public double robotUnityY;
    public double robotUnityZ;
    private GameObject baseHand;
    private GameObject baseMiddle;
    private bool isTargetInstantiated;



    [Header("Visualisation")]
    public GameObject robotRig;
    public GameObject ballRig;
    public GameObject fakeRig;

    [Header("Communication")]
    public UnityCommunicator comms;
    public UnityCommunicatorContinuous commsPressure;

    private Vector3 alignAvatarSave,alignRobotSave, alignLeapSave;
    public bool hasTouched;
    public bool hasUnTouched;

    public bool verbose;
    private bool calibrateRightHand;
    private bool calibrateElbow;
    public GameObject elbow;
    public Vector3 offsetElbow;
    public Vector3 rightHandPos;
    /*public bool robotMoved = false;
public bool avatarMoved = false;
public bool leapMoved = false;*/
    // Start is called before the first frame update
    void Start()
    {
        gameControllers = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.GameController, gameControllers);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rightHand == null)
        {
            rightHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/RightHand").gameObject;
        }
        if(!calibrateRightHand && rightHand != null)
        {
            calibrateRightHand = true;
            rightHand.SetActive(false);
            rightHand.transform.position += rightHandPos;
        }
        if (elbow == null && !calibrateElbow)
        {
            for (int i = 0; i < alignmentPointAvatar.transform.childCount; i++)
            {
                GameObject child = alignmentPointAvatar.transform.GetChild(i).gameObject;
                if (child.name.Contains("Adult_"))
                {
                    Debug.Log(child.name);
                    var x = child.transform.Find("CC_Base_BoneRoot/CC_Base_Hip/CC_Base_Waist/CC_Base_Spine01/CC_Base_Spine02/CC_Base_L_Clavicle");
                    if (x != null)
                    {
                        elbow = x.gameObject;
                        elbow.transform.position += offsetElbow;
                        calibrateElbow = true;
                        break;
                    }

                }
            }

        }
        if (hideScreen && ! isHided)
        {
            screenHider.SetActive(true);
            isHided = true;
        }
        else if (!hideScreen && isHided)
        {
            screenHider.SetActive(false);
            isHided = false;
        }
        /*if(Fz > 50)
        {
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
            throw new Exception("Too much pressure");
        }*/
        if(goHover && isAtPos(hoverSpace))
        {
            goHover = false;
        }
        if (save)
        {
            save = false;
            savePosition(filePos, fileJoints);
        }
        if (load)
        {
            load = false;
            StartCoroutine(loadPosition(filePos, fileJoints));
        }
        if (saveAlignementData)
        {
            saveAlignementData = false;
            saveAlignData();
        }
        if (loadAlignementData)
        {
            loadAlignementData = false;
            loadAlignData();
        }
        updateHandTouchPos();
        if (calibrateHand && !calibratingHand)
        {
            if(leftHand == null)
            {
                leftHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand").gameObject;
                rightHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/RightHand").gameObject;
            }
            leftHand.SetActive(false);
            calibratingHand = true;
            handTouchPos.SetActive(true);
            TouchDetector.SetActive(true);
        }
        else if(calibratingHand && !calibrateHand)
        {
            if (leftHand == null)
            {
                leftHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand").gameObject;
                rightHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/RightHand").gameObject;
            }
            leftHand.SetActive(true);
            calibratingHand = false;
            handTouchPos.SetActive(false);
            TouchDetector.SetActive(false);
        }
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        //ldevice = leftHandDevices[0];
        //rdevice = rightHandDevices[0];
        //Swapped y and z to correspond to robot space
        npx = ((int)(target.transform.position.x * 1000) / 1000.0);
        npz = ((int)(target.transform.position.y * 1000) / 1000.0);
        npy = ((int)(target.transform.position.z * 1000) / 1000.0);
        Transform transform = robotObject.transform;
        double offsetx = transform.position.x;
        double offsety = transform.position.z;
        double offsetz = transform.position.y;
        npx = npx - ((int)(offsetx * 1000) / 1000.0);
        npy = npy - ((int)(offsety * 1000) / 1000.0);
        npz = npz - ((int)(offsetz * 1000) / 1000.0);

        // Define the desired rotation in Euler angles
        Vector3 desiredEulerAngles = new Vector3(180f, 0f, 0f); // Example: Rotate 180 degrees around the y-axis
        // Create a quaternion from the desired Euler angles
        Quaternion desiredRotation = Quaternion.Euler(desiredEulerAngles);
        // Multiply the original rotation by the desired rotation
        Quaternion newRotation = target.transform.rotation * desiredRotation;

        Vector3 rot = robot.ur.QuaternionToPoseR(newRotation);

        double rpx = rot.x;
        double rpy = rot.y;
        double rpz = rot.z;
        if (robot.IsInitialized)
        {
            // Get info from robot
            double[] forces = robot.GetTCPForce();
            Fx = ((int)(forces[0] * 1000) / 1000.0);
            Fy = ((int)(forces[1] * 1000) / 1000.0);
            Fz = ((int)(forces[2] * 1000) / 1000.0);
            Mx = ((int)(forces[3] * 1000) / 1000.0);
            My = ((int)(forces[4] * 1000) / 1000.0);
            Mz = ((int)(forces[5] * 1000) / 1000.0);

            commsPressure.SendData((float)Fz);
            forces = robot.GetTCPSpeed();
            Sx = ((int)(forces[0] * 1000) / 1000.0);
            Sy = ((int)(forces[1] * 1000) / 1000.0);
            Sz = ((int)(forces[2] * 1000) / 1000.0);
            Ssx = ((int)(forces[3] * 1000) / 1000.0);
            Ssy = ((int)(forces[4] * 1000) / 1000.0);
            Ssz = ((int)(forces[5] * 1000) / 1000.0);

            double[] pos = robot.GetTCPPosition();
            x = ((int)(pos[0] * 1000) / 1000.0);
            y = ((int)(pos[1] * 1000) / 1000.0);
            z = ((int)(pos[2] * 1000) / 1000.0);
            /*Vector3 robotToUnity = new Vector3((float)x, (float)y, (float)z);
            robotToUnity = robot.ur.PoseTToVector3(robotToUnity);
            robotUnityX = robotToUnity.x;
            robotUnityY = robotToUnity.y;
            robotUnityZ = robotToUnity.z;



            unityX = tcp.transform.position.x;
            unityY = tcp.transform.position.y;
            unityZ = tcp.transform.position.z;
            Vector3 unityToRobot = new Vector3((float)unityX, (float)unityY, (float)unityZ);
            unityToRobot = robot.ur.Vector3ToPoseT(unityToRobot);
            unityToRobotX = unityToRobot.x;
            unityToRobotY = unityToRobot.y;
            unityToRobotZ = unityToRobot.z;*/
            
            if (teach && !isTeaching)
            {
                if (verbose) { Debug.Log("Teaching"); }
                isTeaching = true;
                string prog = "";

                prog += "teach_mode()\n";

                robot.ur.SendProgram(prog);
            }

            if (!teach && isTeaching)
            {
                if (verbose)
                {
                    Debug.Log("unTeaching");
                }
                isTeaching = false;
                string prog = "";
                prog += "end_teach_mode()\n";

                robot.ur.SendProgram(prog);
            }
            if (move)
            {
                goTo(target, false);
            }
            if (hoverPos)
            {
                defHover();
                hoverPos = false;
                target.transform.position = hoverSpace.transform.position;
            }
            if (goHover)
            {
                hover();
            }
        }

        if (isTargetInstantiated)
        {
            target.transform.position = Vector3.Lerp(baseHand.transform.position, baseMiddle.transform.position, 0.5f);
        }
        if (calibratePressure)
        {
            StartCoroutine(DetermineTouchPressure());
            calibratePressure = false;
        }

        if (alignRobot)
        {
            if (verbose)
            {
                Debug.Log("Aligning robot");
            }
            align("Robot");
            alignRobot = false;
        }
        if (alignAvatar)
        {
            if (verbose)
            {
                Debug.Log("Aligning Avatar");
            }
            align("Avatar");
            alignAvatar = false;
        }
        if (alignLeap)
        {
            if (verbose)
            {
                Debug.Log("Aligning Leap");
            }
            align("Leap");
            alignLeap = false;
        }
    }

    public void align(String mode)
    {
        if(mode == "Robot")
        {
            // Move the robot so the tcp object lign up with the physical alignement point
            Vector3 offset = handTouchPos.transform.position - alignmentPointRobot.transform.Find("AlignementPointRobot").transform.position;
            alignRobotSave = offset;
            foreach (GameObject go in movableRobot)
            {
                go.transform.position += offset;
            }

        }
        /*else if (mode == "Avatar")
        {
            // Move the unity scene so the right hand model lign up with the physical alignement point

            actualAlignment = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
            if (actualAlignment == null)
            {
                Debug.LogError("No right hand found");
            }
            Vector3 offset = controllerAlignment.transform.position - alignment.transform.position;

            foreach (GameObject go in movableAvatar)
            {
                go.transform.position += offset;
            }

        }*/
        else if (mode == "AvatarHand" || mode == "Avatar")
        {
            // Move the unity scene so the right hand model lign up with the physical alignement point

            actualAlignment = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
            if (actualAlignment == null && verbose)
            {
                Debug.LogError("No left hand found");
            }
            Vector3 offset = alignment.transform.position - actualAlignment.transform.position;
            offset.x = 0;offset.z = 0;
            offset.y += tableYOffset;
            alignAvatarSave = offset;
            foreach (GameObject go in movableAvatar)
            {
                go.transform.position -= offset;
            }

        }
        else if (mode == "Leap")
        {
            if (actualAlignmentPointLeap == null)
            {
                for (int i = 0; i < alignmentPointAvatar.transform.childCount; i++)
                {
                    String Go = alignmentPointAvatar.transform.GetChild(i).name;
                    if (Go.Contains("AvatarCCHandsInteractionLeap"))
                    {
                        actualAlignmentPointLeap = alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
                    }
                }
            }


           
            Vector3 offset = alignment.transform.position - actualAlignmentPointLeap.transform.position;

            alignLeapSave = offset;
            foreach (GameObject go in movableLeap)
            {
                // Rotate the object to align with global forward
                //go.transform.rotation = targetRotation;

                // Calculate the position offset

                // Apply the offset to the position
                go.transform.position += offset;
            }

            // Set the alignment flag to true
            //align("AvatarHand");
            isLeapAligned = true;

            // Defining contact point on hand
            defineHandTouchPos();
        }
    }
    public void loadAlign(String mode, Vector3 offset)
    {
        if (mode == "Robot")
        {
            foreach (GameObject go in movableRobot)
            {
                go.transform.position += offset;
            }

        }
        else if (mode == "AvatarHand" || mode == "Avatar")
        {
            foreach (GameObject go in movableAvatar)
            {
                go.transform.position -= offset;
            } 

        }
        else if (mode == "Leap")
        {
            foreach (GameObject go in movableLeap)
            {
                // Apply the offset to the position
                go.transform.position += offset;
            }

            isLeapAligned = true;
            // Defining contact point on hand
            defineHandTouchPos();
        }
    }


    public void defineHandTouchPos()
    {
        if (handTouchPos == null)
        {

            for (int i = 0; i < alignmentPointAvatar.transform.childCount; i++)
            {
                String Go = alignmentPointAvatar.transform.GetChild(i).name;
                if (Go.Contains("AvatarCCHandsInteractionLeap"))
                {
                    locationBase = alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand").gameObject;
                    locationMid = alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
                }
            }
            handTouchPos = Instantiate(prefabHandTouch);
            TouchDetector= Instantiate(prefabTouchDetector);

        }
        float t = 0.75f; // This represents the 3/4th point
        float x = locationBase.transform.position.x + (locationMid.transform.position.x - locationBase.transform.position.x) * t;
        float y = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) * t + offsetHandY;
        float y2 = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) * t + offsetHandY/2;
        float z = locationBase.transform.position.z + (locationMid.transform.position.z - locationBase.transform.position.z) * t;

        handTouchPos.transform.position = new Vector3(x, y, z);
        TouchDetector.transform.position = new Vector3(x, y, z);
    }
    public void updateHandTouchPos()
    {
        if(!calibrateHand && handTouchPos != null)
        {
            handTouchPos.SetActive(false);
            TouchDetector.SetActive(false);
        }
        if(handTouchPos != null)
        {
            float t = 0.75f; // This represents the 3/4th point
            float x = locationBase.transform.position.x + (locationMid.transform.position.x - locationBase.transform.position.x) * t;
            float y = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) * t + offsetHandY;
            float y2 = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) * t + offsetHandY/2;
            float z = locationBase.transform.position.z + (locationMid.transform.position.z - locationBase.transform.position.z) * t;
            handTouchPos.transform.position = new Vector3(x, y, z);
            TouchDetector.transform.position = new Vector3(x, y, z);
        }
    }
    void RotateRobotsAroundTarget(GameObject robot, GameObject target, float angleY)
    {
        Vector3 rotationAxis = Vector3.up;
        Vector3 targetPosition = target.transform.position;

        Transform targetTransform = robot.transform;
        Vector3 offset = targetTransform.position - targetPosition;

        // Rotate the robot around the target
        targetTransform.RotateAround(targetPosition, rotationAxis, angleY);

        // Adjust the position to maintain the same offset from the target
        robot.transform.position = targetPosition + offset;
    }

    public Vector3 transformPosToRobotPosition(Vector3 to, bool offset)
    {
        double dx, dy, dz;
        if (offset)
        {
            dx = ((int)((to.x) * 1000) / 1000.0);
            dy = ((int)((to.y) * 1000) / 1000.0);
            dz = ((int)((to.z) * 1000) / 1000.0);
        }
        else
        {
            dx = ((int)((to.x) * 1000) / 1000.0);
            dy = ((int)((to.y) * 1000) / 1000.0);
            dz = ((int)((to.z) * 1000) / 1000.0);
        }
        Transform transform = robotObject.transform;
        double offsetx = transform.position.x;
        double offsety = transform.position.z;
        double offsetz = transform.position.y;
        dx = dx - ((int)(offsetx * 1000) / 1000.0);
        dy = dy - ((int)(offsety * 1000) / 1000.0);
        dz = dz - ((int)(offsetz * 1000) / 1000.0);
        return new Vector3((float)dx, (float)dy, (float)dz);
    }
    public Vector3 transformPosToRobotPosition(Vector3 to)
    {
        return transformPosToRobotPosition(to, true);
    }

    public void goTo(GameObject to, Boolean followRotation, float speed)
    {
        //Swapped y and z to correspond to robot space
        /*npx = ((int)((to.transform.position.x + targetOffsetx) * 1000) / 1000.0);
        npz = ((int)((to.transform.position.y + targetOffsety) * 1000) / 1000.0);
        npy = ((int)((to.transform.position.z + targetOffsetz) * 1000) / 1000.0);*/
        npx = ((int)((to.transform.position.x) * 100000) / 100000.0);
        npz = ((int)((to.transform.position.y) * 100000) / 100000.0);
        npy = ((int)((to.transform.position.z) * 100000) / 100000.0);

        Transform transform = robotObject.transform;
        double offsetx = transform.position.x;
        double offsety = transform.position.z;
        double offsetz = transform.position.y;
        npx = npx - ((int)(offsetx * 100000) / 100000.0);
        npy = npy - ((int)(offsety * 100000) / 100000.0);
        npz = npz - ((int)(offsetz * 100000) / 100000.0);
        /*double rpx = ((int)(Mathf.Deg2Rad * -1*(target.transform.rotation.eulerAngles.x-180) * 1000) / 1000.0);
        double rpz = ((int)(Mathf.Deg2Rad * target.transform.rotation.eulerAngles.y * 1000) / 1000.0);
        double rpy = ((int)(Mathf.Deg2Rad * target.transform.rotation.eulerAngles.z * 1000) / 1000.0);*/


        // Define the desired rotation in Euler angles
        Vector3 desiredEulerAngles = new Vector3(180f, 0f, 0f); // Example: Rotate 180 degrees around the y-axis

        // Create a quaternion from the desired Euler angles
        Quaternion desiredRotation = Quaternion.Euler(desiredEulerAngles);
        Quaternion newRotation;
        if (followRotation)
        {
            // Multiply the original rotation by the desired rotation
            newRotation = to.transform.rotation * desiredRotation;

        }
        else
        {
            // Multiply the original rotation by the desired rotation
            newRotation = desiredRotation;

        }

        Vector3 rot = robot.ur.QuaternionToPoseR(newRotation);

        double rpx = rot.x;
        double rpy = rot.y;
        double rpz = rot.z;

        if (robot.IsInitialized)
        {
            if (limits.GetComponent<Collider>().bounds.Contains(to.transform.position) || true)
            {
                if ((x != npx || y != npy || z != npz) && (Sx == 0 && Sy == 0 && Sz == 0 && Ssx == 0 && Ssy == 0 && Ssz == 0 && (DateTime.Now - lastMove).TotalSeconds > moveDelay))
                {
                    string prog = "movel(p[" + npx.ToString(CultureInfo.InvariantCulture) + "," + npy.ToString(CultureInfo.InvariantCulture) + "," + npz.ToString(CultureInfo.InvariantCulture) + "," + rpx.ToString(CultureInfo.InvariantCulture) + "," + rpy.ToString(CultureInfo.InvariantCulture) + "," + rpz.ToString(CultureInfo.InvariantCulture) + "], a = 1.4, v ="+speed.ToString(CultureInfo.InvariantCulture) +", t = 0, r = 0)" + "\n";
                    lastMove = DateTime.Now;
                    //Debug.Log(prog);
                    robot.ur.SendProgram(prog);
                    
                    

                }
            }
            else if(verbose)
            {
                Debug.Log("Object outside of limits");
            }
        }
    }
    public void goTo(GameObject to, Boolean followRotation)
    {
        goTo(to, followRotation, (float)1.05);
    }
    public void goTo(GameObject to)
    {
        goTo(to, false, (float)1.05);
    }
    public void goTo(GameObject to, float speed)
    {
        goTo(to, false, speed);
    }


    public void defHover()
    {
        hoverSpace.transform.position = actuator.transform.position;
        fakehoverSpace.transform.position = actuator.transform.position;
        hashoverPos = true;
    }
    public void defPos(string pos)
    {
        if (pos == "No")
        {
            /*baseHand = alignmentPointAvatar.transform.Find("Adult_Female10/CC_Base_BoneRoot/CC_Base_Hip/CC_Base_Waist/CC_Base_Spine01/CC_Base_Spine02/CC_Base_L_Clavicle/CC_Base_L_Upperarm/CC_Base_L_Forearm/CC_Base_L_Hand").gameObject;
            baseMiddle = baseHand.transform.Find("CC_Base_L_Mid1").gameObject;
            vnoPos = baseMiddle.transform.position - alignmentPointRobot.transform.position;*/
            if (noPos == null)
            {
                noPos = Instantiate(instTarget);
            }
            //noPos.transform.position = baseMiddle.transform.position - vnoPos;
            noPos.transform.position = alignmentPointRobot.transform.position;
            hasnoPos = true;
        }
        else if (pos == "Light")
        {
            /*baseHand = alignmentPointAvatar.transform.Find("Adult_Female10/CC_Base_BoneRoot/CC_Base_Hip/CC_Base_Waist/CC_Base_Spine01/CC_Base_Spine02/CC_Base_L_Clavicle/CC_Base_L_Upperarm/CC_Base_L_Forearm/CC_Base_L_Hand").gameObject;
            baseMiddle = baseHand.transform.Find("CC_Base_L_Mid1").gameObject;
            vlightPos = baseMiddle.transform.position - alignmentPointRobot.transform.position;*/
            if (lightPos == null)
            {
                lightPos = Instantiate(instTarget);
            }
            //lightPos.transform.position = baseMiddle.transform.position - vlightPos;
            lightPos.transform.position = alignmentPointRobot.transform.position;
            haslightPos = true;
        }
        else if (pos == "Hard")
        {
            /*baseHand = alignmentPointAvatar.transform.Find("Adult_Female10/CC_Base_BoneRoot/CC_Base_Hip/CC_Base_Waist/CC_Base_Spine01/CC_Base_Spine02/CC_Base_L_Clavicle/CC_Base_L_Upperarm/CC_Base_L_Forearm/CC_Base_L_Hand").gameObject;
            baseMiddle = baseHand.transform.Find("CC_Base_L_Mid1").gameObject;
            vhardPos = baseMiddle.transform.position - alignmentPointRobot.transform.position;*/
            if (hardPos == null)
            {
                hardPos = Instantiate(instTarget);
            }
            //hardPos.transform.position = baseMiddle.transform.position - vhardPos;
            hardPos.transform.position = alignmentPointRobot.transform.position;
            hashardPos = true;
        }
        hoverSpace.transform.position = actuator.transform.position;
        fakehoverSpace.transform.position = actuator.transform.position;
        hashoverPos = true;
    }


    public void hover()
    {
        move = false;
        if (hashoverPos || manualSet)
        {
            if (verbose)
            {
                Debug.Log("Hovering");
            }
            goTo(hoverSpace, false);
        }
    }
    public void acquireTarget()
    {
        if (isLeapAligned)
        {

            baseHand = alignmentPointAvatar.transform.Find("Adult_Female10/CC_Base_BoneRoot/CC_Base_Hip/CC_Base_Waist/CC_Base_Spine01/CC_Base_Spine02/CC_Base_L_Clavicle/CC_Base_L_Upperarm/CC_Base_L_Forearm/CC_Base_L_Hand").gameObject;
            baseMiddle = baseHand.transform.Find("CC_Base_L_Mid1").gameObject;
            target = Instantiate(instTarget);
            target.transform.position = Vector3.Lerp(baseHand.transform.position, baseMiddle.transform.position, 0.5f) ;
            isTargetInstantiated = true;
        }
    }
    public IEnumerator MoveCoroutine(GameObject target, bool physical, float touchTime, bool record)
    {
        if (target == noPos)
        {
            return MoveCoroutine("no", physical, touchTime, record);
        }
        else if (target == lightPos)
        {
            return MoveCoroutine("light", physical, touchTime, record);
        }
        else
        {
            return MoveCoroutine("hard", physical, touchTime, record);
        }
    }
    public IEnumerator MoveCoroutine(string type, bool physical, float touchTime, bool record)
    {
        if (verbose)
        {
            Debug.Log("Starting movement coroutine");
        }
        movingCoroutine = true;
        GameObject target;
        switch (type)
        {
            case "no":
                target = noPos;
                break;
            case "light":
                target = lightPos;
                break;
            case "hard":
                target = hardPos;
                break;
            default:
                target = hardPos;
                break;
        }
        //Setp 0 : initialisation
        moveCouroutineStep = "Init";

        /*if (!physical)
        {
            robotRig.SetActive(false);
            ballRig.GetComponent<MeshRenderer>().enabled = false;

        }*/
        if (verbose)
        {
            Debug.Log("Coroutine intialisation");
        }

        //Step 1 : go to original position 
        Vector3 robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y,(float)z));
        if (verbose)
        {
            Debug.Log("Coroutine hovering");
        }
        goHover = true;
        if (!isAtPos(hoverSpace, false))
        {
            moveCouroutineStep = "GoingHover"; 
            robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            while (!isAtPos(hoverSpace, false))
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        //Step 2 : go to touch position
        if (physical)
        {
            comms.SendMarker(UnityCommunicator.OVMarker.ApproachHaptic);
        }
        else
        {
            comms.SendMarker(UnityCommunicator.OVMarker.ApproachVisual);
        }
        moveCouroutineStep = "MovingTouch";
        if (verbose)
        {
            Debug.Log("Coroutine MovingTouch");
        }
        if (!physical)
        {
            fake.StartReplay(type);
        }
        else
        {
            if (record && physical)
            {
                if((type == "no" && fake.recordedPositionsNo.Count == 0 ) || (type == "light" && fake.recordedPositionsLight.Count == 0) || (type == "hard" && fake.recordedPositionsHard.Count == 0))
                {
                    fake.StartRecording(type);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
            moveCouroutineStep = "Touch";
            while (!isAtPos(target, false))
            {

                if (verbose)
                {
                    Debug.Log("Coroutine Moving to target");
                }
                if (physical) { goTo(target); }
                yield return new WaitForSeconds((float)0.1);
                robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
                if (hasTouched)
                {
                    if (physical)
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.TouchHaptic);
                    }
                    else
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.TouchVisual);
                    }
                    if (record)
                    {
                        fake.sendTouch = true;
                    }
                    hasTouched = false;
                }

            }
            move = false;

            if (verbose)
            {
                Debug.Log("Coroutine Touching");
            }
            moveCouroutineStep = "Touching";
            yield return new WaitForSeconds(touchTime);

            //Step 3 : Go back

            moveCouroutineStep = "Backing";
            if (verbose)
            {
                Debug.Log("Coroutine Backing");
            }

            moveCouroutineStep = "GoingHover"; robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            if (physical) {
                goHover = true;
            }
            while (!isAtPos(hoverSpace, false))
            {
                if (hasUnTouched)
                {
                    if (physical)
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.UnTouchHaptic);
                    }
                    else
                    {
                        comms.SendMarker(UnityCommunicator.OVMarker.UnTouchVisual);
                    }
                    if (record)
                    {
                        fake.sendUnTouch = true;
                    }
                    hasUnTouched = false;
                }
                yield return new WaitForSeconds(Time.deltaTime);
                robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            }
            moveCouroutineStep = "Ending";
            if (verbose)
            {
                Debug.Log("Coroutine Ending");
            }
            while (!isAtPos(hoverSpace))
            {
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
            yield return new WaitForSecondsRealtime(0.5f);
            if (record && physical)
            {
                fake.StopRecording(type);
            }
        }
        if (!physical)
        {
            while (fake.replaying)
            {
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
        }
        movingCoroutine = false;
        /*robotRig.SetActive(true);
        ballRig.GetComponent<MeshRenderer>().enabled = true;
        fakeRig.SetActive(true);
        robotRig.SetActive(true);*/
        yield break;
    }
    public void StartMovement(string type, bool physical, float touchTime, bool record)
    {
        StartCoroutine(MoveCoroutine(type, physical, touchTime, record));
    }
    // Function to check if two GameObjects are at the same rounded position
    public bool ArePositionsEqual(GameObject obj1, GameObject obj2, int decimalPlaces)
    {
        // Get the positions of the GameObjects
        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;

        return compareVector(pos1, pos2);
    }
    public bool ArePositionsEqual(Vector3 pos1, Vector3 pos2, int decimalPlaces)
    {
        return compareVector(pos1, pos2);
    }
    public bool ArePositionsEqual(GameObject obj1, Vector3 pos2, int decimalPlaces)
    {
        Vector3 pos1 = obj1.transform.position;
        Vector3 roundedPos1 = RoundVector(pos1, decimalPlaces);
        Vector3 roundedPos2 = RoundVector(pos2, decimalPlaces);
        return compareVector(pos1, pos2);
    }
    public bool ArePositionsEqual(Vector3 pos1, GameObject obj2, int decimalPlaces)
    {
        Vector3 pos2 = obj2.transform.position;
        return compareVector(pos1, pos2);
    }
    public bool compareVector(Vector3 v1, Vector3 v2)
    {

        double v1x = ((int)((v1.x) * 1000) / 1000.0);
        double v1y = ((int)((v1.y) * 1000) / 1000.0);
        double v1z = ((int)((v1.z) * 1000) / 1000.0);
        double v2x = ((int)((v2.x) * 1000) / 1000.0);
        double v2y = ((int)((v2.y) * 1000) / 1000.0);
        double v2z = ((int)((v2.z) * 1000) / 1000.0);

        bool ret = ((Math.Abs(v1x - v2x) <= 0.015) && (Math.Abs(v1y - v2y) <= 0.015) && (Math.Abs(v1z - v2z) <= 0.015));
        return ret;
        //return (v1x == v2x && v1y == v2y && v1z == v2z);
    }
    private Vector3 RoundVector(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }
    public bool isAtPos(GameObject target)
    {
        return isAtPos(target.transform.position);
    }
    public bool isAtPos(GameObject target, bool swap)
    {
        return isAtPos(target.transform.position,swap);
    }
    public bool isAtPos(Vector3 target, bool swap)
    {
        Vector3 rpos;
        if (swap)
        {
            
            rpos = new Vector3((float)actuator.transform.position.x, (float)actuator.transform.position.y, (float)actuator.transform.position.z);
            //rpos = new Vector3((float)x, (float)z, (float)y);
        }
        else
        {

            rpos = new Vector3((float)actuator.transform.position.x, (float)actuator.transform.position.y, (float)actuator.transform.position.z);
            //rpos = new Vector3((float)x, (float)y, (float)z);
        }
        
        return compareVector(target, rpos);
    }
    public bool isAtPos(Vector3 target)
    {
        //Vector3 rpos = new Vector3((float)x, (float)y, (float)z);

        //rpos = new Vector3((float)actuator.transform.position.x, (float)actuator.transform.position.y, (float)actuator.transform.position.z);
        return isAtPos(target, false);
    }

    public IEnumerator DetermineTouchPressure()
    {
        //hardPos; lightPos; noPos;
        float currentHeight = safeYPos;
        double pressure = 0;

        float noY = -1;
        float lightY = -1;
        float hardY = -1;
        Vector3 originHover = handTouchPos.transform.position;
        Vector3 safeSpace = new Vector3(handTouchPos.transform.position.x, hoverSpace.transform.position.y, handTouchPos.transform.position.z);
        Vector3 targetPos = originHover;
        targetPos.y = currentHeight;
        target.transform.position = targetPos;
        move = true;

        while (! isAtPos(target.transform.position, false))
        {
            if (verbose)
            {
                Debug.Log("Going target1");
            }
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        if (verbose)
        {
            Debug.Log("Calibrating pressure");
        }

        while ((noY == -1 || lightY == -1 || hardY == -1) && currentHeight > unsafeYPos)
        {
            //Return to hover pos
            /*targetPos.y = safeYPos;
            target.transform.position = targetPos;
            move = true;

            while (!isAtPos(target.transform.position, false))
            {
                Debug.Log("Going hover");
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }*/

            //lower target pos and go there
            currentHeight -= 0.002f;
            targetPos.y = currentHeight;
            target.transform.position = targetPos;
            while (!isAtPos(target.transform.position, false))
            {
                if (verbose)
                {
                    Debug.Log("Going target2");
                }
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
            //Wait a second and check pressure
            yield return new WaitForSecondsRealtime(2);
            List<double> pres = new List<double>() ;
            for (int r = 0; r < 20; r++)
            {
                pres.Add(Fz);
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
            
            pressure = pres.Average();
            if (pressure < 10)
            {
                noY = currentHeight;
                defPos("No");
                if (verbose)
                {
                    Debug.Log("No pressure : " + pressure.ToString());
                }
            }
            if(pressure > 15 && pressure < 25)
            {
                lightY = currentHeight;
                defPos("Light");
                if (verbose)
                {
                    Debug.Log("Light pressure : " + pressure.ToString());
                }
            }
            if(pressure>25 && hardY == -1)
            {
                hardY = currentHeight;
                defPos("Hard");
                if (verbose)
                {
                    Debug.Log("Hard pressure : " + pressure.ToString());
                }
            }
            if (verbose)
            {
                Debug.Log("Current pressure : " + pressure.ToString());
            }
        }
        if (verbose)
        {
            Debug.Log("Determining hover pos");
        }
        target.transform.position = safeSpace;
        /*while (!isAtPos(target.transform.position, false))
        {
            Debug.Log("Going target2");
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }*/
        yield return new WaitForSecondsRealtime(movementTime);
        hoverSpace.transform.position = new Vector3(handTouchPos.transform.position.x, actuator.transform.position.y, handTouchPos.transform.position.z);
        fakehoverSpace.transform.position = new Vector3(handTouchPos.transform.position.x, actuator.transform.position.y, handTouchPos.transform.position.z);

        target.transform.position = hoverSpace.transform.position;
        move = false;
        goHover = true;
        while (!isAtPos(hoverSpace.transform.position, false))
        {
            if (verbose)
            {
                Debug.Log("Going target2");
            }
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }

        goHover = false;
        yield break;
    }

    public void savePosition(string filePos,string fileJoints)
    {
        SaveVector3Values(filePos, noPos.transform.position, lightPos.transform.position, hardPos.transform.position, hoverSpace.transform.position);
        fake.SaveData(fileJoints);
    }
    public void saveAlignData()
    {
        SaveVector3Values(fileAlign, alignLeapSave, alignAvatarSave, alignRobotSave);
    }
    public void loadAlignData()
    {
        LoadVectorAlign(fileAlign);
    }
    public IEnumerator loadPosition(string filePos, string fileJoints)
    {
        LoadVector4Values(filePos);
        fake.LoadData(fileJoints);
        yield return new WaitForSecondsRealtime((float)0.1);
        RobotPositionData r0 = fake.recordedPositionsHard[0];
        fake.blockMimic = true;
        for (int i = 0; i < 6; i++)
        {

            fake.joints[i].localRotation = r0.jointPositions[i];
        }

        yield return new WaitForSecondsRealtime((float)0.1);

        hoverSpace.transform.position = new Vector3(fakeActuator.transform.position.x, fakeActuator.transform.position.y, fakeActuator.transform.position.z);
        fakehoverSpace.transform.position = new Vector3(fakeActuator.transform.position.x, fakeActuator.transform.position.y, fakeActuator.transform.position.z);
        yield return new WaitForSecondsRealtime((float)0.1);
        goHover = true;
        fake.blockMimic = false;

        if (noPos != null)
        {
            noPos.transform.position = new Vector3(hoverSpace.transform.position.x, noPos.transform.position.y, hoverSpace.transform.position.z);
        }
        if (lightPos != null)
        {
            lightPos.transform.position = new Vector3(hoverSpace.transform.position.x, lightPos.transform.position.y, hoverSpace.transform.position.z);
        }
        if (hardPos != null)
        {
            hardPos.transform.position = new Vector3(hoverSpace.transform.position.x, hardPos.transform.position.y, hoverSpace.transform.position.z);
        }
    }

    public void SaveVector3Values(string filePath, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            WriteVector3(writer, v1);
            WriteVector3(writer, v2);
            WriteVector3(writer, v3);
            WriteVector3(writer, v4);
        }
    }
    public void SaveVector3Values(string filePath, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            WriteVector3(writer, v1);
            WriteVector3(writer, v2);
            WriteVector3(writer, v3);
        }
    }
    public void LoadVector4Values(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            Vector3 v1 = ReadVector3(reader);
            if (noPos == null)
            {
                noPos = Instantiate(instTarget);
                noPos.transform.position = new Vector3(handTouchPos.transform.position.x, v1.y, handTouchPos.transform.position.z);
            }
            Vector3 v2 = ReadVector3(reader);
            if (lightPos == null)
            {
                lightPos = Instantiate(instTarget);
                lightPos.transform.position = new Vector3(handTouchPos.transform.position.x, v2.y, handTouchPos.transform.position.z);
            }
            Vector3 v3 = ReadVector3(reader);
            if (hardPos == null)
            {
                hardPos = Instantiate(instTarget);
                hardPos.transform.position = new Vector3(handTouchPos.transform.position.x, v3.y, handTouchPos.transform.position.z);
            }
            Vector3 v4 = ReadVector3(reader);
            hoverSpace.transform.position = new Vector3(handTouchPos.transform.position.x, v4.y, handTouchPos.transform.position.z);
            fakehoverSpace.transform.position = new Vector3(handTouchPos.transform.position.x, v4.y, handTouchPos.transform.position.z);


        }
    }
    public void LoadVectorAlign(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            Vector3 v1 = ReadVector3(reader);
            loadAlign("Leap", v1);
            Vector3 v2 = ReadVector3(reader);
            loadAlign("Avatar", v2);
            Vector3 v3 = ReadVector3(reader);
            loadAlign("Robot", v3);
        }
    }
    private Vector3 ReadVector3(StreamReader reader)
    {
        float x = float.Parse(reader.ReadLine());
        float y = float.Parse(reader.ReadLine());
        float z = float.Parse(reader.ReadLine());

        return new Vector3(x, y, z);
    }
    private void WriteVector3(StreamWriter writer, Vector3 vector)
    {
        writer.WriteLine(vector.x);
        writer.WriteLine(vector.y);
        writer.WriteLine(vector.z);
    }



}
