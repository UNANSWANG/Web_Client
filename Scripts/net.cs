using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;

public class net : MonoBehaviour
{
    Socket socket;
    public InputField hostInput;
    public InputField portInput;
    public InputField senTextInput;
    public InputField idInput;
    public InputField pwInput;

    public Text clientText;
    public Text recText;

    const int BUFFER_SIZE = 1024;
    byte[] readBuffer = new byte[BUFFER_SIZE];
    public string recStr;
    //协议
    ProtocolBase proto = new ProtocolBytes();

    private void Update()
    {
        recText.text = recStr;
    }

    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        string host = hostInput.text;
        int port = int.Parse(portInput.text);

        socket.Connect(host,port);
        clientText.text = "客户端地址：" + socket.LocalEndPoint.ToString();
        string str = "Hello Unity";
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        socket.Send(bytes);
        int count = socket.Receive(readBuffer);
        str = System.Text.Encoding.UTF8.GetString(readBuffer,0,count);
        recText.text += str;
        socket.Close();
    }

    public void Connection2()
    {
        recText.text = "";
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        string host = "192.168.1.8";
        int port = int.Parse(portInput.text);
        socket.Connect(host,port);
        clientText.text = "客户端地址：" + socket.LocalEndPoint.ToString();
        socket.BeginReceive(readBuffer,0,BUFFER_SIZE,SocketFlags.None,RecCb,null);
    }
    public void RecCb(IAsyncResult ar)
    {
        try
        { 
            int count = socket.EndReceive(ar);
            /*string str = System.Text.Encoding.UTF8.GetString(readBuffer,0,count);
            if (recStr.Length>300)
            {
                recStr = "";
            }
            recStr += str + "\n";*/
            ProcessData();
            socket.BeginReceive(readBuffer,0,BUFFER_SIZE,SocketFlags.None,RecCb,null);
        }
        catch (Exception)
        {

            recStr += "接收异常";
            socket.Close();
        }
    }
    //处理信息
    void ProcessData()
    {
        //小于字节长度
        if (readBuffer.Length< sizeof(Int32))
        {
            return;
        }
        byte[] lenByte=new byte[sizeof(Int32)];
        Array.Copy(readBuffer,lenByte, sizeof(Int32));
        int msgLen = BitConverter.ToInt32(lenByte, 0);
        //小于最小要求长度则返回表示未接收完全
        if (readBuffer.Length < readBuffer.Length + sizeof(Int32))
        {
            return;
        }
        ProtocolBase protocol = proto.Decode(readBuffer, sizeof(Int32),msgLen);
        HandleMsg(protocol);
        //清除已处理的消息
    }
    //消息的最后处理
    void HandleMsg(ProtocolBase pro)
    {
        //测试
        /* ProtocolBytes proto = (ProtocolBytes)pro;
         Debug.Log("接收 "+proto.GetDesc());*/

        ProtocolBytes proto = (ProtocolBytes)pro;
        //获取数值
        int start = 0;
        string protoName = proto.GetString (start, ref start); 
        int ret = proto.GetInt (start,ref start); 
        //显示
        Debug.Log ("接收 " + proto.GetDesc () ); 
        recStr = "接收 " + proto.GetName() + " " + ret.ToString();
    }

    public void send()
    {
        //正常的发送
    /*  string str = senTextInput.text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        byte[] length = BitConverter.GetBytes(bytes.Length);
        将长度和信息粘合
        byte[] sendbuff = length.Concat(bytes).ToArray();
        try
        {
            socket.Send(sendbuff);
            socket.Send(bytes);
        }
        catch (Exception)
        {

            recStr += "传输错误";
            socket.Close();
        }*/

        //使用协议发送
        ProtocolBytes pro = new ProtocolBytes();
        //已经封装好的转化函数
        pro.AddString("HeaTBeat");
        Debug.Log("发送 "+ pro.GetDesc());
        SendPro(pro);
    }

    public void SendPro(ProtocolBase pro)
    {
        ///
        byte[] bytes = pro.Encode();
        byte[] length = BitConverter.GetBytes(bytes.Length);
        //将长度和信息粘合在一起
        byte[] sendbuff = length.Concat(bytes).ToArray();
        socket.Send(sendbuff);
    }
    //登录
    public void OnLogin()
    {
        ProtocolBytes pro = new ProtocolBytes();
        pro.AddString("Login");
        pro.AddString(idInput.text);
        pro.AddString(pwInput.text);
        Debug.Log("发送 " +pro.GetDesc());
        SendPro(pro);
    }
    //增加分数
    public void OnAddScore()
    {
        ProtocolBytes pro = new ProtocolBytes();
        pro.AddString("AddScore");
        Debug.Log("发送 " + pro.GetDesc());
        SendPro(pro);
    }
    //获得分数
    public void OnGetScore()
    {
        ProtocolBytes pro = new ProtocolBytes();
        pro.AddString("GetScore");
        Debug.Log("发送 " + pro.GetDesc());
        SendPro(pro);
    }
}
