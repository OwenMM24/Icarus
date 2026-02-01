using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class PauseMovement : MonoBehaviour
{
    [SerializeField] NewPlayerMove playerMove;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PauseGame();
    }


    public void PauseGame()
    {
        playerMove.enabled = false;
        playerMove.gameObject.transform.eulerAngles = Vector3.zero;
        playerMove.gameObject.transform.DOMove(new Vector3(playerMove.transform.position.x, 0, 0), 2);
    }

    public void ResumeGame()
    {
        playerMove.enabled = true;
    }

}
