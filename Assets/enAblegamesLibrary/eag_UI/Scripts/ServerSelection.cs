using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerSelection : MonoBehaviour
{
    /// <summary>
    /// Load a scene with the given build index.
    /// </summary>
    /// <param name="n">The build index of the scene.</param>
    public void LoadScene(int n)
    {
        SceneManager.LoadScene(n);
    }
}
