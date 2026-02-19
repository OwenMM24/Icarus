using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class PauseMovement : MonoBehaviour
{
    [SerializeField] NewPlayerMove playerMove;
    bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            PauseGame();
    }


    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            playerMove.enabled = false;
            playerMove.gameObject.transform.eulerAngles = Vector3.zero;
            playerMove.gameObject.transform.DOMove(new Vector3(playerMove.transform.position.x, 0, 0), 2);
        }
        else
        {
            isPaused = false;
            playerMove.enabled = true;

        }
    }

    public void ResumeGame()
    {
        playerMove.enabled = true;
    }

}
