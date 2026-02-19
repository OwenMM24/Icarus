using Enablegames;
using Enablegames.Suki;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;


public class NewPlayerMove : MonoBehaviour
{
    //[SerializeField] float moveSpeed;
    [SerializeField] bool spaceHeld;
    [SerializeField] bool allowMovement;
    [SerializeField] bool spaceGoesDown = true;
    [SerializeField] float rotAmount;
    public float lerpSpeed;
    public int coinCount;

    [SerializeField] TMP_Text coinText;


    int t;
    [Header("Roof Bounce")]
    [SerializeField] float bounceDuration = 0.4f;
    [SerializeField] float bounceSpeedMultiplier = 1.2f;

    //bool spaceHeld;
    bool forcedDown;
    public bool controlsLocked;


    SukiInput suki;

    //VariableHandler.Instance


    public egFloat moveSpeed = .5f;
    public egFloat persentHandHasToClose = .8f;








    private void Awake()
    {
        VariableHandler.Instance.Register(ParameterStrings.PLAYER_MOVE_SPEED, moveSpeed);
        VariableHandler.Instance.Register(ParameterStrings.PERCENT_HAND_HAS_TO_CLOSE, persentHandHasToClose);

        //suki = SukiInput.Instance;
        //Debug.Log("asdfghjklkjhgfd" + suki);
    }

    //int t;

    void Update()
    {

        if(suki)
            Debug.Log("Input: " + suki.GetRange("Placement"));
        if (controlsLocked)
        {
            spaceHeld = false;
            return;
        }

        spaceHeld = Input.GetKey(KeyCode.Space) || suki.GetRange("Placement") < Mathf.Abs(persentHandHasToClose - 1f);
    }

    void FixedUpdate()
    {
        if (!allowMovement) return;


        if (forcedDown)
            t = 1;
        else
            t = spaceGoesDown ? 1 : -1;


        if (spaceHeld)
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y - (0.1f * moveSpeed) * t,
                0f
            );
        else
            transform.position = new Vector3(transform.position.x,
          transform.position.y + (0.1f * moveSpeed) * t,
                0f
            );



        float targetZ;

        if (spaceHeld)
            targetZ = spaceGoesDown ? -rotAmount : rotAmount;
        else
            targetZ = spaceGoesDown ? rotAmount : -rotAmount;

        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            lerpSpeed * Time.fixedDeltaTime
        );
    }
    public void RoofBounce()
    {
        if (forcedDown) return;

        StopAllCoroutines();
        StartCoroutine(RoofBounceRoutine());
    }

    IEnumerator RoofBounceRoutine()
    {
        controlsLocked = true;
        forcedDown = true;

        float originalSpeed = moveSpeed;
        moveSpeed *= bounceSpeedMultiplier;

        yield return new WaitForSeconds(bounceDuration);

        moveSpeed = originalSpeed;
        forcedDown = false;
        controlsLocked = false;
    }


    public void AddCoin()
    {
        coinCount++;
        coinText.text = "Amount: " + coinCount.ToString();
    }


    public void StartStuff()
    {
        suki = SukiInput.Instance;
        controlsLocked = false;
        allowMovement = true;
    }

}