using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private InputField repInput;
    private Button regBtn; 
    private Button closeBtn;
    #region 生命周期
    //初始化
    public override void init(params object[] args) 
    { 
        base.init(args); 
        skinPath = "RegPanel"; 
        layer = PanelMgr.PanelLayer.Panel;
    }
    public override void OnShowing() 
    { 
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        repInput = skinTrans.Find("RepInput").GetComponent<InputField>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>(); 
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        regBtn.onClick.AddListener(OnRegClick); 
        closeBtn.onClick.AddListener(OnCloseClick);
    }

    #endregion
    private void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>(""); 
        Close();
    }

    private void OnRegClick()
    {
        //用户名、密码为空
        if (idInput.text == "" || pwInput.text == "") 
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "用户名密码不能为空 !");
            return; 
        }
        //两次密码不同
        if (pwInput.text != repInput.text)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "两次输入的密码不同 !");
            return;
        }
        //连接服务器
        if (NetMgr.srvConn.status != Connection.Status.Connected) 
        { 
            string host = "192.168.1.8";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(host, port)) 
                PanelMgr.instance.OpenPanel<TipPanel>("", "连接服务器失败 !");
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes(); 
        protocol.AddString("Register"); 
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送 " + protocol.GetDesc()); 
        NetMgr.srvConn.Send(protocol, OnRegBack);
    }
    public void OnRegBack(ProtocolBase protocol)
    { 
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0) 
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "注册成功!");
            PanelMgr.instance.OpenPanel<LoginPanel>("");
            Close();
        }
        else 
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "注册失败，请更换用户名 !");
        }
    }
}
