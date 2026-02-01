using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using TMPro;
using EAGSerialHandler;

using Enablegames;
using Enablegames.Suki;

#if UNITY_EDITOR
using UnityEditor;
#endif

//THIS SCRIPT CONTROLS ALL FUNCTIONS OF EGCANVAS
//IT IS MODIFIED IN THE INSPECTOR BY "egCanvasEditor.cs"
//WHEN A NEW VARIABLE IS CREATED, MAKE SURE TO ADD IT TO "egCanvasEditor.cs"

public struct EGPauseAudioSources
{
    public AudioSource audioSource;
    public float myVolume;

    public EGPauseAudioSources(AudioSource audioSource, float myVolume)
    {
        this.audioSource = audioSource;
        this.myVolume = myVolume;
    }
}



[ExecuteInEditMode]
public class egUIManager : MonoBehaviour
{
    public enum IconPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    };

    [HideInInspector] public RectTransform pos_topLeft;
    [HideInInspector] public RectTransform pos_topRight;
    [HideInInspector] public RectTransform pos_bottomLeft;
    [HideInInspector] public RectTransform pos_bottomRight;

    [HideInInspector] public bool usePauseButton = true;
    [HideInInspector] public IconPosition pauseButtonPosition = IconPosition.TopLeft;

    public bool bodyTrackerView
    {
        get
        {
            return _bodyTrackerView;
        }
        set
        {
            _bodyTrackerView = value;
            UpdateBodyView();
        }
    }
    private bool _bodyTrackerView = true;


    [HideInInspector] public IconPosition cameraViewPosition = IconPosition.BottomRight;

    [HideInInspector] public Vector2 pauseBtn_customPosition;
    [HideInInspector] public Vector2 camView_customPosition;

    //UI windows to control.
    [HideInInspector] public GameObject sessionClear;
    [HideInInspector] public GameObject zeroConButton;
    [HideInInspector] public egGameResults gameResults;
    [HideInInspector] public egPlayerReview playerReview;
    [HideInInspector] public Image bgImage;

    //Float to test if the UI windows are on or off.
    [HideInInspector] public bool gameResultsOn;
    [HideInInspector] public bool playerReviewOn;
    [HideInInspector] public bool sessionClearOn;
    [HideInInspector] public bool darkenBackground;

    private Vector2 lastScreenSize;

    private egSessionClear myegSessionClear;

    //Controls music for results screen.
    AudioSource soundPlayer;

    #region egPauseManager VARIABLES:

    [HideInInspector] public GameObject pausePanel;
    [HideInInspector] public static bool isPaused;        //is game already paused?

    //AudioSources that are found and silenced when the game is paused.
    private List<EGPauseAudioSources> activeAudioSources = new List<EGPauseAudioSources>();

    private SerialHandler serialHandler;

    public enum Page { PLAY, PAUSE }
    private Page currentPage = Page.PLAY;
    #endregion

    public string MenuScene = "eag_MainMenu";

    public bool useEscKeyForPause = true;

    public AudioClip victoryJingle;
    public AudioClip endMusic;

    public Sprite exitBodyView;
    public Sprite seeBodyView;

    private GameObject pauseButtonObject;
    private GameObject bodyView_Group;
        private GameObject bodyView_Cam;
        private GameObject bodyView_Button;
        private Transform bodyView_PosTopLeft;
        private Transform bodyView_PosBottomRight;

    private GameObject fade_BodyView_Group;
        private GameObject sukiSchema_Text;
        private CanvasGroup overlayGroup;



#if UNITY_EDITOR
    // This method is called when the script is loaded or a value is changed in the Inspector
    private void OnValidate()
    {
        UpdateSessionClear();
        UpdateButtonPositions();
        bodyTrackerView = PlayerPrefs.GetInt("bodyView") != 0;

//        UpdateBodyView();
    }
