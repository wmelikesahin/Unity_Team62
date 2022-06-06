using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OyunbittiGOmain : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("Entry", LoadSceneMode.Single);
    }
}
