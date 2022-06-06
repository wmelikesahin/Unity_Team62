using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collecttonote : MonoBehaviour
{
    public int notes;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "Note")
        {
            if(Col.gameObject.tag == "Note")
            {
                Debug.Log("Note collected!");
                notes = notes + 1;
                Col.gameObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
