using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(0,110);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
