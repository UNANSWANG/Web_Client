using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LoginPanel : PanelBase 
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn; 
    private Button regBtn;
    #region 生命周期
    //初始化
    public override void init(params object[] args) 
    { 
        base.init(args); 
        skinPath = "LoginPanel";
        layer = PanelMgr.PanelLayer.Panel; 
    }
    public override void OnShowing() 
    { 
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>(); 
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
    }


    #endregion
    private void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        Close();
    }
    private void OnLoginClick()
    {
        if (idInput.text==null||pwInput.text==null)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "用户名密码不能为空 !");
        }
        if (NetMgr.srvConn.status!=Connection.Status.Connected)
        {
            string host = "192.168.1.15";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(host, port))
            {
                PanelMgr.instance.OpenPanel<TipPanel>("", "连接服务器失败 !");
            }
        }
        //发送
        ProtocolBytes pro = new ProtocolBytes();
        pro.AddString("Login");
        pro.AddString(idInput.text);
        pro.AddString(pwInput.text);
        Debug.Log("发送 "+pro.GetDesc());
        NetMgr.srvConn.Send(pro,OnLoginBack);
    }
    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        //解析协议
        string name = proto.GetString(start,ref start);
        int ret = proto.GetInt(start,ref start);
        if (ret==0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "登录成功 !");
            //登录成功后进入房间列表
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            GameMgr.instance.id = idInput.text;
            //Walk.instance.StartGame(idInput.text);
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "登录失败，请检查用户名密码 !");
        }
    }
}