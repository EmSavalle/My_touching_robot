using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Globalization;
using UnityEngine.XR;
public class URTest : MonoBehaviour
{
    public GameObject robotObject;
    public UR5 robot;
    public GameObject target;
    public GameObject instTarget;
    public GameObject actuator;
    public GameObject safeSpace;
    public GameObject hoverSpace;


    public bool goSafe;
    public bool goHover;
    public bool move = false;



    /* public GameObject tcp;
     public double unityX;
     public double unityY;
     public double unityZ;
     public double unityToRobotX;
     public double unityToRobotY;
     public double unityToRobotZ;*/

    [Header("Target Position")]
    public double npx;
    public double npy;
    public double npz;

    [Header("Simple move")]
    public GameObject limits;
    public float moveDelay = 1f;
    public float speed = 0.1f;



    [Header("Program")]
    public bool send;
    public string textprog;

    [Header("Teach")]
    private bool isTeaching = false;
    public bool teach;

    private DateTime lastMove;
    private List<UnityEngine.XR.InputDevice> gameControllers;


    [Header("Alignement")]
    public bool alignAvatar = false;
    private bool isAvatarAligned = false;
    public bool alignLeap = false;
    private bool isLeapAligned = false;
    public bool alignRobot = false;
    private bool isRobotAligned = false;
    public bool instantiateTarget = false;

    public bool safePos;
    private bool hassafePos;
    public bool hoverPos;
    private bool hashoverPos;


    public GameObject alignment;
    public GameObject alignmentPointRobot;
    public List<GameObject> movableRobot;

    public GameObject alignmentPointLeap;
    public List<GameObject> movableLeap;
    public GameObject actualAlignmentPointLeap;//Avatar left hand

    public GameObject alignmentPointAvatar;
    public List<GameObject> movableAvatar;
    public GameObject actualAlignment;

    [Header("Forces & speeds")]
    private double Fx;
    private double Fy;
    private double Fz;
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
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        UnityEngine.XR.InputDevice ldevice, rdevice;
        //ldevice = leftHandDevices[0];
        //rdevice = rightHandDevices[0];
        //Swapped y and z to correspond to robot space
        npx = ((int)(target.transform.position.x * 100) / 100.0);
        npz = ((int)(target.transform.position.y * 100) / 100.0);
        npy = ((int)(target.transform.position.z * 100) / 100.0);
        Transform transform = robotObject.transform;
        double offsetx = transform.position.x;
        double offsety = transform.position.z;
        double offsetz = transform.position.y;
        npx = npx - ((int)(offsetx * 100) / 100.0);
        npy = npy - ((int)(offsety * 100) / 100.0);
        npz = npz - ((int)(offsetz * 100) / 100.0);

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
            Fx = ((int)(forces[0] * 100) / 100.0);
            Fy = ((int)(forces[1] * 100) / 100.0);
            Fz = ((int)(forces[2] * 100) / 100.0);
            Mx = ((int)(forces[3] * 100) / 100.0);
            My = ((int)(forces[4] * 100) / 100.0);
            Mz = ((int)(forces[5] * 100) / 100.0);

            forces = robot.GetTCPSpeed();
            Sx = ((int)(forces[0] * 100) / 100.0);
            Sy = ((int)(forces[1] * 100) / 100.0);
            Sz = ((int)(forces[2] * 100) / 100.0);
            Ssx = ((int)(forces[3] * 100) / 100.0);
            Ssy = ((int)(forces[4] * 100) / 100.0);
            Ssz = ((int)(forces[5] * 100) / 100.0);

            double[] pos = robot.GetTCPPosition();
            x = ((int)(pos[0] * 100) / 100.0);
            y = ((int)(pos[1] * 100) / 100.0);
            z = ((int)(pos[2] * 100) / 100.0);
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
                Debug.Log("Teaching");
                isTeaching = true;
                string prog = "";

                prog += "teach_mode()\n";