#endif



    private void Start()
    {
        UpdateSessionClear();
        UpdateButtonPositions();

        bodyTrackerView = PlayerPrefs.GetInt("bodyView") != 0;

        lastScreenSize = new Vector2(Screen.width, Screen.height);
    }



    private void UpdateSessionClear()
    {
        //Find the sessionClear and pausePanel objects in the Windows group.
        if (sessionClear == null)
            sessionClear = transform.Find("Windows").transform.Find("SessionClear").gameObject;
        if (pausePanel == null)
            pausePanel = transform.Find("Windows").transform.Find("PausePanel").gameObject;
        if (zeroConButton == null)
            zeroConButton = pausePanel.transform.Find("CenterElements").transform.Find("ZeroConBtn").gameObject;

        myegSessionClear = sessionClear.GetComponent<egSessionClear>();
    }



    private void UpdateButtonPositions()
    {
        //Find the corner view positions' transforms.
        #region FindObjectsAndTranforms
        if (pos_bottomRight == null)
         pos_bottomRight = transform.Find("CornerPositions").transform.Find("Pos_BottomRight").GetComponent<RectTransform>();
        if (pos_bottomLeft == null)
         pos_bottomLeft = transform.Find("CornerPositions").transform.Find("Pos_BottomLeft").GetComponent<RectTransform>();
        if (pos_topRight == null)
         pos_topRight = transform.Find("CornerPositions").transform.Find("Pos_TopRight").GetComponent<RectTransform>();
        if (pos_topLeft == null)
         pos_topLeft = transform.Find("CornerPositions").transform.Find("Pos_TopLeft").GetComponent<RectTransform>();

        //Find the pause button and camera view objects.
        if (pauseButtonObject == null)
         pauseButtonObject = transform.Find("Windows").transform.Find("Overlay").transform.Find("FadeObjects").transform.Find("PauseButton").gameObject;
        if (bodyView_Group == null)
         bodyView_Group = transform.Find("Windows").transform.Find("Overlay").transform.Find("BodyView_Group").gameObject;
            if (bodyView_Cam == null)
             bodyView_Cam = transform.Find("Windows").transform.Find("Overlay").transform.Find("BodyView_Group").transform.Find("BodyView_Cam").gameObject;
            if (bodyView_Button == null)
             bodyView_Button = transform.Find("Windows").transform.Find("Overlay").transform.Find("FadeObjects").transform.Find("BodyView_Group_Fade").transform.Find("BodyView_Button").gameObject;
            if (bodyView_PosTopLeft == null)
             bodyView_PosTopLeft = transform.Find("Windows").transform.Find("Overlay").transform.Find("BodyView_Group").transform.Find("BodyView_PosTopLeft");
            if (bodyView_PosBottomRight == null)
             bodyView_PosBottomRight = transform.Find("Windows").transform.Find("Overlay").transform.Find("BodyView_Group").transform.Find("BodyView_PosBottomRight");
            if(fade_BodyView_Group == null)
                fade_BodyView_Group = transform.Find("Windows").transform.Find("Overlay").transform.Find("FadeObjects").transform.Find("BodyView_Group_Fade").gameObject;
            if(sukiSchema_Text == null)
            if (sukiSchema_Text == null)
                sukiSchema_Text = transform.Find("Windows").transform.Find("Overlay").transform.Find("FadeObjects").transform.Find("BodyView_Group_Fade").transform.Find("SchemaDisplay_Text").gameObject;

        if (overlayGroup == null)
            overlayGroup = transform.Find("Windows").transform.Find("Overlay").transform.Find("FadeObjects").GetComponent<CanvasGroup>();
        #endregion

        if (!isPaused)
            pauseButtonObject.SetActive(usePauseButton);
        else
            pauseButtonObject.SetActive(false);

        if (pauseButtonPosition == cameraViewPosition)
        {
            pauseButtonPosition = IconPosition.TopLeft;
            cameraViewPosition = IconPosition.BottomRight;
        }

    #region RepositionObjects
        serialHandler = (SerialHandler)FindObjectOfType(typeof(SerialHandler));
        switch (pauseButtonPosition)
        {
            case IconPosition.TopLeft:
                pauseButtonObject.GetComponent<RectTransform>().position = pos_topLeft.position;
                break;
            case IconPosition.TopRight:
                pauseButtonObject.GetComponent<RectTransform>().position = pos_topRight.position;
                break;
            case IconPosition.BottomLeft:
                pauseButtonObject.GetComponent<RectTransform>().position = pos_bottomLeft.position;
                break;
            case IconPosition.BottomRight:
                pauseButtonObject.GetComponent<RectTransform>().position = pos_bottomRight.position;
                break;
        }
        
        switch (cameraViewPosition)
        {
            case IconPosition.TopLeft:
                bodyView_Group.GetComponent<RectTransform>().position = pos_topLeft.position;
                break;
            case IconPosition.TopRight:
                bodyView_Group.GetComponent<RectTransform>().position = pos_topRight.position;
                break;
            case IconPosition.BottomLeft:
                bodyView_Group.GetComponent<RectTransform>().position = pos_bottomLeft.position;
                break;
            case IconPosition.BottomRight:
                bodyView_Group.GetComponent<RectTransform>().position = pos_bottomRight.position;
                break;
        }
        
        switch (cameraViewPosition)
        {
            case IconPosition.TopLeft:
                fade_BodyView_Group.GetComponent<RectTransform>().position = pos_topLeft.position;
                break;
            case IconPosition.TopRight:
                fade_BodyView_Group.GetComponent<RectTransform>().position = pos_topRight.position;
                break;
            case IconPosition.BottomLeft:
                fade_BodyView_Group.GetComponent<RectTransform>().position = pos_bottomLeft.position;
                break;
            case IconPosition.BottomRight:
                fade_BodyView_Group.GetComponent<RectTransform>().position = pos_bottomRight.position;
                break;
        }
    #endregion
    }



    private void Awake()
    {
        soundPlayer = GetComponent<AudioSource>();

        isPaused = false;
        pausePanel.SetActive(false);
    }


    private Vector3 lastMousePosition;
    private float checkMousePosTimer = 0;
    private bool showOverlayCanvas = true;

    // Update is called once per frame
    void Update()
    {
        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
        {
            // Update lastScreenSize
            lastScreenSize.x = Screen.width;
            lastScreenSize.y = Screen.height;

            UpdateSessionClear();
            UpdateButtonPositions();
        }


        //Controls for SessionClear window.
        if (sessionClearOn && myegSessionClear.isSessionClear == false)
        {
            sessionClear.SetActive(true);
            myegSessionClear.isSessionClear = true;
        }
        else if (!sessionClearOn && myegSessionClear.isSessionClear == true)
        {
            myegSessionClear.isSessionClear = false;
        }


        //Controls for GameResults window.
        if (gameResultsOn && gameResults.showGameResults == false)
        {
            gameResults.showGameResults = true;
        }
        else if (!gameResultsOn && gameResults.showGameResults == true)
        {
            gameResults.showGameResults = false;
        }


        //Controls for PlayerReview window.
        if (playerReviewOn && playerReview.isRunningPlayerReview == false)
        {
            playerReview.isRunningPlayerReview = true;
        }
        else if (!playerReviewOn && playerReview.isRunningPlayerReview == true)
        {
            playerReview.isRunningPlayerReview = false;
        }


        //controls for darkening bg effect.
        if (darkenBackground)
        {
            bgImage.color = Vector4.Lerp(bgImage.color, new Color(0, 0, 0, 0.75f), Time.unscaledDeltaTime * 4);
        }
        else
        {
            bgImage.color = Vector4.Lerp(bgImage.color, new Color(0, 0, 0, 0), Time.unscaledDeltaTime * 4);
        }
        
        //controls for darkening bg effect.
        if (showOverlayCanvas || isPaused)
        {
            overlayGroup.alpha = Mathf.Lerp(overlayGroup.alpha, 1, Time.unscaledDeltaTime * 8);
        }
        else
        {
            overlayGroup.alpha = Mathf.Lerp(overlayGroup.alpha, 0, Time.unscaledDeltaTime * 8);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                overlayGroup.alpha = 1;
#endif
        }

        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            checkMousePosTimer = 0;
            showOverlayCanvas = true;
        }
        else
        {
            checkMousePosTimer += Time.deltaTime;
            if(checkMousePosTimer >= 2.5f)
                showOverlayCanvas = false;
        }


