using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsoler : MonoBehaviour
{
    // Start is called before the first frame update
    public Image im;
    private Image i;
    void Start()
    {
        im = Resources.Load<Image>("Image");
        i=Instantiate(im);
    }

    // Update is called once per frame
    void Update()
    {
        Creat();
        if (Input.GetAxis("Mouse ScrollWheel")>0)//前滚
        {
            print(">>>>");
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)//后滚
        {
            print("<<<<");
        }
    }

    private void Creat()
    {
        i.rectTransform.sizeDelta = new Vector2(20,20);
        i.rectTransform.anchoredPosition = new Vector2(0, 0);
        i.transform.SetParent(transform,false);
    }
}
