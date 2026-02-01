using UnityEngine;
using System.Collections;
using Enablegames;
public class playerLaneTracker : MonoBehaviour {

	private Transform _playerTransform;

	private string whichLaneAmIn;

	// Use this for initialization
	void Start () {
		whichLaneAmIn = "None";
		_playerTransform = GetComponent<Transform> ();

		Tracker.Instance.AddTickModule (new TrackerModule ("Player Lane", playerLaneCheck));
		Tracker.Instance.BeginTracking ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_playerTransform.position.x > 0.6f) {
			whichLaneAmIn = "left";
		} else if (_playerTransform.position.x < -0.6f) {
			whichLaneAmIn = "right";
		} else if ((_playerTransform.position.x <= 0.6f) && (_playerTransform.position.x >= -0.6f)){
			whichLaneAmIn = "middle";
		}
	}

	EnableString playerLaneCheck(){
		return new EnableString(whichLaneAmIn);
	}
}