#region egPauseManager UPDATE:
        //touch control
        //touchManager();

        //optional pause
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)) && useEscKeyForPause)
        {
            //PAUSE THE GAME
            switch (currentPage)
            {
                case Page.PLAY:
                    PauseGame();
                    break;
                case Page.PAUSE:
                    UnPauseGame();
                    break;
                default:
                    currentPage = Page.PLAY;
                    break;
            }
        }

        //debug restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGame();
        }
#endregion
    }
    


    public void DarkenBackground()
    {
        darkenBackground = !darkenBackground;
        if(darkenBackground)
            bgImage.color = new Color(0, 0, 0, 0.75f);
    }



    public void DarkenBackground(bool setBG)
    {
        darkenBackground = setBG;
        if (darkenBackground)
            bgImage.color = new Color(0, 0, 0, 0.75f);
    }



    public void EndGame()
    {
        StartCoroutine(PlayVictoryJingle());
        sessionClearOn = true;
        DarkenBackground(true);
        pauseButtonObject.SetActive(false);
        EndSession();
    }



    IEnumerator PlayVictoryJingle()
    {
        soundPlayer.clip = victoryJingle;
        soundPlayer.loop = false;
        soundPlayer.Play();

        print("jingle length: " + victoryJingle.length);
        yield return new WaitForSecondsRealtime(victoryJingle.length);

        soundPlayer.clip = endMusic;
        soundPlayer.loop = true;
        soundPlayer.Play();
    }



    public void ZeroController()
    {
        serialHandler.ZeroController();
    }



