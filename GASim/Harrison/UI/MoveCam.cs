using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * User "Free-lock" camera mover movement
 * Toggle lock and free by pressing "L"
 * Move with WASD or arrow keys when unlocked
 * Zoom in with left shift
 * Zoom out with spacebar
 */ 
public class MoveCam : MonoBehaviour {

    public bool lockon = false;
    float scale = 0.25f;

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.L))
        {
            lockon = !lockon;
        }
        if (!lockon)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            float z = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                z = 1*scale;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                z = -1*scale;
            }
            transform.Translate(new Vector3(h, v, z));
        }
    }
}
