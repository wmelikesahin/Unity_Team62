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

    public void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.tag == "Note")
        
            {
                Destroy(Other.gameObject);
            }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
