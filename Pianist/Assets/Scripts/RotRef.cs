using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotRef : MonoBehaviour
{

    float rot;
    Vector3 initRot, initRotPlayer;
    [HideInInspector]
    public static int side;
    public Transform player;
    public Transform pont;
    public static bool isRot;
    public Player pm;
    public Material sky;

    void Start()
    {
        isRot = false;
        rot = transform.rotation.eulerAngles.y;
        initRot = transform.rotation.eulerAngles;
        initRotPlayer = pm.transform.rotation.eulerAngles;

        side = 0;
    }

    float angle;

    float time;
    public float skyRotSpeed = 1f;
    void Update()
    {
        sky.SetFloat("_Rotation", time * skyRotSpeed);
        time += Time.deltaTime;
        if (time >= 360) time = 0f;



        if (Mathf.Abs(side) == 4) side = 0;



        if (Input.GetKeyDown(KeyCode.A))
        {
            rot += 90; side--;
            if (side == -2) side = 2;
            if (side == -3) side = 1;
            if (side == -1) side = 3;
            Player.isJumping = false;

            initRotPlayer.y += 90;
            StartCoroutine(PlayerRotateWithDelay());

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rot -= 90; side++;

            Player.isJumping = false;
            initRotPlayer.y -= 90;
            StartCoroutine(PlayerRotateWithDelay());


        }

        initRot.y = rot;


        transform.rotation = Quaternion.Euler(initRot);

        pont.transform.rotation = transform.rotation;


    }

    IEnumerator PlayerRotateWithDelay()
    {
        yield return new WaitForSeconds(0.15f);
        pm.transform.rotation = Quaternion.Euler(initRotPlayer);
    }

    void playerReset()
    {

    }

}
