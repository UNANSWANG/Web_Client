using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.IO;
using System;

public class Connection 
{
    //常量，字节长度
    const int BUFFER_SIZE = 131072;
    private Socket socket;
    private byte[] readBuffer = new byte[BUFFER_SIZE];
    //当前使用的字节数
    private int buffCount = 0;
    //粘包分包
    private Int32 msgLength = 0;
    private byte[] lenBytes = new byte[sizeof(Int32)];
    //协议
    public ProtocolBase proto;
    //心跳时间
    public float lastTickTime = 0;
    public float heatBeatTime = 30;
    //消息分发
    public MsgDistribution msgDisc = new MsgDistribution();
    //状态
    public enum Status
    {
        None,
        Connected,
    };
    public Status status = Status.None;

    //连接
    public bool Connect(string host, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.Connect(host,port);
            socket.BeginReceive(readBuffer,buffCount,BUFFER_SIZE-buffCount,SocketFlags.None,ReceiveCb,readBuffer);
            Debug.Log("连接成功");
            status = Status.Connected;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("连接失败 "+e.Message);
            return false;
        }
    }
    //异步回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount += count;
            ProcessData();
            socket.BeginReceive(readBuffer,buffCount,BUFFER_SIZE-buffCount,SocketFlags.None,ReceiveCb,readBuffer);
        }
        catch (Exception e)
        {
            Debug.Log("ReceiveCb 失败 :"+e.Message);
            status = Status.None;
        }
    }
    //消息处理
    public void ProcessData()
    {

        //粘包处理
        if (buffCount<sizeof(Int32))
        {
            return;
        }
        Debug.Log("buffCount  " + buffCount);
        //包体长度
        Array.Copy(readBuffer,lenBytes,sizeof(Int32));
        //转化成长度
        msgLength = BitConverter.ToInt32(lenBytes,0);
        if (buffCount<sizeof(Int32)+msgLength)
        {
            return;
        }
        //协议解码,如何解码看在其他界面将proto初始化成了什么类型的子类，02
        ProtocolBase pro = proto.Decode(readBuffer, sizeof(Int32),msgLength);
        Debug.Log("收到消息 :"+pro.GetName()+"  "+pro.GetDesc());
        //将信息加到消息列表
        lock (msgDisc.msgList)
        {
            msgDisc.msgList.Add(pro);
        }
        //消除已经处理过的信息
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuffer,sizeof(Int32)+msgLength,readBuffer,0,count);
        buffCount = count;
        //如果还有多余消息就接着处理
        if (buffCount>0)
        {
            ProcessData();
        }
    }
    //关闭连接
    public bool Close() 
    { 
        try 
        { 
            socket.Close(); 
            return true; 
        }
        catch (Exception e) 
        {
            Debug.Log("关闭失败 :" + e.Message); 
            return false; 
        } 
    }
    //发送信息的三个重载
    public bool Send(ProtocolBase protocol)
    {
        if (status != Status.Connected)
        {
            Debug.LogError("[Connection]还没连接就发送数据是不好的 ");
            return false;
        }
        byte[] b = protocol.Encode(); 
        byte[] length = BitConverter.GetBytes(b.Length); 
        byte[] sendbuff = length.Concat(b).ToArray(); 
        socket.Send(sendbuff); 
        Debug.Log("发送消息 " + protocol.GetDesc()); 
        return true;
    }
    public bool Send(ProtocolBase protocol, string cbName, MsgDistribution.Delegate cb)
    {
        if (status != Status.Connected)
            return false;
        msgDisc.AddOnceListener(cbName, cb);
        return Send(protocol); 
    }
    public bool Send(ProtocolBase protocol, MsgDistribution.Delegate cb)
    { 
        string cbName = protocol.GetName(); 
        return Send(protocol, cbName, cb);
    }

    public void Update()
    {
        //消息分发
        msgDisc.Update();
        //心跳
        if (status==Status.Connected)
        {
            if (Time.time-lastTickTime>heatBeatTime)
            {
                ProtocolBase protocol = NetMgr.GetHeatBeatProtocol();
                //把心跳协议发过去
                Send(protocol);
                lastTickTime = Time.time;
            }
        }
    }
}
