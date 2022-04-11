using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : MonoBehaviour
{
    private int TargetMask;
    void Start()
    {
        TargetMask = LayerMask.GetMask("Target");//得到的是2的Target层级的次方
        
        //print(TargetMask);//输出看看是不是得到了值
        
    }
    void Update()
    {
        /*if (Physics.Raycast(transform.position, transform.forward, 100.0f, TargetMask))
            Debug.Log("Hit something");*/
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从鼠标所在位置发射射线
        RaycastHit hit;//设置一个被射线射到的物体
        if (Physics.Raycast(ray,out hit,100.0F,TargetMask))//使这个射线往原定方向射100米并且只与你设置的Layer层级相撞
        {
            Vector3 offset = new Vector3(0,0,5);//目标物体偏移量 
            hit.transform.position+= offset;//使目标物体偏移
        }  
    }
 
}
