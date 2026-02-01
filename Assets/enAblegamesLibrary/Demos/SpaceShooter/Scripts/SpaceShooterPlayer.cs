using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Enablegames;
using Enablegames.Suki;
using EAGSerialHandler;

namespace SpaceShooterDemo
{
    [RequireComponent(typeof(AudioSource))]
    public class SpaceShooterPlayer : MonoBehaviour
    {
        //for game end:
        public egUIManager uiManager;
        private bool gameOver = false;

        public bool bombActive = false;

        public static SpaceShooterPlayer instance;
        private PlayerShooting playerShooting;

        public GameObject destructionFX;
        public Text scoreText;
        public int totalLives = 3;
        public float immortalPeriod = 2f;

        public Transform levelTextMarginPos, levelTextCenerPos;
        public bool levelTextInCenter;

        public CameraShake camShake;

        [SerializeField]
        private AudioClip takeDamageSound, nextLevelSound;
        [SerializeField]
        private Text timerText;

        private SpriteRenderer spriteRenderer;
        private egFloat duration = 200;
        private bool immortal;
        private float timeSinceGameStarted;
        private int score;

        public Text levelText;
        public float levelNum;

        public AudioSource audioSource;

        public int DDAdamageToPlayer = 0;



        private void Awake()
        {
            playerShooting = GetComponent<PlayerShooting>();

            if (instance == null)
                instance = this;
        }



        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            StartCoroutine(WaitForVar());

            Tracker.Instance.BeginTracking();

            Tracker.Instance.Message("Game Begin, level: " + levelNum);
            //Tracker.Instance.Message("Game Begin, userid: " + SerialHandler.userid);

            audioSource = GetComponent<AudioSource>();
        }


        IEnumerator WaitForVar()
        {
            if (ParameterHandler.Instance.AllParameters[0] != null || ParameterHandler.Instance.AllParameters[0].initialized == true)
            {
                VariableHandler.Instance.Register(egParameterStrings.GAME_LENGTH, duration);
                print("variables set up: duration");
                yield return new WaitForSeconds(0);
            }
            else
            {
                yield return new WaitForSeconds(1);
                StartCoroutine(WaitForVar());
            }
        }


        public void Damage(int damage)
        {
            if (immortal)
            {
                return;
            }

            if (DDAdamageToPlayer < 18)
                DDAdamageToPlayer += 3;

            DDAManager.Instance.PerformanceData["DamageToPlayer"] = DDAdamageToPlayer;

            playerShooting.weaponPower = 1;

            camShake.ShakePosition();

            score -= 10;

            Tracker.Instance.Message("Damage Taken, Score: " + score);
            immortal = true;
            Instantiate(destructionFX, transform.position, Quaternion.identity);

            StartCoroutine(Immortal());

            PlaySoundOneShot(takeDamageSound);
        }



        public void AddScore(int amount)
        {
            score += amount;
            Tracker.Instance.Message("Enemy Destroyed, Score: " + score);
        }


        public void GetPowerUp()
        {
            DDAdamageToPlayer -= 1;

            DDAManager.Instance.PerformanceData["DamageToPlayer"] = DDAdamageToPlayer;

            DDAManager.Instance.UpdateDifficulty();

            PlayerShooting.instance.weaponPower++;
        }


        private IEnumerator Immortal()
        {
            for (int i = 0; i < 4; i++)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(immortalPeriod / 8f);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(immortalPeriod / 8f);
            }

            immortal = false;
        }



        private void Update()
        {
            timeSinceGameStarted += Time.deltaTime;

            if (score < 0)
                score = 0;

            scoreText.text = score.ToString();

            if (timeSinceGameStarted > duration && !gameOver)
            {
                //Over(true);
                gameOver = true;
                uiManager.EndGame();
            }

            timerText.text = gameOver ? string.Empty : Mathf.RoundToInt(duration - timeSinceGameStarted).ToString();

            if (levelTextInCenter)
            {
                levelText.transform.position = levelTextCenerPos.position;
            }
            else
                levelText.transform.position = Vector3.Lerp(levelText.transform.position, levelTextMarginPos.position, Time.deltaTime * 4);

            eagDebug.Log("Extent Max" + SukiInput.Instance.GetExtentMax("placement") + " Extent Min" + SukiInput.Instance.GetExtentMin("placement"));
        }



        private void Over(bool isComplete)
        {
            Destroy(gameObject);
            SerialHandler serialHandler = FindObjectOfType<SerialHandler>();
            if (serialHandler)
            {
                serialHandler.GameEnded();
            }
            if (Tracker.Instance)
            {
                Tracker.Instance.Message("Game " + (isComplete ? "Finished" : "Over"));
                Tracker.Instance.Interrupt((int)egEvent.Type.CustomEvent, "GameEnd");
                Tracker.Instance.StopTracking();
            }
        }



        public void PlaySoundOneShot(AudioClip _clip, float volumeScaler = 1f)
        {
            audioSource.PlayOneShot(_clip, volumeScaler);
        }



        public float GetTimeRemaining()
        {
            return duration - timeSinceGameStarted;
        }



        public void UpdateLevel()
        {
            levelTextInCenter = true;

            PlaySoundOneShot(nextLevelSound);
            bombActive = true;
            levelNum += 1;
            levelText.text = "level " + levelNum;
            StartCoroutine(ResetBomb());

            Tracker.Instance.Message("Level progressed: " + levelNum);

            DDAManager.Instance.PerformanceData["Level"] = Mathf.RoundToInt(levelNum);
            DDAManager.Instance.UpdateDifficulty();
        }



        public IEnumerator ResetBomb()
        {
            yield return new WaitForSeconds(1f);
            bombActive = false;
            levelTextInCenter = false;
        }
    }
}