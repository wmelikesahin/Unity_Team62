using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontroller : MonoBehaviour
{ public float rotatemin = -90; 
    public float rotatemax = 90; 
    void Start()
        {


        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                rotateTween(90);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                rotateTween(-90);
            }
        }

        private void rotateTween(float amount)
        {
            transform.Rotate(new Vector3(0,amount,0));

        }


    }
