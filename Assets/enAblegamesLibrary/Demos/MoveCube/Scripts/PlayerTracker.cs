using UnityEngine;
using System.Collections;
using Enablegames;
public class PlayerTracker : MonoBehaviour {

	private Transform _playerTransform;

	private string whichLaneAmIn;

	// Use this for initialization
	void Start () {
		whichLaneAmIn = "None";
		_playerTransform = GetComponent<Transform> ();

		Tracker.Instance.AddTickModule (new TrackerModule ("Player Tracker", playerLaneCheck));
		Tracker.Instance.BeginTracking ();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (_playerTransform.position.x > 0.6f) {
			whichLaneAmIn = "left";
		} else if (_playerTransform.position.x < -0.6f) {
			whichLaneAmIn = "right";
		} else if ((_playerTransform.position.x <= 0.6f) && (_playerTransform.position.x >= -0.6f)){
			whichLaneAmIn = "middle";
		}*/

	}

	EnableString playerLaneCheck(){
//		return new EnableString(whichLaneAmIn);
//		Tracker.Instance.Message(new TrackerMessage("Player Position", string.Format("XPos: {0} ; YPos: {1}", this.transform.position.x.ToString(),  this.transform.position.y.ToString())));
		string msg = string.Format("XPos: {0} ; YPos: {1}", this.transform.position.x.ToString(),  this.transform.position.y.ToString());
		return new EnableString(msg);

	}
}
