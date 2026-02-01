using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Enablegames;
using Enablegames.Suki;

namespace Enablegames
{
	public class SukiInputManager : MonoBehaviour
	{
		public static SukiInputManager Instance;

		[SerializeField][Tooltip("Automatically start session record after scene loaded.")]
		public bool autoStartSession = true;

		private float startTime; //start time of game

		private void Awake()
		{
			if (Instance is null)
				Instance = this;
			else if (Instance != this) Destroy(gameObject);
#if UNITY_IOS
            var activeLoader = LoaderUtility.GetActiveLoader();
#endif
			egAwake();
		}

		private void Start()
		{
			//egStart();
		}

		private void Update()
		{
		}

		private bool gameStarted;
		private readonly bool gameOver = false;
		public float duration;

		private void FixedUpdate()
		{
			egFixedUpdate();
		}

		public bool RangeExists(string name)
		{
			return suki.RangeExists(name);
		}

		public float GetRange(string name)
		{
			return suki.GetRange(name);
		}

		public float GetRange(string name, float threshold, float deadZone)
		{
			return ReRange(suki.GetRange(name), threshold, deadZone);
		}

		public float GetRange(string name, float threshold, float minper, float maxper)
		{
			return ReRange(suki.GetRange(name), threshold, minper, maxper);
		}

		public bool GetRange(string name, out float value)
		{
			return suki.GetRange(name, out value);
		}

		public bool GetRange(string name, float threshold, float deadZone, out float value)
		{
			if (suki.GetRange(name, out value))
			{
				value = ReRange(value, threshold, deadZone);
				return true;
			}
			value = threshold;
			return false;
		}

		public bool GetRange(string name, float threshold, float minper, float maxper, out float value)
		{
			if (suki.GetRange(name, out value))
			{
				value = ReRange(value, threshold, minper, maxper);
				return true;
			}
			value = threshold;
			return false;
		}
		
		public bool SignalExists(string name)
		{
			return suki.SignalExists(name);
		}

		public bool GetSignal(string name)
		{
			return suki.GetSignal(name);
		}

		public bool GetSignal(string name, out bool value)
		{
			return suki.GetSignal(name, out value);
		}
		
		public bool TriggerExists(string name)
		{
			return suki.TriggerExists(name);
		}

		public bool GetTrigger(string name)
		{
			return suki.GetTrigger(name);
		}
		

		public bool GetTrigger(string name, out bool value)
		{
			return suki.GetTrigger(name, out value);
		}
		
		public bool Location2DExists(string name)
		{
			return suki.Location2DExists(name);
		}

		public Vector2 GetLocation2D(string name)
		{
			return suki.GetLocation2D(name);
		}
		
		public bool GetLocation2D(string name, out Vector2 value)
		{
			return suki.GetLocation2D(name, out value);
		}

		public Vector2 GetLocation2D(string name, Vector2 threshold, float xDeadZone, float yDeadZone)
		{
			Vector2 originalRange = suki.GetLocation2D(name);
			float x = ReRange(originalRange.x, threshold.x, xDeadZone);
			float y = ReRange(originalRange.y, threshold.y, yDeadZone);
			Vector2 newRange = new Vector2(x, y);
			return newRange;
		}

		public Vector2 GetLocation2D(string name, Vector2 threshold, float deadZone)
		{
			return GetLocation2D(name, threshold, deadZone, deadZone);
		}

		public Vector2 GetLocation2D(string name, Vector2 threshold, float xMin, float yMin, float xMax, float yMax)
		{
			Vector2 originalRange = suki.GetLocation2D(name);
			float x = ReRange(originalRange.x, threshold.x, xMin, xMax);
			float y = ReRange(originalRange.y, threshold.y, yMin, yMax);
			Vector2 newRange = new Vector2(x, y);
			return newRange;
		}


		public bool GetLocation2D(string name, Vector2 threshold, float xDeadZone, float yDeadZone, out Vector2 value)
		{
			if(suki.GetLocation2D(name, out var originalRange))
			{
				float x = ReRange(originalRange.x, threshold.x, xDeadZone);
				float y = ReRange(originalRange.y, threshold.y, yDeadZone);
				Vector2 newRange = new Vector2(x, y);
				value = newRange;
				return true;
			}
			value = threshold;
			return false;
		}
		
		public bool GetLocation2D(string name, Vector2 threshold, float xMin, float yMin, float xMax, float yMax, out Vector2 value)
		{
			if(suki.GetLocation2D(name, out var originalRange))
			{
				float x = ReRange(originalRange.x, threshold.x, xMin, xMax);
				float y = ReRange(originalRange.y, threshold.y, yMin, yMax);
				Vector2 newRange = new Vector2(x, y);
				value = newRange;
				return true;
			}
			value = threshold;
			return false;
		}

		public bool Location3DExists(string name)
		{
			return suki.Location3DExists(name);
		}

		public Vector3 GetLocation3D(string name)
		{
			return suki.GetLocation3D(name);
		}

		private bool GetLocation3D(string name, out Vector3 value)
		{
			return suki.GetLocation3D(name, out value);
		}

		public Vector3 GetLocation3D(string name, Vector3 threshold, float xDeadZone, float yDeadZone, float zDeadZone)
		{
			Vector3 originalRange = suki.GetLocation2D(name);
			float x = ReRange(originalRange.x, threshold.x, xDeadZone);
			float y = ReRange(originalRange.y, threshold.y, yDeadZone);
			float z = ReRange(originalRange.z, threshold.z, zDeadZone);
			Vector3 newRange = new Vector3(x, y, z);
			return newRange;
		}

