using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using SerialPortUtility;
using Enablegames;
using Enablegames.Suki;

namespace EAGSerialHandler
{
    public class SerialHandler : MonoBehaviour
    {
        public bool fakeData = true;
        public SkeletonData skeletonData;
        public RoboticData roboticData;

        ///Biometric data from separate communication 
        public RoboticDatum roboticDatum;

        ///Biometric data from separate communication 
        public BiometricData biometricData;

        ///Biometric data from separate communication 
        public BiometricDatum biometricDatum;

        ///Biometric data from separate communication
        public int forceLevel;

        private float _setPointPercentage;
        public int setPoint;
        int heartrate;
        float galvanicSkin;
        float robotAngle;
        float pinchAngle;
        public static float baselineGSR;
        public static int baselineHR;
        private egBool assistiveMode = false;
        private bool ended = false;

        public egString sukiType
        {
            get { return _sukiType; }
            set
            {
                _sukiType = value;
                if (value == "RR1Vertical.suki" || value == "LR1Vertical.suki")
                {
                    if (assistiveMode)
                    {
                        controlMode = 1;
                    }
                    else
                    {
                        controlMode = 2;
                    }
                }
                else if (value == "R1Horizontal.suki")
                {
                    if (assistiveMode)
                    {
                        controlMode = 4;
                    }
                    else
                    {
                        controlMode = 5;
                    }
                }
                else if (value == "R1P&B.suki")
                {
                    if (assistiveMode)
                    {
                        controlMode = 7;
                    }
                    else
                    {
                        controlMode = 8;
                    }
                }
            }
        }

        private egString _sukiType = "LR1Vertical.suki";

        // General Variables/

        int theta1 = 0;
        int force1 = 0;
        int button1 = 0;
        int aX1 = 0;
        int aY1 = 0;
        int aZ1 = 0;
        int aZ2 = 0;
        bool aB1 = false;
        int GSR1 = new int();
        int BVP1 = new int();
        private int theta2 = 0;

        public static string userid = "unknownID";

        private bool gameEnded = false;

        private egBool flipNeed = false;

        private SerialPortUtilityPro serialPort;
        private List<string> TheraDriveArray;

        private bool initialized = false;

        private int _setPoint;
        private int controlMode;
        private int _controlMode;
        private int _forceLvl;
        private int offset = 0;

        [SerializeField] private float heartbeatInterval = 5f;



