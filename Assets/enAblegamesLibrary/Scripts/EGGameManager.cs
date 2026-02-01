using UnityEngine;
using UnityEngine.SceneManagement;
using Enablegames;
using Enablegames.Suki;
#if UNITY_IOS
//&& !UNITY_EDITOR
using UnityEngine.XR.ARFoundation;
#endif
#if UNITY_IOS
using ARFoundationRemote.Runtime;
#endif



/// <summary>
/// Demonstrates network connection to EAG launcher, using SUKI, binding parameters to variables.
/// Recording data is in Tracker scene object (not here) which sets up tracker, header, and footer.
/// 
/// </summary>
public class egGameManager : MonoBehaviour
{
	protected static egGameManager _instance = null;

	// player note from the game over panel
	// in Citadel we store the game information such as score and stats in different placese, we may want to have a script 
	// or a singleton that store all those information (so the tracker footer can retrieve those information directly by it)
	public string Notes;   //Player notes to be recorded in footer for portal
	public int score = 0;  // only non-zero scores show up on portal

	public string MenuScene = "eag_MainMenu";
	public string GameScene = "eag_KickCubeGame";

	public Transform PlayerObject;  //the player object
	float startTime; //start time of game

	public static bool isPaused;

	/// <summary>
	/// This floats are set with the egGetSukiInput() function.  It is used for a basic single input from 0 to 1.
	/// </summary>
	float sukiValue1 = 0;
	/// <summary>
	/// This floats are set with the egGetSukiInput() function.  It is used for a basic secondary input from 0 to 1.
	/// </summary>
	float sukiValue2 = 0;

	public bool printIfSukiIsUpdating = false;

