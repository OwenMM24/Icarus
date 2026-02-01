using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    /*    int ran_obstacle;
        float delay_value, x_pos, y_pos;
        public GameManager game_manager;
        [SerializeField] GameObject[] obstacles;
        [SerializeField] GameObject fast_particles, slow_particles;


        public float helpIntensity;
        public float harmIntensity;*/


    [SerializeField] GameObject coinObject;
    [SerializeField] float spawnDelay;
    [SerializeField] float spawnTimeRange;

    [SerializeField] float spawnHightRange;
    public bool gameActive = true;

    // x val: 15
    // y values: 4 to -3


    public void StartSpawner()
    {
        StartCoroutine(SpawnCoins());
    }

    IEnumerator SpawnCoins()
    {
        while (gameActive)
        {
            float yVal = Random.Range(-3f, 4f);

            Instantiate(coinObject, new Vector3(15f, yVal, 0f), Quaternion.identity);



            yield return new WaitForSeconds(spawnDelay);
        }



    }



    /*    private void Start()
        {

            x_pos = transform.position.x;
            StartCoroutine(HazardSpawning());
            StartCoroutine(HelpSpawning());
        }*/

    /*    public void Speed_Up_On()
        {
            fast_particles.SetActive(true);
        }

        public void Speed_Up_Off()
        {
            fast_particles.SetActive(false);
        }

        public void Slow_Down_On()
        {
            slow_particles.SetActive(true);
        }
        public void Slow_Down_Off()
        {
            slow_particles.SetActive(false);
        }*/

    /*    IEnumerator HazardSpawning()
        {
            yield return new WaitForSeconds(1.5f);

            while (game_manager.play_game)
            {
                if (harmIntensity == 0)
                {
                    yield return null;
                    continue;
                }

                float yPos = Random.Range(-5f, 5f);
                int ranObstacle = Random.Range(0, 2);

                float minDelay = Mathf.Lerp(6f, 0.5f, harmIntensity / 7f);
                float maxDelay = Mathf.Lerp(8f, 1.5f, harmIntensity / 7f);

                float randomDelay = Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(randomDelay);

                Instantiate(
                    obstacles[ranObstacle],
                    new Vector2(x_pos, yPos),
                    Quaternion.identity
                );
            }
        }*/



    /*    IEnumerator HelpSpawning()
        {
            yield return new WaitForSeconds(2f);

            while (game_manager.play_game)
            {
                if (helpIntensity == 0)
                {
                    yield return null;
                    continue;
                }

                float yPos = Random.Range(-5f, 5f);
                int ranObstacle = Random.Range(2, 4);

                float minDelay = Mathf.Lerp(6f, 0.5f, helpIntensity / 7f);
                float maxDelay = Mathf.Lerp(8f, 1.5f, helpIntensity / 7f);

                float randomDelay = Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(randomDelay);

                Instantiate(
                    obstacles[ranObstacle],
                    new Vector2(x_pos, yPos),
                    Quaternion.identity
                );
            }
        }*/


}
