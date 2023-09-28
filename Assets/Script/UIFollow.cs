using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //camera = GetComponentInParent<Canvas>().worldCamera;
    }

    // Update is called once per frame
    void Update()
    {
        //((RectTransform)transform).anchoredPosition = camera.WorldToScreenPoint(target.transform.position) + (Vector3)offset;
    }
}
