using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimatedBorder : MonoBehaviour
{
    public GameObject objectToMove;
    public GameObject[] pathPoints;
    public int numberOfPoints;

    float speed;
    Vector3 currentPosition;
    int i;


    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        speed = 5f;
    }

    // Update is called once per frame
    void Update() {
        currentPosition = objectToMove.transform.position;
        objectToMove.transform.position = Vector3.MoveTowards(currentPosition, pathPoints[i].transform.position, speed * Time.deltaTime);

        if(currentPosition == pathPoints[i].transform.position) {
            i = (i + 1) % numberOfPoints; 
        }
    }

}