#region egPauseManager FUNCTIONS:
    public void EndSession()
    {
        pausePanel.SetActive(false);
        PauseTime();
    }



    public void PauseGame()
    {
        try
        {
            if (SukiSchemaList.currentSukiFile.Contains("R1") || SukiSchemaList.currentSukiFile.Contains("B1"))
                zeroConButton.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.Log("Could not check for robot due to error: " + e);
        }


        currentPage = Page.PAUSE;
        DarkenBackground(true);
        pauseButtonObject.SetActive(false);
        if (pausePanel)
            pausePanel.SetActive(true);
        PauseTime();
    }


    //This is kept separate since it is used in multiple places.
    public void PauseTime()
    {
        isPaused = true;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        FindAndMuteActiveAudioSources();
    }



    public void UnPauseGame()
    {
        currentPage = Page.PLAY;
        DarkenBackground(false);
        pauseButtonObject.SetActive(usePauseButton);

        //print("Unpause");
        isPaused = false;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        FindAndPlayPausedAudioSources();

        checkMousePosTimer = 0;
        showOverlayCanvas = true;

        if (pausePanel)
            pausePanel.SetActive(false);
    }



    public void FindAndMuteActiveAudioSources()
    {
        activeAudioSources.Clear();

        // Find all GameObjects in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                // If the AudioSource is playing, add it to the list
                EGPauseAudioSources newEGAS = new EGPauseAudioSources(audioSource, audioSource.volume);
                activeAudioSources.Add(newEGAS);
                audioSource.volume = 0;
            }
        }
    }


    public void FindAndPlayPausedAudioSources()
    {
        foreach (EGPauseAudioSources EGaudioSource in activeAudioSources)
        {
            EGaudioSource.audioSource.volume = EGaudioSource.myVolume;
        }
    }



    public void ToggleBodyView()
    {
        bodyTrackerView = !bodyTrackerView;
    }



    public void UpdateBodyView()
    {
        bodyView_Cam.SetActive(bodyTrackerView);

        sukiSchema_Text.GetComponent<SchemaNameDisplay>().UpdateText();

        if (!bodyTrackerView)
        {
            bodyView_Button.GetComponent<Image>().sprite = seeBodyView;
            switch (cameraViewPosition)
            {
                case IconPosition.TopLeft:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x, bodyView_PosTopLeft.position.y);
                    bodyView_Button.transform.localScale = new Vector3(1, 1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x
                        + bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosTopLeft.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
                    break;
                case IconPosition.TopRight:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x, bodyView_PosTopLeft.position.y);
                    bodyView_Button.transform.localScale = new Vector3(1, 1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x
                        - bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosTopLeft.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
                    break;
                case IconPosition.BottomLeft:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x, bodyView_PosBottomRight.position.y);
                    bodyView_Button.transform.localScale = new Vector3(1, 1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(0, 0.3f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x
                        + bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosBottomRight.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
                    break;
                case IconPosition.BottomRight:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x, bodyView_PosBottomRight.position.y);
                    bodyView_Button.transform.localScale = new Vector3(1, 1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(1, 0.3f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x
                        - bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosBottomRight.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
                    break;
            }
            TMP_Text sst_tmpt = sukiSchema_Text.GetComponent<TMP_Text>();
            if(sst_tmpt != null)
            {
                sst_tmpt.color = new Color32(255, 255, 255, 70);
            }
        }
        else
        {
            bodyView_Button.GetComponent<Image>().sprite = exitBodyView;
            switch (cameraViewPosition)
            {
                case IconPosition.TopLeft:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x, bodyView_PosTopLeft.position.y);
                    bodyView_Button.transform.localScale = new Vector3(-1,-1,1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(0,0.5f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x
                        + bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosTopLeft.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
                    break;
                case IconPosition.TopRight:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x, bodyView_PosTopLeft.position.y);
                    bodyView_Button.transform.localScale = new Vector3(1, -1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x
                        - bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosTopLeft.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
                    break;
                case IconPosition.BottomLeft:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x, bodyView_PosBottomRight.position.y);
                    bodyView_Button.transform.localScale = new Vector3(-1, 1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(0, 0.3f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x
                        + bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosBottomRight.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
                    break;
                case IconPosition.BottomRight:
                    bodyView_Button.GetComponent<RectTransform>().position = new Vector2(bodyView_PosBottomRight.position.x, bodyView_PosBottomRight.position.y);
                    bodyView_Button.transform.localScale = new Vector3(1, 1, 1);

                    sukiSchema_Text.GetComponent<RectTransform>().pivot = new Vector2(1, 0.3f);
                    sukiSchema_Text.GetComponent<RectTransform>().position = new Vector2(bodyView_PosTopLeft.position.x
                        - bodyView_Button.GetComponent<RectTransform>().rect.width, bodyView_PosBottomRight.position.y);
                    sukiSchema_Text.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Right;
                    break;
            }
            TMP_Text sst_tmpt = sukiSchema_Text.GetComponent<TMP_Text>();
            if (sst_tmpt != null)
            {
                sst_tmpt.color = new Color32(255, 255, 255, 125);
            }
        }
        
        PlayerPrefs.SetInt("bodyView", (bodyTrackerView ? 1 : 0));
    }



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
        //LoaderUtility.Deinitialize();		
    }



    public void MainMenu()
    {
        print("MainMenu");
        UnPauseGame();  //must start up unity time again so DOTweens work
        Tracker.Instance.StopTracking(); //writes footer
        Destroy(SessionCreator.Instance);

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



    public void egEndSession()
    {
        if (Tracker.Instance != null)
        {
            Tracker.Instance.Interrupt((int)egEvent.Type.CustomEvent, "GameEnd");
            Tracker.Instance.StopTracking(); //writes footer
        }
    }
#endregion

}
