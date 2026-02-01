/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>
/// 
/// 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using extOSC;
using TMPro;

public class QREncodeStart : MonoBehaviour {
	public QRCodeEncodeController e_qrController;
	public RawImage qrCodeImage;
	public TMP_Text infoText;
	public OSCReceiver receiver;


	public Texture2D codeTex;
	// Use this for initialization
	void Start () 
	{
		receiver = FindObjectOfType<OSCReceiver>();

		string ipAddress = "";
		string localPort = "";

		ipAddress = OSCUtilities.GetLocalHost();
		localPort = "7778";

		infoText.text = "local IP address:\n" + ipAddress;

		setCodeType(0);
		//Encode("ipAddress:localPort");
		Encode(ipAddress + ":" + localPort);
	}
	

	public void qrEncodeFinished(Texture2D tex)
	{
		if (tex != null && tex != null) {
			int width = tex.width;
			int height = tex.height;
			float aspect = width * 1.0f / height;
			qrCodeImage.GetComponent<RectTransform> ().sizeDelta = new Vector2 (170, 170.0f / aspect);
			qrCodeImage.texture = tex;
            codeTex = tex;
        } else {
		}
	}

    public void setCodeType(int typeId)
    {
        e_qrController.eCodeFormat = (QRCodeEncodeController.CodeMode)(typeId);
        Debug.Log("clicked typeid is " + e_qrController.eCodeFormat);
    }


    public void Encode(string encodeText)
	{
		if (e_qrController != null) 
		{
			string valueStr = encodeText;
			int errorlog = e_qrController.Encode(valueStr);
		}
	}

    
    public void SaveCode()
    {
        GalleryController.SaveImageToGallery(codeTex);
    }

	public void GotoNextScene(string scenename)
	{
		SceneManager.LoadScene(scenename);
	}
}
