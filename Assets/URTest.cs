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

    [Header("Forces")]
    private double Fx;
    private double Fy;
    private double Fz;
    private double Mx;
    private double My;
    private double Mz;
    [Header("Speeds")]
    private double Sx;
    private double Sy;
    private double Sz;
    private double Ssx;
    private double Ssy;
    private double Ssz;

    [Header("Position")]
    public double x;
    public double y;
    public double z;


    [Header("Simple move")]
    public bool move=false;
    public float moveDelay = 1f;
    public float speed = 0.1f;
    public double rx;
    public double ry;
    public double rz;

    [Header("Rotating it ")]
    double angle = 0;
    public bool rotate = false;
    double a = 0.7;
    double v = 0.55;
    double t = 0;
    double r = 0;

    [Header("Program")]
    public bool send;
    public string textprog;

    [Header("Teach")]
    private bool isTeaching = false;
    public bool teach;

    private DateTime lastMove;
    private List<UnityEngine.XR.InputDevice> gameControllers;
    public GameObject contact;
    public GameObject movable;
    public GameObject alignment;
    public bool moved = false;
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
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        UnityEngine.XR.InputDevice device;
        device = leftHandDevices[0];
        bool triggerValue;
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
        {
            Debug.Log("Trigger button is pressed");
            Vector3 offset = contact.transform.position - alignment.transform.position;
            movable.transform.position -= offset;
            moved = true;
        }


        //Swapped y and z to correspond to robot space
        double npx = ((int)(target.transform.position.x * 100 + .5) / 100.0);
        double npz = ((int)(target.transform.position.y * 100 + .5) / 100.0);
        double npy = ((int)(target.transform.position.z * 100 + .5) / 100.0);
        /*double rpx = ((int)(Mathf.Deg2Rad * -1*(target.transform.rotation.eulerAngles.x-180) * 100 + .5) / 100.0);
        double rpz = ((int)(Mathf.Deg2Rad * target.transform.rotation.eulerAngles.y * 100 + .5) / 100.0);
        double rpy = ((int)(Mathf.Deg2Rad * target.transform.rotation.eulerAngles.z * 100 + .5) / 100.0);*/


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
        
        rx = rpx; ry = rpy; rz = rpz;
        
        /*double rpx = Mathf.Deg2Rad*rx;
        double rpy = Mathf.Deg2Rad*ry;    
        double rpz = Mathf.Deg2Rad * rz; */
        if (robot.IsInitialized)
        {
            double[] forces = robot.GetTCPForce();
            Fx = ((int)(forces[0] * 100 + .5) / 100.0);
            Fy = ((int)(forces[1] * 100 + .5) / 100.0);
            Fz = ((int)(forces[2] * 100 + .5) / 100.0);
            Mx = ((int)(forces[3] * 100 + .5) / 100.0);
            My = ((int)(forces[4] * 100 + .5) / 100.0);
            Mz = ((int)(forces[5] * 100 + .5) / 100.0);
            double[] pos = robot.GetTCPPosition();
            x = ((int)(pos[0] * 100 + .5) / 100.0);
            y = ((int)(pos[1] * 100 + .5) / 100.0);
            z = ((int)(pos[2] * 100 + .5) / 100.0);

            forces = robot.GetTCPSpeed();
            Sx = ((int)(forces[0] * 100 + .5) / 100.0);
            Sy = ((int)(forces[1] * 100 + .5) / 100.0);
            Sz = ((int)(forces[2] * 100 + .5) / 100.0);
            Ssx = ((int)(forces[3] * 100 + .5) / 100.0);
            Ssy = ((int)(forces[4] * 100 + .5) / 100.0);
            Ssz = ((int)(forces[5] * 100 + .5) / 100.0);
            

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
                //move = false;
                x = ((int)(pos[0] * 100 + .5) / 100.0);
                y = ((int)(pos[1] * 100 + .5) / 100.0);
                z = ((int)(pos[2] * 100 + .5) / 100.0);
                if ((x != npx || y != npy || z != npz) && (Sx == 0 && Sy == 0 && Sz == 0 && Ssx == 0 && Ssy == 0 && Ssz == 0 && (DateTime.Now - lastMove).TotalSeconds> moveDelay))
                {
                    string prog = "movel(p[" + npx.ToString(CultureInfo.InvariantCulture) + "," + npy.ToString(CultureInfo.InvariantCulture) + "," + npz.ToString(CultureInfo.InvariantCulture) + "," + rpx.ToString(CultureInfo.InvariantCulture) + "," + rpy.ToString(CultureInfo.InvariantCulture) + "," + rpz.ToString(CultureInfo.InvariantCulture) + "], a = 1.4, v = 1.05, t = 0, r = 0)" + "\n";
                    lastMove = DateTime.Now;
                    Debug.Log(prog);
                    robot.ur.SendProgram(prog);

                }
            }
        }
        /*if (send)
        {
            send = false;
            Debug.Log("Connecting");
            TcpClient client = new TcpClient();
            client.Connect("169.254.70.112", 30002);

            NetworkStream stream = client.GetStream();

            string output = "movej(p[0.20,0.20, 0.50, 0.0, 2.20, 2.20], a = 1.4, v = 1.05, t = 0, r = 0)" + "\n";
            byte[] data = Encoding.ASCII.GetBytes(output);

            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Debug.Log("Received: " + receivedData);

            stream.Close();
            client.Close();
            //send = false;
            //robot.ur.SendProgram(textprog);
        }*/


    }
}
