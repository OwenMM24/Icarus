using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public float speed = 1.0f;
    TrailRenderer trailRenderer;
    bool trail_active = false;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] positions = new Vector3[trailRenderer.positionCount];
        for (int i = 0; i < trailRenderer.positionCount; i++)
        {
            Vector3 position = trailRenderer.GetPosition(i);
            position += Vector3.left * speed * Time.deltaTime;
            positions[i] = position;
        }
        trailRenderer.SetPositions(positions);
        if (speed <= 0f)
        {
            trailRenderer.enabled = false;
            StopAllCoroutines();
        }
    }

    public void StartTrail()
    {
        speed = 0.1f;
        trail_active = true;
        trailRenderer.enabled = true;
        StartCoroutine(AdjustTrailSize());
    }

    public void EndTrail()
    {
        trail_active = false;
        
    }


    IEnumerator AdjustTrailSize()
    {
        while (trail_active)
        {
            speed += Time.deltaTime *4f;
            yield return null;
        }

        while (!trail_active)
        {
            speed -= Time.deltaTime * 2f;
            yield return null;
        }


    }
}