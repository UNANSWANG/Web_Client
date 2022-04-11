using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;

    #region 生命周期
    public override void init(params object[] args)
    {
        base.init(args);
        skinPath = "RoomPanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //将6个玩家位置加入预制体链表
        for (int i = 0; i < 6; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(name);
            prefabs.Add(prefab);
        }
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        //按钮事件
        startBtn.onClick.AddListener(OnStartClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        //监听
        NetMgr.srvConn.msgDisc.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDisc.AddListener("Fight", RecvFight);
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.srvConn.Send(protocol);
        #region 测试
        /*protocol.AddInt(2);

        protocol.AddString("aaaa");
        protocol.AddInt(1);
        protocol.AddInt(15);
        protocol.AddInt(18);
        protocol.AddInt(0);

        protocol.AddString("bbbb");
        protocol.AddInt(2);
        protocol.AddInt(3);
        protocol.AddInt(8);
        protocol.AddInt(1);
        RecvGetRoomInfo(protocol);*/
        #endregion
    }

    #endregion
    //重写关闭函数，关闭该面板时将监听关闭
    public override void OnClosing()
    {
        NetMgr.srvConn.msgDisc.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDisc.DelListener("Fight", RecvFight);
        //因为协议可能只返回一个Fight协议不返回StartFight协议
        NetMgr.srvConn.msgDisc.DelOnceListener("StartFight",OnStartBack);
    }

    //房间信息的回调函数
    private void RecvGetRoomInfo(ProtocolBase protocol)
    {
        ProtocolBytes pro = (ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start, ref start);
        int count = pro.GetInt(start, ref start);
        //逐个处理,因为i不一定用完所以在外面定义可以接着使用i的的数值
        int i = 0;
        print(count+"   "+name);
        for (i = 0; i < count; i++)
        {
            string id = pro.GetString(start, ref start);
            int team = pro.GetInt(start, ref start);
            int win = pro.GetInt(start, ref start);
            int fail = pro.GetInt(start, ref start);
            int isOwner = pro.GetInt(start, ref start);
            //信息处理
            Transform tran = prefabs[i];
            Text text = tran.Find("Text").GetComponent<Text>();
            string str = "名字: " + id + "\r\n";
            str += "阵营: " + (team == 1 ? "红" : "蓝")+"\r\n";
            str += "胜利: " + win.ToString() + " ";
            str += "失败: " + fail.ToString() + "\r\n";
            if (id==GameMgr.instance.id)
            {
                str += "[我自己]  ";
            }
            if (isOwner==1)
            {
                str += "[房主]";
            }
            text.text = str;
            //不同阵营不同颜色1为红色方，其他为蓝色方
            if (team==1)
            {
                tran.GetComponent<Image>().color = Color.red;
            }
            else
            {
                tran.GetComponent<Image>().color = Color.blue;
            }
        }
        for (; i< 6; i++)
        {
            Transform tran = prefabs[i];
            Text text = tran.Find("Text").GetComponent<Text>();
            text.text = "[等待玩家]";
            tran.GetComponent<Image>().color =Color.gray;
        }
    }
    //关闭按钮
    public void OnCloseClick()
    {
        ProtocolBytes pro = new ProtocolBytes();
        pro.AddString("LeaveRoom");
        NetMgr.srvConn.Send(pro,OnCloseBack);
    }
    //关闭的回调函数
    public void OnCloseBack(ProtocolBase protocol)
    {
        ProtocolBytes pro = (ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start, ref start);
        int ret = pro.GetInt(start, ref start);
        if (ret==0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("","退出成功!");
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败!");
        }
    }
    //开始按钮
    public void OnStartClick()
    {
        ProtocolBytes pro = new ProtocolBytes();
        pro.AddString("StartFight");
        NetMgr.srvConn.Send(pro, OnStartBack);
    }
    //开始按钮的回调函数
    private void OnStartBack(ProtocolBase protocol)
    {
        ProtocolBytes pro = (ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start, ref start);
        int ret = pro.GetInt(start, ref start);
        if (ret != 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "开始游戏失败!两队至少都需要一名玩家，只有队长可以开始战斗! ");
        }
    }
    //战斗的回调函数
    public void RecvFight(ProtocolBase protocol)
    {
        ProtocolBytes pro=(ProtocolBytes)protocol;
        //后面将会实现的战斗功能

        Close();
    }
}