	public static egGameManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<egGameManager>();
				if (_instance == null)
				{
					// still no GameManager present, raise awareness:
					Debug.LogError("An instance of type GameManager is needed in the scene, but there is none!");
				}
			}
			return _instance;
		}
	}


	public string networkAddress = "localhost";
	void Awake()
	{
#if UNITY_IOS
            var activeLoader = LoaderUtility.GetActiveLoader();
#endif
		PlayerPrefs.SetString(egParameterStrings.LAUNCHER_ADDRESS, networkAddress);
		egAwake();
		//Physics.gravity = new Vector3(0.0f, Gravity, 0.0f);
	}


	void Start()
	{
		egStart();
	}


	// Update is called once per frame
	void Update()
	{
		egUpdate();
	}


	bool gameStarted = false;
	public float duration = 0;

	private void FixedUpdate()
	{
		egFixedUpdate();
	}


	/// <summary>
	/// Main game loop. Checks SUKI Input, updates game time, etc.
	/// </summary>
	private void egFixedUpdate()
	{
		if (!egInitialized())
			return;
		if (!gameStarted)
		{  //is level just starting?
			gameStarted = true;
			startTime = Time.time;
			egBeginSession();
		}
		GameLength = 300000;
		duration = Time.time - startTime;

		//Get translated game input from SUKI
		print("suki input values are " + GetSukiInput());
	}


	/// <summary>
	/// Pauses game time and audio
	/// </summary>
	public void PauseGame()
	{
		//print("Game is Paused...");
		isPaused = true;
		Time.timeScale = 0;
		Time.fixedDeltaTime = 0;
		AudioListener.volume = 0;
	}


	/// <summary>
	/// Unpauses game time and audio
	/// </summary>
	public void UnPauseGame()
	{
		//print("Unpause");
		isPaused = false;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		AudioListener.volume = 1.0f;
	}


	/// <summary>
	/// End the session and Load the main menu scene.
	/// </summary>
	public void MainMenu()
	{
		print("MainMenu");
		UnPauseGame();  //must start up unity time again so DOTweens work
		egEndSession();

#if UNITY_IOS && !UNITY_EDITOR
		LoaderUtility.Deinitialize();
		LoaderUtility.Initialize();
#endif
#if UNITY_ANDROID
		SceneManager.LoadScene(MenuScene);
#else
		SceneManager.LoadScene(MenuScene);
#endif
	}


	/// <summary>
	/// End current session and Reload game
	/// </summary>
	public void ReloadGame()
	{
		print("ReloadGame");
		UnPauseGame();  //must start up unity time again so DOTweens work
		egEndSession();

#if UNITY_IOS
		LoaderUtility.Deinitialize();
		LoaderUtility.Initialize();
#endif
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		//		LoaderUtility.Deinitialize();
	}


	/// <summary>
	/// Loads next game level.
	/// </summary>
	public void NextGame()
	{
		print("NextGame");
		UnPauseGame();  //must start up unity time again so DOTweens work
		egEndSession();

#if UNITY_IOS
		LoaderUtility.Deinitialize();
		LoaderUtility.Initialize();
#endif
		SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
	}



	public ParameterHandler ph;

	public Enablegames.SkeletonData Skeleton;       //holds the body data for the avatar
	public NetworkSkeletonOSC netskeleton; //connects avatar to EAG launcher

	public RoboticData roboticData;
	private Enablegames.Suki.SukiInput suki = null; //maps avatar body data to game input

	//egFloat,etc. are custom variables that can be attached to parameters in the settings menu and portal
	//They are attached to the parameters in the egAwake function below.
	egFloat Speed = 1.0f;       //speed of player
	egFloat Gravity = -1.0f;    //falling cylinder's gravity (-1.0 is unity default)
	egInt GameLength = 300000;  //in seconds


	// Use this for initialization
	void egStart()
	{
		suki = SukiInput.Instance;
		suki.Skeleton = Skeleton;
		Debug.Log("suki.skeleton = " + suki.Skeleton);
		// Bind Speed to the variable "STARTING SPEED" from the settings menu
		//NOTE:Binding will be skipped if ParameterHandler not loaded (i.e. running this scene 
		//without first running MainMenu scene)
		//Also, parameters must be added to DefaultParameters.json file (located in StreamingAssets folder).

		VariableHandler.Instance.Register(ParameterStrings.STARTING_SPEED, Speed);
		VariableHandler.Instance.Register(ParameterStrings.GRAVITY, Gravity);
		VariableHandler.Instance.Register(egParameterStrings.GAME_LENGTH, GameLength);
		print("Speed=" + Speed);
		print("Gravity=" + Gravity);
		print("GameLength=" + GameLength);
	}


	bool egStarted = false;

	bool egInitialized()
	{
		if (egStarted) return true;
		print("egInitialized: " + egStarted);
		if (ph == null) return false;
		GameParameters gp = (GameParameters)ParameterHandler.Instance.AllParameters[0];
		if (gp == null || gp.initialized == false) return false;
		egStart();
		if (suki == null) return false;
		if (suki.Skeleton == null) return false;
		if (netskeleton == null) return false;
		egStarted = true;
		return true;
	}


	// Use this for initialization 
	void egAwake()
	{
		ph = ParameterHandler.Instance;
		string playerID = PlayerPrefs.GetString(_LAST_USED_USER_NAME);
		GameParameters gp = (GameParameters)ParameterHandler.Instance.AllParameters[0];
		print("egBeginSession:playerID = " + playerID);
		SessionCreator.Instance.SessionName = playerID;
		SessionCreator.Instance.CreateSession();

		print("egAwake");
		// initialize SUKI
		if (Skeleton == null) Skeleton = GameObject.Find("Tracking Avatar").GetComponentInChildren<SkeletonData>();
		if (netskeleton == null)
			netskeleton = GameObject.Find("Tracking Avatar").GetComponentInChildren<NetworkSkeletonOSC>();
		print("egAwake:Trying to connect...");

		// connect the client skeleton to the server skeleton (running in the enablegames launcher app)
		string address = PlayerPrefs.GetString(egParameterStrings.LAUNCHER_ADDRESS);
		print("Address= " + address);
		print("egAwake:after connect.");
	}



	// Update is called once per frame
	void egUpdate()
	{
		if (!egInitialized())
			return;
	}
	private const string _LAST_USED_USER_NAME = "lastUsedUserName";


	void egBeginSession()
	{
		print("egBeginSession:Tracker");
		Tracker.Instance.BeginTracking();
	}


	void egEndSession()
	{
		Tracker.Instance.Interrupt((int)egEvent.Type.CustomEvent, "GameEnd");
		Tracker.Instance.StopTracking(); //writes footer
	}


	float timeSinceLastLaneMove = 0f;
	/// <summary>
	/// Main game loop. Checks SUKI Input, updates game time, etc.
	/// </summary>
	public Vector2 GetSukiInput()
	{
		if (!suki)
			return new Vector2 (0,0);
		//print ("egGetSukiInput-------------------");
		timeSinceLastLaneMove += Time.deltaTime;

		//Get translated game input from SUKI
		// no-op if SUKI is not currently giving us input data

		if(printIfSukiIsUpdating)
        {
			if (!suki.Updating)
			{
				print("Game:suki not updating.");
				return new Vector2(0, 0);
			}
			print("Game:suki updating.");
		}

		///
		/// Read the various Suki inputs (depending on what suki file was loaded)
		/// Below contains examples for different types of input, including joint angles, bone positions, etc.
		/// 
		// read the placement range input and move the cube
		//elbow angle suki schema profile is set as "placement", but probably should use better name.
		//In X-Z movement, can be used for Z-movement together with "joystick" for X

		if (suki.RangeExists("placement"))
		{
			// we can use a range value as a placement to move left and right
			sukiValue1 = suki.GetRange("placement");
			print("placement min = " + suki.GetExtentMin("placement"));
		}

		//shoulder profile is set as "joystick"
		if (suki.RangeExists("joystick"))
		{
			// we can use a range value as a placement to move left and right
			sukiValue1 = suki.GetRange("joystick");
			print("joystick min = " + suki.GetExtentMin("joystick"));
		}

		if (suki.RangeExists("placement2"))
		{
			// we can use a range value as a placement to move left and right
			sukiValue2 = suki.GetRange("placement2");
			print("placement2 min = " + suki.GetExtentMin("placement2"));
		}

		//shoulder profile is set as "joystick"
		if (suki.RangeExists("joystick2"))
		{
			// we can use a range value as a placement to move left and right
			sukiValue2 = suki.GetRange("joystick2");
			print("joystick2 min = " + suki.GetExtentMin("joystick2"));
		}

		//shoulder profile is set as "joystick"
		if (suki.RangeExists("button"))
		{
			// we can use a range value as a placement to move left and right
			sukiValue2 = suki.GetRange("button");
			print("button min = " + suki.GetExtentMin("button"));
		}

		//moving in discrete steps/lanes
		if (suki.SignalExists("moveLeft") && suki.SignalExists("moveRight"))
		{
			// we can use a pair of triggers to move left or move right
			bool moveLeft = suki.GetSignal("moveLeft");
			bool moveRight = suki.GetSignal("moveRight");
			print("Moveleft= " + moveLeft + ", Moverightt= " + moveRight);

			// only if there is a direction to move, and it's been some time since our last move
			// Instead of changing the speed of the movement here we change the pause between movements
			if ((!moveLeft && !moveRight) || (moveLeft && moveRight) || (timeSinceLastLaneMove < 1 / Speed)) // we use speed as a time scaler
			{
				return new Vector2(0, 0);
			}
			else if (moveLeft)
			{
				sukiValue1 = (sukiValue1 - 0.2f);
			}
			else if (moveRight)
			{
				sukiValue1 = (sukiValue1 + 0.2f);
			}
			timeSinceLastLaneMove = 0f;
		}

		//using foot or hand x-y position to control player position
		//You could also use each independently as Kollect does to control the hand/footprints.
		if (suki.Location2DExists("leftfoot") || suki.Location2DExists("rightfoot") || suki.Location2DExists("lefthand") || suki.Location2DExists("righthand"))
		{
			Vector2 fpos;
			if (suki.Location2DExists("leftfoot"))
				fpos = suki.GetLocation2D("leftfoot");
			else if (suki.Location2DExists("rightfoot"))
				fpos = suki.GetLocation2D("rightfoot");
			else if (suki.Location2DExists("lefthand"))
				fpos = suki.GetLocation2D("lefthand");
			else if (suki.Location2DExists("righthand"))
				fpos = suki.GetLocation2D("righthand");
			else
				fpos = new Vector2();
			print("fpos= " + fpos);

			float xPercent = (fpos.x * 2) - 1f;
			float yPercent = (fpos.y * 2) - 1f;
			float weight = 10f;
			sukiValue1 = (sukiValue1 * (weight - 1) + (xPercent * Speed * 4)) / weight; // we use speed as position scaler
			sukiValue2 = (sukiValue2 * (weight - 1) + (yPercent * Speed * 4)) / weight; // we use speed as position scaler
		}

		if (!(suki.SignalExists("handOverHead") && !suki.GetSignal("handOverHead")))
		{
			return new Vector2(sukiValue1, sukiValue2);
		}
		else
        {
			return new Vector2(0, sukiValue2);
		}
	}
}
