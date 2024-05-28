using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Globalization;
using UnityEngine.XR;
using System.Linq;
public class URTest : MonoBehaviour
{


    public GameObject robotObject;
    public UR5 robot;
    public FakeUR fake;
    public bool dupl;
    public GameObject target;
    public float targetOffsetx, targetOffsety, targetOffsetz;
    public GameObject instTarget;
    public GameObject actuator;
    public GameObject safeSpace;
    public GameObject hoverSpace;


    public bool goSafe;
    public bool goHover;
    public bool move = false;
    public float movementTime = 1;



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

    public float tableYOffset = 0f;
    public bool calibrateHand;
    private bool calibratingHand = false;
    private GameObject leftHand, rightHand;


    [Header("Robot positions")]
    public GameObject handTouchPos;
    private GameObject locationBase, locationMid;

    public bool safePos;
    private bool hassafePos;
    public bool hoverPos;
    private bool hashoverPos;
    public GameObject noPos, lightPos, hardPos;
    public Vector3 vnoPos, vlightPos, vhardPos;
    public bool setnoPos, setlightPos, sethardPos;
    public bool calibratePressure;
    private bool hasnoPos, haslightPos, hashardPos;
    private bool manualSet = true;
    public int noPosPressure, lightPosPressure, hardPosPressure;
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



    public float offsetHandx, offsetHandy, offsetHandz;
    public float simulatedOffsetx, simulatedOffsety, simulatedOffsetz;

    [Header("Testing coroutine")]
    public string moveCouroutineStep;
    public bool movingCoroutine;
    public bool pCphy, pCrec;
    public string pCtype;
    public bool testCoroutine;

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
        updateHandTouchPos();
        if (testCoroutine)
        {
            testCoroutine = false; 
            StartCoroutine(MoveCoroutine(pCtype, pCphy, 1, pCrec));
        }
        if (calibrateHand && !calibratingHand)
        {
            if(leftHand == null)
            {
                leftHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand").gameObject;
                rightHand = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/RightHand").gameObject;
            }
            leftHand.SetActive(false);
            calibratingHand = true;
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
                target.transform.position = hoverSpace.transform.position;
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
            target.transform.position = Vector3.Lerp(baseHand.transform.position, baseMiddle.transform.position, 0.5f)+new Vector3(targetOffsetx, targetOffsety, targetOffsetz);
        }
        if (calibratePressure)
        {
            StartCoroutine(DetermineTouchPressure());
            calibratePressure = false;
        }
        /*if (sethardPos)
        {
            sethardPos = false;
            defPos("Hard");
        }
        if (setlightPos)
        {
            setlightPos = false;
            defPos("Light");
        }
        if (setnoPos)
        {
            setnoPos = false;
            defPos("No");
        }*/

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

    internal void StartMovement(string tType, bool phy, bool v, object touchTime, bool record)
    {
        throw new NotImplementedException();
    }

