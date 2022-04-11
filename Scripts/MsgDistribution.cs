using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgDistribution
{

    //每帧处理消息的数量
    public int num = 15;
    //消息列表
    public List<ProtocolBase> msgList = new List<ProtocolBase>(); 
    //委托类型
    public delegate void Delegate(ProtocolBase proto);
    //事件监听表
    private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
    private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();

    //需要updata的方法,在其他类调用他
    public void Update()
    {
        //每帧处理num条信息
        for (int i = 0; i < num; i++)
        {
            if (msgList.Count>0)
            {
                //对第一个信息进行处理
                DispatchMsgEvent(msgList[0]);
                //防止异步对这个消息列表占线
                lock (msgList)
                {
                    //把第一个去除
                    msgList.RemoveAt(0);
                }
            }
            else
            {
                break;
            }
        }
    }
    //消息分发
    public void DispatchMsgEvent(ProtocolBase pro)
    {
        string name = pro.GetName();
        Debug.Log("分发信息 "+name);
        if (eventDict.ContainsKey(name))
        {
            if (name== "GetRoomInfo") {
            }
            eventDict[name](pro);
        }
        //一次性调用，用完就删 

        else if(onceDict.ContainsKey(name))
        {
            onceDict[name](pro);
            onceDict[name] = null;
            onceDict.Remove(name);
        }
    }
    //添加监听事件
    public void AddListener(string name, Delegate cb)
    {
        if (eventDict.ContainsKey(name)) {
            eventDict[name] += cb;
        }
        else {
            eventDict[name] = cb;
        }
    }
    //删除监听事件
    public void DelListener(string name, Delegate cb) 
    { 
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] -= cb;
            if (eventDict[name] == null) 
                eventDict.Remove(name);
        } 
    }
    //添加单次监听事件
    public void AddOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
            onceDict[name] += cb;
        else onceDict[name] = cb; 
    }
    //删除单次监听事件
    public void DelOnceListener(string name, Delegate cb)
    {
        if (onceDict.ContainsKey(name))
        { 
            onceDict[name] -= cb; 
            if (onceDict[name] == null) 
                onceDict.Remove(name); 
        } 
    }
}
