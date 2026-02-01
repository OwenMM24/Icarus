using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Serializable classes
[System.Serializable]
public class EnemyWaves 
{
    [Tooltip("time for wave generation from the moment the game started")]
    public float timeToStart;

    [Tooltip("Enemy wave's prefab")]
    public GameObject wave;
}

#endregion

namespace SpaceShooterDemo
{
    public class LevelController : MonoBehaviour
    {

        //Serializable classes implements
        public EnemyWaves[] enemyWaves;

        public GameObject powerUp;
        public float timeForNewPowerup;
        private bool durationInitialized;
        private float waitTime;

        Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            StartCoroutine(PowerupBonusCreation());
        }

        //Create a new wave after a delay
        IEnumerator CreateEnemyWave(float delay, GameObject Wave)
        {
            if (delay != 0)
                yield return new WaitForSeconds(delay);

            if (SpaceShooterPlayer.instance != null && SpaceShooterPlayer.instance.GetTimeRemaining() > 5f)
                Instantiate(Wave);
        }

        //endless coroutine generating 'levelUp' bonuses. 
        IEnumerator PowerupBonusCreation()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeForNewPowerup);
                Instantiate(
                    powerUp,
                    //Set the position for the new bonus: for X-axis - random position between the borders of 'Player's' movement; for Y-axis - right above the upper screen border 
                    new Vector2(
                        Random.Range(-14f, 14f),
                        mainCamera.ViewportToWorldPoint(Vector2.up).y + powerUp.GetComponent<Renderer>().bounds.size.y / 2),
                    Quaternion.identity
                    );
            }
        }



        private void LateUpdate()
        {
            if (!durationInitialized)
            {
                InitializeFullDurationEnemyWaves();
            }
        }


        private void Update()
        {
            timeForNewPowerup = (22 - DDAManager.Instance.DifficultyValues["PowerUpSpawnRate"]) / 2;
        }


        private void InitializeFullDurationEnemyWaves(int iteration = 0)
        {
            if (durationInitialized)
            {
                return;
            }

            for (int i = 0; i < enemyWaves.Length; i++)
            {
                waitTime = enemyWaves[i].timeToStart;
                if (iteration > 0)
                {
                    waitTime += 4f + (iteration * enemyWaves[enemyWaves.Length - 1].timeToStart);
                }
                if (SpaceShooterPlayer.instance.GetTimeRemaining() > waitTime + 5f)
                {
                    Debug.Log(i + " , " + waitTime);
                    StartCoroutine(CreateEnemyWave(waitTime, enemyWaves[i].wave));
                    if (i == enemyWaves.Length - 1)
                    {
                        InitializeFullDurationEnemyWaves(iteration + 1);
                    }
                }
                else
                {
                    durationInitialized = true;
                    return;
                }
            }
        }
    }
}