                robot.ur.SendProgram(prog);
            }

            if (!teach && isTeaching)
            {
                Debug.Log("unTeaching");
                isTeaching = false;
                string prog = "";
                prog += "end_teach_mode()\n";

                robot.ur.SendProgram(prog);
            }
            if (move)
            {
                goTo(target, false);
            }
            if (safePos)
            {
                defSafeSpace();
                safePos = false;
            }
            if (hoverPos)
            {
                defHover();
                hoverPos = false;
            }
            if (goSafe)
            {
                safe();
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
        if (alignRobot)
        {
            Debug.Log("Aligning robot");
            align("Robot");
            alignRobot = false;
        }
        if (alignAvatar)
        {
            Debug.Log("Aligning Avatar");
            align("Avatar");
            alignAvatar = false;
        }
        if (alignLeap)
        {
            Debug.Log("Aligning Leap");
            align("Leap");
            alignLeap = false;
        }
        if (instantiateTarget)
        {
            acquireTarget();
            instantiateTarget = false;

            foreach (GameObject m in movableAvatar)
            {
                //Get hand lowest point, set object to that height
            }
        }
    }

    public void align(String mode)
    {
        if(mode == "Robot")
        {
            // Move the robot so the tcp object lign up with the physical alignement point
            Vector3 offset = alignment.transform.position - alignmentPointRobot.transform.position;

            foreach (GameObject go in movableRobot)
            {
                go.transform.position += offset;
            }
            isRobotAligned = true;

        }
        else if (mode == "Avatar")
        {
            // Move the robot so the right hand model lign up with the physical alignement point

            actualAlignment = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand/Tracked Root L Hand").gameObject;
            if(actualAlignment == null)
            {
                Debug.LogError("No right hand found");
            }
            Vector3 offset = actualAlignment.transform.position - alignment.transform.position;

            foreach (GameObject go in movableAvatar)
            {
                go.transform.position += offset;
            }
            isAvatarAligned = true;

        }
        else if (mode == "Leap")
        {
            if (actualAlignmentPointLeap == null)
            {
                for (int i = 0; i < alignmentPointAvatar.transform.childCount; i++)
                {
                    String Go = alignmentPointAvatar.transform.GetChild(i).name;
                    if (Go.Contains("Adult") || Go.Contains("Child") || Go.Contains("Male") || Go.Contains("Female"))
                    {
                        actualAlignmentPointLeap = alignmentPointAvatar.transform.GetChild(i).transform.Find("CC_Base_BoneRoot/CC_Base_Hip/CC_Base_Waist/CC_Base_Spine01/CC_Base_Spine02/CC_Base_L_Clavicle/CC_Base_L_Upperarm/CC_Base_L_Forearm/CC_Base_L_Hand").gameObject;
                    }
                }
            }
            Vector3 offset = alignment.transform.position - actualAlignmentPointLeap.transform.position;
            float zz = offset.x;
            offset.x = -offset.z;
            offset.z = zz;
            foreach (GameObject go in movableLeap)
            {
                go.transform.position += offset;
            }
            isLeapAligned = true;
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

    public void goTo(GameObject to, Boolean followRotation)
    {
        //Swapped y and z to correspond to robot space
        npx = ((int)(to.transform.position.x * 100) / 100.0);
        npz = ((int)(to.transform.position.y * 100) / 100.0);
        npy = ((int)(to.transform.position.z * 100) / 100.0);
        Transform transform = robotObject.transform;
        double offsetx = transform.position.x;
        double offsety = transform.position.z;
        double offsetz = transform.position.y;
        npx = npx - ((int)(offsetx * 100) / 100.0);
        npy = npy - ((int)(offsety * 100) / 100.0);
        npz = npz - ((int)(offsetz * 100) / 100.0);
        /*double rpx = ((int)(Mathf.Deg2Rad * -1*(target.transform.rotation.eulerAngles.x-180) * 100) / 100.0);
        double rpz = ((int)(Mathf.Deg2Rad * target.transform.rotation.eulerAngles.y * 100) / 100.0);
        double rpy = ((int)(Mathf.Deg2Rad * target.transform.rotation.eulerAngles.z * 100) / 100.0);*/


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
            if (limits.GetComponent<Collider>().bounds.Contains(to.transform.position))
            {
                if ((x != npx || y != npy || z != npz) && (Sx == 0 && Sy == 0 && Sz == 0 && Ssx == 0 && Ssy == 0 && Ssz == 0 && (DateTime.Now - lastMove).TotalSeconds > moveDelay))
                {
                    string prog = "movel(p[" + npx.ToString(CultureInfo.InvariantCulture) + "," + npy.ToString(CultureInfo.InvariantCulture) + "," + npz.ToString(CultureInfo.InvariantCulture) + "," + rpx.ToString(CultureInfo.InvariantCulture) + "," + rpy.ToString(CultureInfo.InvariantCulture) + "," + rpz.ToString(CultureInfo.InvariantCulture) + "], a = 1.4, v = 1.05, t = 0, r = 0)" + "\n";
                    lastMove = DateTime.Now;
                    Debug.Log(prog);
                    robot.ur.SendProgram(prog);

                }
            }
            else
            {
                Debug.Log("Object outside of limits");
            }
        }
    }

    public void defSafeSpace()
    {
        safeSpace.transform.position = actuator.transform.position;
        hassafePos = true;
    }
    public void defHover()
    {
        hoverSpace.transform.position = actuator.transform.position;
        hashoverPos = true;
    }

    public void safe()
    {
        move = false;
        goHover = false;
        if (hassafePos)
        {
            Debug.Log("Going to safe space");
            goTo(safeSpace, false);
        }
    }
    public void hover()
    {
        move = false;
        goSafe = false;
        if (hashoverPos)
        {
            Debug.Log("Hovering");
            goTo(hoverSpace, false);
        }
    }
    public void acquireTarget()
    {
        if (isLeapAligned)
        {
            baseHand = actualAlignmentPointLeap;
            baseMiddle = actualAlignmentPointLeap.transform.Find("CC_Base_L_Mid1").gameObject;
            target = Instantiate(instTarget);
            target.transform.position = Vector3.Lerp(baseHand.transform.position, baseMiddle.transform.position, 0.5f) ;
            isTargetInstantiated = true;
        }
    }
}
