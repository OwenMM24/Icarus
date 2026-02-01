using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Enablegames;
using Enablegames.Suki;
using System;
using System.IO;
using System.Linq;
using TinyJSON;

public class DDAModule : MonoBehaviour
{
	public egBool usingDDA = true;
	[SerializeField] private int suggestLevel = 0;
	[Range(0, 1)]
	public float adjustmentParameterEasier = 0.4f;
	[Range(0, 1)]
	public float adjustmentParameterHarder = 0.4f;
	public egInt manualLevel = 4;
	public string purpose;

	private int numOfLevels;

	private Dictionary<int, float> _difficultyLevels = new Dictionary<int, float>();
	private List<float> values = new List<float>();
	private bool gameOver = false;
	private bool gameStarted = false;
	private float startTime;
	private int hr;
	private float gsr;
	private int stressLevel;
	private egBool usingBio = true;
	private float baseLineGSR;
	private float baseLineHR;
	private egString playerID = "DefaultID";

	public void Awake()
	{
		//VariableHandler.Instance.Register(ParameterStrings.USING_DDA,usingDDA);
		//VariableHandler.Instance.Register(ParameterStrings.DEFAULT_DIFFICULTY,manualLevel);
		VariableHandler.Instance.Register("Using Bio", usingBio);
		print("is using DDA" + usingDDA);
		print("Default Difficulty" + manualLevel);
		egAwake();
	}

	void GetBioBaseline()
	{
		if (usingBio)
		{
			string jsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games/EnableGames", playerID, "PatientInfo");

			if (File.Exists(Path.Combine(jsonFilePath, System.DateTime.Today.ToString("yy-MM-dd") + ".json")))
			{
				string jsonString;

				PatientProfile profile;
				StreamReader jsonReader = new StreamReader(Path.Combine(jsonFilePath,
					System.DateTime.Today.ToString("yy-MM-dd") + ".json"));
				jsonString = jsonReader.ReadToEnd();
				JSON.MakeInto(JSON.Load(jsonString), out profile);
				baseLineHR = profile.BaselineHR;
				baseLineGSR = profile.BaselineGSR;
				if (baseLineGSR == 0)
				{
					usingBio = false;
				}
			}
			else
			{
				usingBio = false;
			}
		}
	}

	public void Start()
	{
		GetBioBaseline();
		egStart();
	}

	private void FixedUpdate()
	{
		egGetSukiInput();
	}

	public void Initialize(int nol)
	{
		numOfLevels = nol;
		for (int i = 0; i < numOfLevels; i++)
		{
			_difficultyLevels.Add(i, 1f);
		}
		GetBioBaseline();

		UpdateSuggestLevel();
		//suggestLevel = manualLevel;
	}

	public int SuggestLevel()
	{
		if (usingDDA)
		{
			return suggestLevel;
		}

		return (int)Math.Floor(numOfLevels / 10f * manualLevel);
	}

	void UpdateStressLevel()
	{
		float zone = baseLineGSR / 3;
		if (gsr <= baseLineGSR - 2 * zone)
		{
			stressLevel = 0;
		}
		else if (baseLineHR - 2 * zone < gsr && gsr <= baseLineGSR - zone)
		{
			stressLevel = 1;
		}
		else if (baseLineGSR - zone < gsr && gsr <= baseLineGSR + zone)
		{
			stressLevel = 2;
		}
		else if (baseLineGSR + zone < gsr && gsr <= baseLineGSR + 2 * zone)
		{
			stressLevel = 3;
		}
		else if (baseLineGSR + 2 * zone < gsr)
		{
			stressLevel = 4;
		}
	}

	//get new difficulty level suggested by POSM 
	private int UpdateSuggestLevel()
	{
		UpdateStressLevel();
		int newLevel = 0;
		float newLevelVector = 0;
		for (int i = 0; i < _difficultyLevels.Count; i++)
		{
			float a = 0;
			for (int p = i; p >= 0; p--)
			{
				a += _difficultyLevels[p];
			}

			float b = 0;
			for (int p = i; p < _difficultyLevels.Count; p++)
			{
				b += _difficultyLevels[p];
			}

			if (newLevelVector < Math.Min(a, b))
			{
				newLevel = i;
				newLevelVector = Math.Min(a, b);
			}
		}

		suggestLevel = newLevel;
		values = _difficultyLevels.Values.ToList();
		Tracker.Instance.Message("DDA: " + purpose + " Suggest Level Updated", "Suggest Level = " + suggestLevel);
		return suggestLevel;
	}

