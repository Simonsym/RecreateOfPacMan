using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{

    Vector2 center;
    int R = 20;
    float CENTER_X = 13.5f;
    float CENTER_Y = -13.5f;

    public GameObject cherryPrefab;


    // Start is called before the first frame update
    void Start()
    {
        center = new Vector2(13, -13);
        InvokeRepeating("emission_runnable", 0f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void emission_runnable() {
        StartCoroutine(emission());
    }

    IEnumerator emission() {
        Vector2 emissionPosition = pickRandomOnRing(CENTER_X, CENTER_Y);
        GameObject cherry = Instantiate(cherryPrefab, new Vector3(emissionPosition.x, emissionPosition.y, 0), Quaternion.identity);
        Vector2 direction = new Vector2(CENTER_X, CENTER_Y) - emissionPosition;
        Vector3 direction3 = new Vector3(direction.x, direction.y);
        direction3.Normalize();
        direction3 /= 12f;
        float startTime = Time.time;

        while(Time.time - startTime < 15f) {
            cherry.transform.position += direction3;
            yield return null;
        }

        try {
            Destroy(cherry);
        }
        catch(Exception) { }
    }



    Vector2 pickRandomOnRing(float x, float y) {
        int angle = UnityEngine.Random.Range(1, 360);
        float redians = (float)((Math.PI / 180) * angle);
        float nx, ny;
        nx = x + Mathf.Sin(redians) * R;
        ny = y + Mathf.Cos(redians) * R;

        return new Vector2(nx, ny);
    }

}
