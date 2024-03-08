using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    int ran_obstacle;
    float delay_value, x_pos, y_pos;
    public GameManager game_manager;
    [SerializeField] GameObject[] obstacles;
    [SerializeField] GameObject fast_particles, slow_particles;

    private void Start()
    {

        x_pos = transform.position.x;
        StartCoroutine(HazardSpawning());
        StartCoroutine(HelpSpawning());
    }

    public void Speed_Up_On()
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
    }

    IEnumerator HazardSpawning()
    {

        yield return new WaitForSeconds(1.5f);

        while (game_manager.play_game == true)
        {
            y_pos = Random.Range(-5f, 5f);
            ran_obstacle = Random.Range(0, 2);
            delay_value = Random.Range(4f, 8f);
            yield return new WaitForSeconds(delay_value);
            Instantiate(obstacles[ran_obstacle], new Vector2(x_pos, y_pos), Quaternion.identity);

            yield return null;
        }

    }

    IEnumerator HelpSpawning()
    {

        yield return new WaitForSeconds(2f);

        while (game_manager.play_game == true)
        {
            y_pos = Random.Range(-5f, 5f);
            ran_obstacle = Random.Range(2, 4);
            delay_value = Random.Range(4f, 8f);
            yield return new WaitForSeconds(delay_value);
            Instantiate(obstacles[ran_obstacle], new Vector2(x_pos, y_pos), Quaternion.identity);

            yield return null;
        }

    }


}