        void Awake()
        {
            DDAManager.Instance.onDifficultyChanging.AddListener(UpdateParameters);
            VariableHandler.Instance.Register("Suki file", sukiType);
            try
            {
                VariableHandler.Instance.Register("Assistive Mode", assistiveMode);
                VariableHandler.Instance.Register("Right Hand Mode", flipNeed);
            }
            catch (Exception e)
            {
            }

            if (sukiType == "RR1Vertical.suki" || sukiType == "LR1Vertical.suki")
            {
                if (assistiveMode)
                {
                    controlMode = 1;
                }
                else
                {
                    controlMode = 2;
                }
            }
            else if (sukiType == "R1Horizontal.suki")
            {
                if (assistiveMode)
                {
                    controlMode = 4;
                }
                else
                {
                    controlMode = 5;
                }
            }
            else if (sukiType == "R1P&B.suki")
            {
                if (assistiveMode)
                {
                    controlMode = 7;
                }
                else
                {
                    controlMode = 8;
                }
            }

            userid = PlayerPrefs.GetString("lastUsedUserName");

            TheraDriveArray = new List<string>();

            serialPort = GetComponent<SerialPortUtilityPro>();

            if (fakeData)
            {
                serialPort.enabled = false;
                return;
            }

            egString motionTrackingCategory = "";
            VariableHandler.Instance.Register("Suki type", motionTrackingCategory);
            Debug.Log("Log Suki Type: " + motionTrackingCategory);
            if (!((string)motionTrackingCategory).ToLower().Contains("robo"))
            {
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            StartCoroutine(FPS());
            StartCoroutine(HeartBeatRoutine());
            if (!fakeData)
            {
                serialPort.ReadCompleteEventObject.RemoveAllListeners();
                serialPort.ReadCompleteEventObject.AddListener(ReadSerialInput);
            }


            if (skeletonData == null)
            {
                GameObject go = GameObject.Find("Tracking Avatar");
                if (go != null)
                    skeletonData = go.GetComponentInChildren<SkeletonData>();
            }

            if (skeletonData != null)
            {
                roboticData = skeletonData.roboticData;
                biometricData = skeletonData.biometricData;
            }

            if (roboticData == null)
                roboticData = new RoboticData();
            //       if (roboticData.data.Count <= 1)
            {
                print("SerialHandler:SET RD");
                roboticDatum = new RoboticDatum();
                roboticDatum.Value = 0f;
                roboticData.data["R1"] = roboticDatum;

                roboticDatum = new RoboticDatum();
                roboticDatum.Value = 0f;
                roboticData.data["R2"] = roboticDatum;

                roboticDatum = new RoboticDatum();
                roboticDatum.Value = 0f;
                roboticData.data["B1"] = roboticDatum;
            }

            if (biometricData == null)
                biometricData = new BiometricData();
            //       if (biometricData.data.Count <= 1)
            {
                biometricDatum = new BiometricDatum();
                biometricDatum.Value = 0f;
                biometricData.data["HR"] = biometricDatum;

                biometricDatum = new BiometricDatum();
                biometricDatum.Value = 0f;
                biometricData.data["GSR"] = biometricDatum;
            }
            if (skeletonData != null)
            {
                skeletonData.roboticData = roboticData;
                skeletonData.biometricData = biometricData;
            }
            UpdateParameters();
        }

        private bool IsSerialPortValid()
        {
            return serialPort && serialPort.enabled && serialPort.IsConnected() && serialPort.IsOpened();
        }


        public void ReadSerialInput(object obj)
        {
            string[] array = ((string)obj).Split(',');

            if (array.Length == 0)
            {
                return;
            }

            TheraDriveArray = array.ToList();
        }

        private void ProcessSerialData()
        {
            if (IsSerialPortValid() && !initialized)
            {
                try
                {
                    serialPort.WriteLF("s" + "," + userid + "," + DateTime.Now.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("HH-mm-ss") + "," + Application.productName);
                    initialized = true;
                    ended = false;
                }
                catch (Exception)
                {

                }
            }

            /*
             * WORK IN PROGRESS CODE
             * Variable ID string: Check if data packet contains data labels
             * if (TheraDriveArray[0]=='ID')
             * {
             *      // TheraDriveArray contains comma separated strings to identify variables passed from Arduino
             *      // TheraDriveArray[1] contains the number of variables to follow
             * }
             * else //assign Thera Drive Variables below, as is default
             */

            //Assign Thera Drive Variables
            int.TryParse(TheraDriveArray[0].ToString(), out theta1);
            int.TryParse(TheraDriveArray[1].ToString(), out force1);
            int.TryParse(TheraDriveArray[2].ToString(), out button1);
            int.TryParse(TheraDriveArray[3].ToString(), out aX1);
            int.TryParse(TheraDriveArray[4].ToString(), out aY1);
            int.TryParse(TheraDriveArray[5].ToString(), out aZ1);
            int.TryParse(TheraDriveArray[6].ToString(), out aZ2);
            float.TryParse(TheraDriveArray[7].ToString(), out galvanicSkin);
            int.TryParse(TheraDriveArray[8].ToString(), out heartrate);

            /*
             * WORK IN PROGRESS CODE
             * Serial write to robot
             * current syntax:
             * outputs 1-3 are for the base robot
             * output 0 is for initialization state
             * output 1 is for desired angle or other data point output to controller: three digits
             * output 2 is for controller magnitude/gain: two digits,  0 to 8 to scale resistance/assistance
             * output 3 is controller mode: single digit, 0 for zero impedance, 1 for basic angular assist, 2 for basic angular resist, 3 for dynamic resistance
             *
             * outputs 4-6 are for an accessory or handle with similar syntax to the Thera Drive
             * output 4 is controller mode for accessory: single digit
             * output 5 is controller magnitude/gain for accesory: two digits 0 to 99
             * output 6 is for desired angle or other data point: two or three digits
             */


            if ((setPoint != _setPoint || forceLevel != _forceLvl || controlMode != _controlMode) &&
                IsSerialPortValid())
            {
                try
                {
                    if (serialPort.WriteLF("a" + "," + Convert.ToString(setPoint) + "," + Convert.ToString(forceLevel) +
                                           "," + controlMode.ToString()))
                    {
                        _setPoint = setPoint;
                        _forceLvl = forceLevel;
                        _controlMode = controlMode;
                    }
                }
                catch (Exception)
                {

                }
            }

            TheraDriveArray.Clear();
        }

        private void OnDestroy()
        {
            if (!fakeData && IsSerialPortValid())
            {
                HardGameEnded();
                serialPort.Close();
            }
        }

        void CalculateRobotAngle()
        {
            if (fakeData)
            {
                robotAngle = 100f * Mathf.Sin(Time.time);
            }
            else
            {
                robotAngle = (theta1 - offset);
                if (flipNeed)
                {
                    robotAngle = -robotAngle;
                }
            }

            /*if (!rightState)
            {
                robotAngle = -robotAngle;
            }*/
            pinchAngle = theta2;
        }

        IEnumerator HeartBeatRoutine()
        {
            while (true)
            {
                if (!IsSerialPortValid() || !initialized)
                {
                    yield return null;
                }

                serialPort.WriteLF("h");
                yield return new WaitForSeconds(heartbeatInterval);
            }
        }

        void CalculateHeartrate()
        {
            heartrate = BVP1;
        }

        void CalculateGSR()
        {
            galvanicSkin = GSR1;
        }

        void Update()
        {
            if (skeletonData is null)
            {
                GameObject go = GameObject.Find("Tracking Avatar");
                if (go is not null)
                    skeletonData = go.GetComponentInChildren<SkeletonData>();
                if (skeletonData is not null && skeletonData.roboticData != null)
                {
                    roboticData = skeletonData.roboticData;
                }

                if (skeletonData is not null && skeletonData.biometricData != null)
                {
                    biometricData = skeletonData.biometricData;
                }
            }

            if (skeletonData is not null)
            {
                skeletonData.roboticData = roboticData;
                skeletonData.biometricData = biometricData;
            }

            if (gameEnded && !ended)
            {
                if (TheraDriveArray.Count == 10 && IsSerialPortValid())
                {
                    try
                    {
                        _ = serialPort.WriteLF("e" + "," + userid + "," + DateTime.Now + ":" +
                                               DateTime.Now.Millisecond + "," + Application.productName);
                        ended = true;
                    }
                    catch (Exception)
                    {

                    }

                    return;
                }

                return;
            }

            if (!fakeData)
            {
                ProcessSerialData();
            }

            //CalculateHeartrate();
            CalculateRobotAngle();
            roboticData.SetData("R1", robotAngle, Time.time);
            roboticData.SetData("B1", button1, Time.time);
            roboticData.SetData("R2", pinchAngle, Time.time);
            biometricData.SetData("HR", heartrate, Time.time);
            biometricData.SetData("GSR", galvanicSkin, Time.time);

            if (Input.GetKeyDown(KeyCode.E) && !gameEnded)
            {
                GameEnded();
            }

            if ( /*debugKey.triggered ||*/ Input.GetKeyDown(KeyCode.BackQuote))
            {
                drawGUI = !drawGUI;
            }
        }

        public void GameEnded()
        {
            initialized = false;
            TheraDriveArray = new List<string>();
            gameEnded = true;
        }

        public void Restart()
        {
            gameEnded = false;
        }

        public void ZeroController()
        {
            offset = (int)theta1;
        }

        public bool drawGUI = false;
        public int offsetX = 5;
        public int offsetY = 150;

        public int width = 500, height = 400;

        //public InputAction debugKey;
        private SerialHandler _serialHandler;
        private float count;

        private void OnGUI()
        {
            if (this.drawGUI)
                this.Display(new Rect(offsetX, offsetY, width, height));
        }

        void Display(Rect displayRect)
        {
            GUILayout.BeginArea(displayRect);
            GUILayout.Label(string.Format("FPS: {0}, Suki is updating: {1}, R1: {2}, B1: {3}, HR: {4}, GSR: {5}",
                Math.Ceiling(count), SukiInput.Instance.Updating, robotAngle, button1, heartrate, galvanicSkin));
        }

        private IEnumerator FPS()
        {
            while (true)
            {
                count = 1f / Time.unscaledDeltaTime;
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void SendStartSignal(string name)
        {
            print("Creating New File for: " + "s" + "," + userid + "," + DateTime.Now + "_" + DateTime.Now.Millisecond +
                  "," + name);
            gameEnded = false;
            ended = false;
            try
            {
                serialPort.WriteLF("s" + "," + userid + "," + DateTime.Now.ToString("yyyy-MM-dd") + "_" +
                                   DateTime.Now.ToString("HH-mm-ss") + "," + name);
            }
            catch (Exception)
            {

            }
        }

        public void SendStartSignal()
        {
            gameEnded = false;
            ended = false;
            try
            {
                serialPort.WriteLF("s" + "," + userid + "," + DateTime.Now.ToString("yyyy-MM-dd") + "_" +
                                   DateTime.Now.ToString("HH-mm-ss") + "," + Application.productName);
            }
            catch (Exception)
            {

            }
        }

        public void HardGameEnded()
        {
            gameEnded = true;

            if (!ended && IsSerialPortValid())
            {
                try
                {
                    _ = serialPort.WriteLF("e" + "," + userid + "," + DateTime.Now + ":" + DateTime.Now.Millisecond +
                                           "," + Application.productName);
                    ended = true;
                }
                catch (Exception)
                {

                }

                return;
            }

            return;
        }

        private void UpdateParameters()
        {
            forceLevel = DDAManager.Instance.DifficultyValues["ForceFeedback"];
        }
    }
}