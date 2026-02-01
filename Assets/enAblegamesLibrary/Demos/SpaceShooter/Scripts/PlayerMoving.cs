using UnityEngine;
using Enablegames;
using UnityEngine.Networking;
using Enablegames.Suki;

/// <summary>
/// This script defines the borders of ‘Player’s’ movement. Depending on the chosen handling type, it moves the ‘Player’ together with the pointer.
/// </summary>

namespace SpaceShooterDemo
{
    [System.Serializable]
    public class Borders
    {
        [Tooltip("offset from viewport borders for player's movement")]
        public float minXOffset = 1.5f, maxXOffset = 1.5f, minYOffset = 1.5f, maxYOffset = 1.5f;
        [HideInInspector] public float minX, maxX, minY, maxY;
    }
}

namespace SpaceShooterDemo
{
    public class PlayerMoving : MonoBehaviour
    {

        [Tooltip("offset from viewport borders for player's movement")]
        public Borders borders;
        Camera mainCamera;

        public static PlayerMoving instance; //unique instance of the script for easy access to the script

        public SkeletonData Skeleton;
        private RoboticData roboticData;
        public egGameManager gameManager = null;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            Application.targetFrameRate = 200;

            PlayerPrefs.SetString(egParameterStrings.LAUNCHER_ADDRESS, PlayerPrefs.HasKey("EgServerIPInputField") ? PlayerPrefs.GetString("EgServerIPInputField") : "localhost");
            print("egAwake");
            // initialize SUKI
        }

        private void Start()
        {
            gameManager = egGameManager.Instance;
            mainCamera = Camera.main;
            ResizeBorders();                //setting 'Player's' moving borders deending on Viewport's size
        }

        private void Update()
        {
            Vector3 pos = transform.position;

            if (SukiInput.Instance.RangeExists("placement"))
            {
                pos.x = ((egGameManager.Instance.GetSukiInput().x * 2) - 1f) * borders.maxX;
            }

            if (Mathf.Abs(pos.x - transform.position.x) >= 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 50f);
            }
            else
            {
                transform.position = pos;
            }
        }

        //setting 'Player's' movement borders according to Viewport size and defined offset
        void ResizeBorders()
        {
            borders.minX = mainCamera.ViewportToWorldPoint(Vector2.zero).x + borders.minXOffset;
            borders.minY = mainCamera.ViewportToWorldPoint(Vector2.zero).y + borders.minYOffset;
            borders.maxX = mainCamera.ViewportToWorldPoint(Vector2.right).x - borders.maxXOffset;
            borders.maxY = mainCamera.ViewportToWorldPoint(Vector2.up).y - borders.maxYOffset;
        }
    }
}