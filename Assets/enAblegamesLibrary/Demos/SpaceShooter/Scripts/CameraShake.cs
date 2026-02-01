using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooterDemo
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float durationT, strength;//, randomness;
                                                           //[SerializeField] private bool fadeOut;
                                                           //[SerializeField] private int vibrato;
        public SpaceShooterPlayer player;

        public void ShakePosition()
        {
            Debug.Log(GetComponent<Camera>().name);

            StartCoroutine(Shake(durationT, strength));

            //GetComponent<Camera>().DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
        }
        public IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 orignalPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, -10);
                elapsed += Time.deltaTime;
                yield return 0;
            }

            yield return new WaitForEndOfFrame();
            transform.position = new Vector3(0, 0, -10);
        }
    }
}
