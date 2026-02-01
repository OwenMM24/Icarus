using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using DG.Tweening;
namespace Enablegames
{

/// <summary>
/// UI Element. Default for generic/quick setup.
/// </summary>
public class SessionCreatorPanel_Modular : MonoBehaviour {

    public delegate void SessionEvent( );
    public static event SessionEvent OnSessionLoaded;
    public string category;

    [SerializeField]
    private InputField textField;
	[SerializeField]
	private GameObject dropDownText;
	public FilesToDropdown ftd; 
	private static float TWEEN_TIME = .2f;
	private GamePanel gamePanel;

	void Start () {
        print("SessionCreatorPanel:Start");
		gamePanel = this.gameObject.GetComponent<GamePanel> ();
	}

	/*
	void Awake () {
		this.transform.localScale = Vector3.zero;
		this.transform.DOScale(1f,TWEEN_TIME);
	}

    public void Dismiss() {
		this.transform.DOScale(0f,TWEEN_TIME);
		StartCoroutine (Disable ());
    }

	IEnumerator Disable () {
		yield return new WaitForSeconds(TWEEN_TIME);
		UnityEngine.Object.Destroy(this.gameObject);
	}
	*/

    /// <summary>
    /// Save currently set parameters into settings folder
    /// </summary>
    public void SaveParameters() {
        DirectoryInfo info = new DirectoryInfo(GameParameters.SettingsFolder);
        if (info.Exists == false) {
            info.Create();
        }
        string playerID = textField.text;
        GameParameterCreator_alt[] gameCreators = GetGameParameterCreators();
        foreach (GameParameterCreator_alt gpc in gameCreators) {
            if (null == gpc.Parameters) {
                gpc.SetUp();
            }
            var gameParams = gpc.Parameters.Categories[category];
            string filename = Path.Combine(GameParameters.SettingsFolder, playerID + "." + gpc.ParameterKey + "." + category + GameParameters.FileExtension);
            string serializedParameters = JSONSerializer.Serialize(typeof(List<GameParameter>),gameParams);
            using (StreamWriter sw = new StreamWriter(filename)) {
                sw.Write(serializedParameters);
            }
        }
        SessionCreator.Instance.SessionName = playerID;
		ftd.GetFileNames ();  //populate the dropdown list
		gamePanel.Hide();
    }

    /// <summary>
    /// Load saved parameters from SettingsFolder
    /// </summary>
    public void LoadParameters() {
        //string playerID = textField.text;
		string playerID = dropDownText.GetComponent<Text>().text;
        GameParameterCreator_alt[] gameCreators = GetGameParameterCreators();
        ParameterHandler.Instance.ResetParameters();
        bool success = true;
        foreach (GameParameterCreator_alt gpc in gameCreators) {
            gpc.Reset();
            string filename = Path.Combine(GameParameters.SettingsFolder, playerID + "." + gpc.ParameterKey + "." + category + GameParameters.FileExtension);
            try {
                string serializedParameters = string.Empty;
                using (StreamReader sr = new StreamReader(filename)) {
                    serializedParameters = sr.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(serializedParameters)) {
                    List<GameParameter> gp = (List<GameParameter>)JSONSerializer.Deserialize(typeof(List<GameParameter>),serializedParameters);
                    gpc.Parameters.Categories[category] = gp;
                    //gpc.LoadIntoParamHandler(gp);
                }
            } catch (Exception e) {
                Debug.Log(e.Message);
                success = false;
            }
        }
        if(success)
        {
            TextFadeInOut.MakeTextFadeInOut("Profile loaded.");
            
            if( OnSessionLoaded  != null )
            {
                OnSessionLoaded( );
            }
        }
        else
        {
            TextFadeInOut.MakeTextFadeInOut("Error, profile not loaded.");
        }
        SessionCreator.Instance.SessionName = playerID;
		gamePanel.Hide ();
    }

    private GameParameterCreator_alt[] GetGameParameterCreators() {
        return GameCreatorPanel.Instance.GameParameterCreators;
    }

}
}