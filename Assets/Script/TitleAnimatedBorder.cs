using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimatedBorder : MonoBehaviour
{
    public Vector3 speed = new Vector3(0.12f, 0f, 0f);
    public Vector3 pos = new Vector3(0f, 0f, 0f);

    public bool a = false;
    public bool b = false;
    public bool c = false;
    public bool d = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = gameObject.transform.position;
    }

    void FixedUpdate() {
        gameObject.transform.position += speed;

        a = gameObject.transform.position.x > 6f;
        b = gameObject.transform.position.y < 2f;
        c = gameObject.transform.position.x < -3f;
        d = gameObject.transform.position.y > 4.5f;

        if(gameObject.transform.position.x > 6f && gameObject.transform.position.y > 3.5f) {
            speed = new Vector3(0f, -0.12f, 0f);
        }

        if(gameObject.transform.position.y < 2f && gameObject.transform.position.x > 0f) {
            speed = new Vector3(-0.12f, 0f, 0f);
        }

        if(gameObject.transform.position.x < -3f && gameObject.transform.position.y < 3.5f) {
            speed = new Vector3(0f, 0.12f, 0f);
        }

        if(gameObject.transform.position.y > 4.5f && gameObject.transform.position.x < 0f) {
            speed = new Vector3(0.12f, 0f, 0f);
        }
    }


}
