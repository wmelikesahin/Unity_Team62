using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tımelineiki : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("2", LoadSceneMode.Single);
    }
}