    public void align(String mode)
    {
        if(mode == "Robot")
        {
            // Move the robot so the tcp object lign up with the physical alignement point
            Vector3 offset = handTouchPos.transform.position - alignmentPointRobot.transform.Find("AlignementPointRobot").transform.position;

            foreach (GameObject go in movableRobot)
            {
                go.transform.position += offset;
            }
            isRobotAligned = true;

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
            isAvatarAligned = true;

        }*/
        else if (mode == "AvatarHand" || mode == "Avatar")
        {
            // Move the unity scene so the right hand model lign up with the physical alignement point

            actualAlignment = alignmentPointAvatar.transform.Find("AvatarCCHandsInteractionLeap(Clone)/LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
            if (actualAlignment == null)
            {
                Debug.LogError("No left hand found");
            }
            Vector3 offset = alignment.transform.position - actualAlignment.transform.position;
            offset.x = 0;offset.z = 0;
            offset.y += tableYOffset;
            foreach (GameObject go in movableAvatar)
            {
                go.transform.position -= offset;
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
                    if (Go.Contains("AvatarCCHandsInteractionLeap"))
                    {
                        actualAlignmentPointLeap = alignmentPointAvatar.transform.GetChild(i).transform.Find("LeftHand/Tracked Root L Hand/L Hand/CC_Base_L_Middle1").gameObject;
                    }
                }
            }


           
            foreach (GameObject go in movableLeap)
            {
                // Rotate the object to align with global forward
                //go.transform.rotation = targetRotation;

                // Calculate the position offset
                Vector3 offset = alignment.transform.position - actualAlignmentPointLeap.transform.position;
                offset.x += offsetHandx;
                offset.y += offsetHandy;
                offset.z += offsetHandz;

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
            handTouchPos = Instantiate(new GameObject("HandTouch"));

            handTouchPos.AddComponent<Rigidbody>();
            //handTouchPos.AddComponent<BoxCollider>();
            //handTouchPos.GetComponent<BoxCollider>().isTrigger = true;
        }
        float x = locationBase.transform.position.x + (locationMid.transform.position.x - locationBase.transform.position.x) / 2;
        float y = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) / 2+0.02f;
        float z = locationBase.transform.position.z + (locationMid.transform.position.z - locationBase.transform.position.z) / 2;
        handTouchPos.transform.position = new Vector3(x, y, z);
    }
    public void updateHandTouchPos()
    {
        if(handTouchPos != null)
        {
            float x = locationBase.transform.position.x + (locationMid.transform.position.x - locationBase.transform.position.x) / 2;
            float y = locationBase.transform.position.y + (locationMid.transform.position.y - locationBase.transform.position.y) / 2+0.02f;
            float z = locationBase.transform.position.z + (locationMid.transform.position.z - locationBase.transform.position.z) / 2;
            handTouchPos.transform.position = new Vector3(x, y, z);
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
            dx = ((int)((to.x + targetOffsetx) * 1000) / 1000.0);
            dy = ((int)((to.y + targetOffsety) * 1000) / 1000.0);
            dz = ((int)((to.z + targetOffsetz) * 1000) / 1000.0);
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
        npx = ((int)((to.transform.position.x) * 1000) / 1000.0);
        npz = ((int)((to.transform.position.y) * 1000) / 1000.0);
        npy = ((int)((to.transform.position.z) * 1000) / 1000.0);

        Transform transform = robotObject.transform;
        double offsetx = transform.position.x;
        double offsety = transform.position.z;
        double offsetz = transform.position.y;
        npx = npx - ((int)(offsetx * 1000) / 1000.0);
        npy = npy - ((int)(offsety * 1000) / 1000.0);
        npz = npz - ((int)(offsetz * 1000) / 1000.0);
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
            else
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
        hashoverPos = true;
    }

    public void safe()
    {
        move = false;
        goHover = false;
        if (hassafePos || manualSet)
        {
            Debug.Log("Going to safe space");
            goTo(safeSpace, false);
        }
    }
    public void hover()
    {
        move = false;
        goSafe = false;
        if (hashoverPos || manualSet)
        {
            Debug.Log("Hovering");
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
        Debug.Log("Starting movement coroutine");
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

        if (!physical)
        {
            robotRig.SetActive(false);
            ballRig.GetComponent<MeshRenderer>().enabled = false;

        }
        Debug.Log("Coroutine intialisation");

        //Step 1 : go to original position 
        Vector3 robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y,(float)z));
        if (!isAtPos(hoverSpace, false))
        {
            Debug.Log("Coroutine hovering");
            moveCouroutineStep = "GoingHover"; 
            robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            while (!isAtPos(hoverSpace, false))
            {
                hover();
                yield return new WaitForSeconds((float)0.5);
            }
        }
        //Step 2 : go to touch position
        moveCouroutineStep = "MovingTouch";
        Debug.Log("Coroutine MovingTouch");
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
                }
            }
            moveCouroutineStep = "Touch";
            while (!isAtPos(target, false))
            {
                if (physical) { goTo(target); }

                yield return new WaitForSeconds((float)0.1);
                robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            }
            Debug.Log("Coroutine Touching");
            moveCouroutineStep = "Touching";
            yield return new WaitForSeconds(touchTime);

            //Step 3 : Go back

            moveCouroutineStep = "Backing";
            Debug.Log("Coroutine Backing");

            moveCouroutineStep = "GoingHover"; robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            while (!isAtPos(hoverSpace, false))
            {
                if (physical) { hover(); }

                yield return new WaitForSeconds((float)0.5);
                robotSpace = transformPosToRobotPosition(new Vector3((float)x, (float)y, (float)z));
            }
            moveCouroutineStep = "Ending";
            Debug.Log("Coroutine Ending");
            if (record && physical)
            {
                fake.StopRecording(type);
            }
        }
        if (!physical)
        {
            while (fake.replaying)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        movingCoroutine = false;
        robotRig.SetActive(true);
        ballRig.GetComponent<MeshRenderer>().enabled = true;
        fakeRig.SetActive(true);
        robotRig.SetActive(true);
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
        Vector3 targetPos = originHover;
        targetPos.y = currentHeight;
        target.transform.position = targetPos;
        move = true;

        while (! isAtPos(target.transform.position, false))
        {
            Debug.Log("Going target1");
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Debug.Log("Calibrating pressure");

        while ((noY == -1 || lightY == -1 || hardY == -1) && currentHeight > unsafeYPos)
        {
            //Return to hover pos
            targetPos = originHover;
            targetPos.y = safeYPos;
            target.transform.position = targetPos;
            move = true;

            while (!isAtPos(target.transform.position, false))
            {
                Debug.Log("Going hover");
                yield return new WaitForSeconds(Time.deltaTime);
            }

            //lower target pos and go there
            currentHeight -= 0.002f;
            targetPos = originHover;
            targetPos.y = currentHeight;
            target.transform.position = targetPos;
            while (!isAtPos(target.transform.position, false))
            {
                Debug.Log("Going target2");
                yield return new WaitForSeconds(Time.deltaTime);
            }
            //Wait a second and check pressure
            yield return new WaitForSeconds(2);
            List<double> pres = new List<double>() ;
            for (int r = 0; r < 10; r++)
            {
                pres.Add(Fz);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            
            pressure = pres.Average();
            if (pressure < 10)
            {
                noY = currentHeight;
                defPos("No");
            }
            if(pressure>15 && lightY == -1)
            {
                lightY = currentHeight;
                defPos("Light");
            }
            if(pressure>25 && hardY == -1)
            {
                hardY = currentHeight;
                defPos("Hard");
            }
        }
        target.transform.position = originHover;
        while (!isAtPos(target.transform.position, false))
        {
            Debug.Log("Going target2");
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Vector3 towardHover = new Vector3(originHover.x, originHover.y + 0.1f, originHover.z);
        yield return new WaitForSeconds(movementTime);
        hoverSpace.transform.position = new Vector3(alignmentPointRobot.transform.position.x, alignmentPointRobot.transform.position.y, alignmentPointRobot.transform.position.z);
                
        move = false;
        goHover = true;
        while (!isAtPos(hoverSpace.transform.position, false))
        {
            Debug.Log("Going target2");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        goHover = false;
        yield break;
    }
}