		public Vector3 GetLocation3D(string name, Vector2 threshold, float deadZone)
		{
			return GetLocation3D(name, threshold, deadZone, deadZone,deadZone);
		}

		public Vector3 GetLocation3D(string name, Vector3 threshold, float xMin, float yMin, float xMax, float yMax, float zMin, float zMax)
		{
			Vector3 originalRange = suki.GetLocation2D(name);
			float x = ReRange(originalRange.x, threshold.x, xMin, xMax);
			float y = ReRange(originalRange.y, threshold.y, yMin, yMax);
			float z = ReRange(originalRange.z, threshold.z, zMin, zMax);
			Vector3 newRange = new Vector3(x, y, z);
			return newRange;
		}


		public bool GetLocation3D(string name, Vector3 threshold, float xDeadZone, float yDeadZone, float zDeadZone, out Vector3 value)
		{
			if(suki.GetLocation3D(name, out var originalRange))
			{
				float x = ReRange(originalRange.x, threshold.x, xDeadZone);
				float y = ReRange(originalRange.y, threshold.y, yDeadZone);
				float z = ReRange(originalRange.z, threshold.z, zDeadZone);
				Vector3 newRange = new Vector3(x, y, z);
				value = newRange;
				return true;
			}
			value = threshold;
			return false;
		}
		
		public bool GetLocation3D(string name, Vector3 threshold, float xMin, float yMin, float zMin,  float xMax, float yMax, float zMax, out Vector3 value)
		{
			if(suki.GetLocation3D(name, out var originalRange))
			{
				float x = ReRange(originalRange.x, threshold.x, xMin, xMax);
				float y = ReRange(originalRange.y, threshold.y, yMin, yMax);
				float z = ReRange(originalRange.z, threshold.z, zMin, zMax);
				Vector3 newRange = new Vector3(x, y, z);
				value = newRange;
				return true;
			}
			value = threshold;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		private void egFixedUpdate()
		{
			if (!egInitialized())
				return;
			if (gameOver)
				return;
			if (autoStartSession && !gameStarted)
			{
				//is level just starting?
				gameStarted = true;
				startTime = Time.time;
				egBeginSession();
			}
		}

		public ParameterHandler ph;

		public SkeletonData skeleton; //holds the body data for the avatar
		
		private SukiInput suki; //maps avatar body data to game input


		// Use this for initialization
		private void egStart()
		{
			suki = SukiInput.Instance;
			suki.Skeleton = skeleton;
			Debug.Log("suki.skel = " + suki.Skeleton);
		}

		private bool egStarted;

		private bool egInitialized()
		{
			if (egStarted)
				return true;
			print("egInitialized: " + egStarted);
			if (ph == null)
				return false;
			var gp = ParameterHandler.Instance.AllParameters[0];
			//if (gp == null || gp.initialized == false)
			//return false;
			egStart();
			if (suki == null)
				return false;
			if (suki.Skeleton == null)
				return false;
			egStarted = true;

			return true;
		}

		private const string _LAST_USED_USER_NAME = "lastUsedUserName";

		// Use this for initialization 
		private void egAwake()
		{
			ph = ParameterHandler.Instance;
			var playerID = PlayerPrefs.GetString(_LAST_USED_USER_NAME);
			var gp = ParameterHandler.Instance.AllParameters[0];
			print("egBeginSession:playerID = " + playerID);
//		Session session = SessionCreator.Instance.CurrentSession;
			SessionCreator.Instance.SessionName = playerID;
			SessionCreator.Instance.CreateSession();

			print("egAwake");
			// initialize SUKI
			if (skeleton == null)
				skeleton = GameObject.Find("Tracking Avatar").GetComponentInChildren<SkeletonData>();
		}


		private void egBeginSession()
		{
			print("egBeginSession:Tracker");
			Tracker.Instance.BeginTracking();
		}

		private void egEndSession()
		{
			Tracker.Instance.Interrupt((int)egEvent.Type.CustomEvent, "GameEnd");
			Tracker.Instance.StopTracking(); //writes footer
		}

		public static float ReRange(float range, float threshold, float deadZoneSize)
		{
			var xPercent = range;

			if (xPercent > threshold - deadZoneSize && xPercent < threshold + deadZoneSize) xPercent = threshold;

			return xPercent;
		}

		public static float ReRange(float range, float threshold, float minper, float maxper)
		{
			if (range > minper && range < maxper) range = threshold;
			var xPercent = range;
			return xPercent;
		}

		public static float ReMap(float range, float min, float max)
		{ 
			return min < max ? range * Mathf.Abs(max - min) + min : -(range - 1) * Mathf.Abs(max - min) + max;
		}

		public static Vector2 ReMap(Vector2 value, float minX, float maxX, float minY, float maxY)
		{
			return new Vector2(ReMap(value.x, minX, maxX), ReMap(value.y, minY, maxY));
		}

		public static Vector2 ReMap(Vector2 value, float min, float max)
		{
			return new Vector2(ReMap(value.x, min, max), ReMap(value.y, min, max));
		}
		
		public static Vector3 ReMap(Vector3 value, float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
		{
			return new Vector3(ReMap(value.x, minX, maxX), ReMap(value.y, minY, maxY), ReMap(value.z, minZ, maxZ));
		}

		public static Vector3 ReMap(Vector3 value, float min, float max)
		{
			return new Vector3(ReRange(value.x, min, max), ReRange(value.y, min, max), ReRange(value.z, min, max));
		}
	}
}

