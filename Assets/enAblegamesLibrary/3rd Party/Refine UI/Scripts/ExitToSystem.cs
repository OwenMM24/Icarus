using UnityEngine;

namespace RefineUI
{
    public class ExitToSystem : MonoBehaviour
    {
        public void ExitGame()
        {
            Debug.Log("Exit");
            Application.Quit();
        }
    }
}