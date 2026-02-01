using System;
using System.Collections;
using System.Collections.Generic;
using Enablegames.Suki;
using UnityEngine;

namespace Enablegames.Demo.moveCube
{
    public class GameManager : MonoBehaviour
    {
        private bool sukiLoaded = false;
        
        //GameObject you want to control with SUKI input
        [SerializeField]
        private GameObject playerObject;
        
        //Some Parameters for the game
        //Create them as egBool or egInt etc.. They work like regular bool and int data structures except they are nullable
        //You need to initialize them before accessing them we recommend doing so when you declare them.
        private egBool tickTracking = new egBool();
        private egInt messageTimeout = new egInt();
        private egFloat speed = new egFloat();

        private void Awake()
        {
            //Sync variables with main menu with VariableHandler.Instance.Register(string key, egVar var),
            //Changes in the value of the variables will be synced across all scenes
            VariableHandler.Instance.Register("Tick Tracking", tickTracking);
            VariableHandler.Instance.Register("Message Timeout", messageTimeout);
            VariableHandler.Instance.Register("speed", speed);
        }

        // Start is called before the first frame update
        void Start()
        {
            //You can send a one time message, seen in the Coroutine
            StartCoroutine(TrackerRoutine());
            
            //Or you can register a tickModule
            if (tickTracking)
            {
                Tracker.Instance.AddTickModule(new TrackerModule("name", paramaterTracking));
            }
        }
        
        //Setup what you want to put in the tickModule by create a funtion returns a EnableString
        EnableString paramaterTracking()
        {
            string msg = string.Format("This string will be record per tick (every 1/3 seconds)");
            return new EnableString(msg);
        }

        //Showcasing how to 
        IEnumerator TrackerRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(messageTimeout);
                Tracker.Instance.Message("Example Message");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        
        private void FixedUpdate()
        {
            GetSukiInput();
        }
        
        //
        // Read the various Suki inputs (depending on what suki file was loaded)
        // Below contains examples for different types of input, including joint angles, bone positions, etc.
        // 
        // read the placement range input and move the cube
        //elbow angle suki schema profile is set as "placement", but probably should use better name.
        //In X-Z movement, can be used for Z-movement together with "joystick" for X

        private void GetSukiInput()
        {
            // Check whether the SUKI system is initialized, return if not
            if(!sukiLoaded) //&& SukiInputManager.Instance.egStarted)
                sukiLoaded = true;
            if (!sukiLoaded) return;
            
            float xpercent = 0;
            // Get a Range value from suki if a range value with the name "placement" exists
            if(SukiInputManager.Instance.GetRange("placement", out xpercent))
            {
                Vector3 pos = playerObject.transform.localPosition;
                // Remap the xpercent to a new range and set to the x axis of the position of the block
                pos.x = SukiInputManager.ReMap(xpercent, -1f, 1f);
                playerObject.transform.localPosition = pos;
            }
            
            // Or you can also check if a signal exists with the name then try to grab it from, for example "joystick" here
            if (SukiInputManager.Instance.Location2DExists("joystick"))
            {
                // Get the signal value and remap it with a dead zone at center
                var location2D = SukiInputManager.Instance.GetLocation2D("joystick");
                print("location2D = " + location2D);
                print("location2DMax = " + SukiInput.Instance.GetExtentMax2D("joystick") + "location2DMin" + SukiInput.Instance.GetExtentMin2D("joystick") );
                // Remap it to -1 ot 1 so we can easily use it as a joystick
                location2D = SukiInputManager.ReMap(location2D, -1f, 1f);
                
                
                Vector3 pos = playerObject.transform.localPosition;
                pos.y += location2D.y * speed / 40;
                pos.x += location2D.x * speed / 40;// we use speed as position scaler
                playerObject.transform.localPosition = pos;
                // Perform actions based on the signal value
            }
        }
    }
} 
