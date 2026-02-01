using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using eaglib;
//EG REQUIRED
//using enableGame;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using Enablegames;
using Enablegames.Suki;
/// <summary>
/// Demo Game.  Moves cube lef and right using 2 suki profiles.
/// Demonstrates network connection to EAG launcher, using SUKI, binding parameters to variables.
/// Recording data is in Tracker scene object (not here) which sets up tracker, header, and footer.
/// 
/// </summary>
public class egServer : MonoBehaviour {

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

	public string networkAddress = "localhost";
	void Awake () {
		PlayerPrefs.SetString(egParameterStrings.LAUNCHER_ADDRESS,networkAddress);
		egAwake ();

	}
	
	// Update is called once per frame
	void Update () {
		egUpdate ();
	}

	bool gameStarted = false;
	bool gameOver = false;
	public float duration = 0;

	/// <summary>
	/// Main game loop. Checks SUKI Input, updates game time, etc.
	/// </summary>
    private void FixedUpdate()
    {
		if (gameOver)
			return;
		if (!gameStarted) {  //is level just starting?
			gameStarted = true;
			startTime = Time.time;
		}

		duration = Time.time - startTime;			

    }

    /// <summary>
    /// Pauses game time and audio
    /// </summary>
	public void PauseGame() {
		//print("Game is Paused...");
		isPaused = true;
		Time.timeScale = 0;
		Time.fixedDeltaTime = 0;
		AudioListener.volume = 0;
	}

    /// <summary>
    /// Unpauses game time and audio
    /// </summary>
	public void UnPauseGame (){
		//print("Unpause");
		isPaused = false;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		AudioListener.volume = 1.0f;
	}

    /// <summary>
    /// Quit the current gamesession (tracking).
    /// </summary>
	public void EndGame() {
		PauseGame ();
		gameOver = true;
        UnPauseGame();  //must start up unity time again so DOTweens work

	}

    /// <summary>
    /// End the session and Load the main menu scene.
    /// </summary>
	public void MainMenu()
	{
		print ("MainMenu");
        UnPauseGame();  //must start up unity time again so DOTweens work


		#if UNITY_IOS
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
	public void ReloadGame ()
	{
		print ("ReloadGame");
        UnPauseGame();  //must start up unity time again so DOTweens work


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
	public void NextGame ()
	{
		print ("NextGame");
        UnPauseGame();  //must start up unity time again so DOTweens work

		#if UNITY_IOS
		LoaderUtility.Deinitialize();
		LoaderUtility.Initialize();
		#endif
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
	}

	///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////
	/// BEGIN ENABLEGAMES REQUIRED CODE
	/// </summary>
	/// 
	/// 
	///
	/// //holds the body data for the avatar

	public RoboticData roboticData;
	private Enablegames.Suki.SukiInput suki = null; //maps avatar body data to game input

	// Use this for initialization
	void egAwake () {
		print ("egAwake");
		print ("egAwake:Trying to connect...");

		// connect the client skeleton to the server skeleton (running in the enablegames launcher app)
		string address = PlayerPrefs.GetString(egParameterStrings.LAUNCHER_ADDRESS);
		print ("Address= " + address);
		//NetworkClientConnect.Instance.Connect (address);
		print ("egAwake:after connect.");


	}

	void Start()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

	}
	
	// Update is called once per frame
	void egUpdate () {
		// Return to main menu
		
		if (Input.GetKeyDown(KeyCode.A))
		{
			roboticData.data["R1"].Value -= 1f;
			Debug.Log("Left key: " + suki.Skeleton.roboticData.data["R1"].Value);
		}


		if (Input.GetKeyDown(KeyCode.S))
		{
			roboticData.data["R1"].Value += 1f;
			Debug.Log("Right key: " + suki.Skeleton.roboticData.data["R1"].Value);
		}


		if (Input.GetKeyDown(KeyCode.Escape))
		{
			EndGame ();
		}
	}


}