	//update belief vector base on observation
	public void UpdateBeliefVector(int observation)
	{
		UpdateStressLevel();
		print("Observation: " + observation);
		Tracker.Instance.Message("DDA: New Observation" + purpose, string.Format("Observation = {0}, Stress Level = {1}", observation, stressLevel));
		if (!usingBio)
		{
			if (observation == 0)
			{
				return;
			}
			else if (observation == -1)
			{
				for (int p = suggestLevel; p < _difficultyLevels.Count; p++)
				{
					_difficultyLevels[p] *= adjustmentParameterEasier;
				}
			}
			else if (observation == 1)
			{
				for (int p = suggestLevel; p >= 0; p--)
				{
					_difficultyLevels[p] *= adjustmentParameterHarder;
				}
			}
			else if (observation == -2)
			{
				for (int p = suggestLevel; p < _difficultyLevels.Count; p++)
				{
					_difficultyLevels[p] *= (adjustmentParameterEasier / 2f);
				}
			}
			else if (observation == 2)
			{
				for (int p = suggestLevel; p >= 0; p--)
				{
					_difficultyLevels[p] *= (adjustmentParameterHarder);
				}
			}
			else
			{
				print("The Observation must be 1, 0, or -1");
			}
			Tracker.Instance.Message(new TrackerMessage("Suggested Lvl",
				String.Format("Level Num.{0}", (suggestLevel + 1).ToString())));
		}
		else
		{
			if (stressLevel is 0)
			{
				if (observation >= -1)
				{
					for (int p = suggestLevel; p >= 0; p--)
					{
						_difficultyLevels[p] *= adjustmentParameterHarder;
					}
				}
				else
				{
					for (int p = suggestLevel; p < _difficultyLevels.Count; p++)
					{
						_difficultyLevels[p] *= adjustmentParameterEasier;
					}
				}
			}

			if (stressLevel == 3 || stressLevel == 4)
			{
				if (observation <= 1)
				{
					for (int p = suggestLevel; p < _difficultyLevels.Count; p++)
					{
						_difficultyLevels[p] *= adjustmentParameterEasier;
					}

				}
				else
				{
					for (int p = suggestLevel; p >= 0; p--)
					{
						_difficultyLevels[p] *= adjustmentParameterHarder;
					}
				}
			}
		}

		UpdateSuggestLevel();
	}

	///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////
	/// BEGIN ENABLEGAMES REQUIRED CODE
	/// </summary>
	/// 
	/// 
	/// 
	public Enablegames.SkeletonData Skeleton;       //holds the body data for the avatar

	public RoboticData roboticData;
	private Enablegames.Suki.SukiInput suki = null; //maps avatar body data to game input

	//egFloat,etc. are custom variables that can be attached to parameters in the settings menu and portal
	//They are attached to the parameters in the egAwake function below.

	// Use this for initialization
	void egStart()
	{
		suki = SukiInput.Instance;
		suki.Skeleton = Skeleton;
		Debug.Log("suki.skel = " + suki.Skeleton);
		suki.Skeleton.roboticData = roboticData;
	}
	private const string _LAST_USED_USER_NAME = "lastUsedUserName";
	void egAwake()
	{
		playerID = PlayerPrefs.GetString(_LAST_USED_USER_NAME);
		print("egBeginSession:playerID = " + playerID);
		//		Session session = SessionCreator.Instance.CurrentSession;
		SessionCreator.Instance.SessionName = playerID;
		SessionCreator.Instance.CreateSession();

		print("egAwake");
		// initialize SUKI

		if (Skeleton == null)
		{
			var skeletonObject = Resources.FindObjectsOfTypeAll<SkeletonData>()[0].gameObject;
			Skeleton = skeletonObject.GetComponent<SkeletonData>();
		}

		Debug.Log("Skeleton is Null: " + (Skeleton is null));
		//if (netskeleton == null)
		//netskeleton = GameObject.Find ("Tracking Avatar").GetComponentInChildren<NetworkSkeleton> ();
		//suki = SukiInput.Instance;
		//suki.Skeleton = Skeleton;
		//Debug.Log("suki.skel = "+ suki.Skeleton);


		/* Now initialized in SerialHandler.cs
				if (roboticData==null)
					roboticData = new RoboticData();
				if (roboticData.data.Count==0){
					RoboticDatum rd = new RoboticDatum();
					rd.Value = 0f;
					roboticData.data["R1"]=rd;
				}
		*/
		//		suki.Skeleton.roboticData = roboticData;


		print("egAwake:Trying to connect...");

		// connect the client skeleton to the server skeleton (running in the enablegames launcher app)
		string address = PlayerPrefs.GetString(egParameterStrings.LAUNCHER_ADDRESS);
		print("Address= " + address);
		print("egAwake:after connect.");
	}

	float timeSinceLastLaneMove = 0f;
	/// <summary>
	/// Main game loop. Checks SUKI Input, updates game time, etc.
	/// </summary>
	private void egGetSukiInput()
	{
		if (!suki)
			return;
		//print ("egGetSukiInput-------------------");
		timeSinceLastLaneMove += Time.deltaTime;
		/*
			float duration = Time.time - startTime;
			if (duration >= GameLength)  //is game time over?
				showGameOverPanel ();
			timeSinceLastLaneMove += Time.deltaTime;
			*/

		//Get translated game input from SUKI
		// no-op if SUKI is not currently giving us input data

		/*NO LONGER NEED NETSKELETON TO KNOW IF CONNECTED..USES MOVEMENT FROM T-POSE INSTEAD (suki.Updating)
		//print("Game:FixedUpdate:" + suki.Updating);
		if (netskeleton && netskeleton.moving)
		{
			print ("netskel moving:" + suki.Skeleton.Moving);
			suki.Skeleton.moving = true;
			suki.Skeleton.resetMinMax = true;
		}
		*/
		if (!suki.Updating)
		{
			print("Game:suki not updating.");
			return;
		}
		print("Game:suki updating.");

		//shoulder profile is set as "joystick"
		//In X-Z movement, can be used for X-movement togther with "placement" for Z.
		if (suki.RangeExists("galvanicSkinResponse"))
		{
			// we can use a range value as a placement to move left and right
			float range = suki.GetRange("galvanicSkinResponse");
			gsr = range * 10f;
			//UpdateStressLevel();
		}
		if (suki.RangeExists("heartRate"))
		{
			// we can use a range value as a placement to move left and right
			float range = suki.GetRange("heartRate");
			hr = (int)range;
			//UpdateStressLevel();
		}
	}
	///
	/// END ENABLEGAMES REQUIRED CODE
	///////////////////////////////////////////////////////////////////////////////
}
