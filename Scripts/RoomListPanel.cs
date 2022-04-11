using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : PanelBase
{
    public Text idText;
    public Text winText;
    public Text lostText;
    public Transform content;
    public GameObject roomPrefab;
    private Button closeBtn;
    private Button newBtn;
    private Button reflashBtn;

    #region 生命周期
    public override void init(params object[] args)
    {
        base.init(args);
        skinPath = "RoomListPanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        //获取Transform
        Transform skinTrans = skin.transform;
        Transform winTrans= skinTrans.Find("WinImage");
        //获取成绩栏部件
        idText = winTrans.Find("idText").GetComponent<Text>();
        winText = winTrans.Find("WinText").GetComponent<Text>();
        lostText = winTrans.Find("LostText").GetComponent<Text>();
        //获取列表栏物件
        Transform scrollRect = skinTrans.Find("ScrollRect");
        content = scrollRect.Find("Content");
        //roomPrefab = Resources.Load<GameObject>("Room");
        roomPrefab = content.Find("RoomPrefab").gameObject;
        //先关掉
        roomPrefab.SetActive(false);
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        newBtn = skinTrans.Find("NewBtn").GetComponent<Button>();
        reflashBtn = skinTrans.Find("ReflashBtn").GetComponent<Button>();
        //按钮事件
        reflashBtn.onClick.AddListener(OnReflashClick);
        newBtn.onClick.AddListener(OnNewClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        //开启监听
        NetMgr.srvConn.msgDisc.AddListener("GetAchieve",RecvGetAchieve);
        NetMgr.srvConn.msgDisc.AddListener("GetRoomList",RecvGetRoomList);
        //发送查询
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("GetRoomList");
        #region 测试
        /*proto.AddInt(2);

        proto.AddInt(2);
        proto.AddInt(1);

        proto.AddInt(3);
        proto.AddInt(4);
        RecvGetRoomList(proto);*/
        #endregion
        NetMgr.srvConn.Send(proto);

        proto = new ProtocolBytes();
        proto.AddString("GetAchieve");
        //NetMgr.srvConn.Send(proto);
    }


    #endregion
    //重写关闭函数，使得退出去后不在监听，防止调用已经关闭的组件
    public override void OnClosing() 
    {
        NetMgr.srvConn.msgDisc.DelListener("GetAchieve", RecvGetAchieve);
        NetMgr.srvConn.msgDisc.DelListener("GetRoomList", RecvGetRoomList);
    }
    //对玩家房间状态赋值
    public void RecvGetAchieve(ProtocolBase protocol)
    {
        ProtocolBytes pro = (ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start,ref start);
        int win = pro.GetInt(start,ref start);
        int lost = pro.GetInt(start, ref start);
        idText.text = "指挥官: " + GameMgr.instance.id;
        winText.text = "胜: "+win.ToString();
        lostText.text = "败: "+lost.ToString();
    }
    //房间列表的更新
    public void RecvGetRoomList(ProtocolBase protocol)
    {
        ClearRoomUnit();
        ProtocolBytes pro=(ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start, ref start);
        int count = pro.GetInt(start, ref start);
        for (int i = 0; i < count; i++)
        {
            int num = pro.GetInt(start,ref start);
            int status = pro.GetInt(start, ref start);
            GenerateRoomUnit(i,num,status);
        }
    }
    //清空已经生成的房间
    public void ClearRoomUnit()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).name.Contains("Clone"))
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
    //创建房间
    public void GenerateRoomUnit(int i,int num ,int status)
    {
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(250,(i+1)*110);
        GameObject o = Instantiate(roomPrefab);
        o.transform.SetParent(content);
        o.SetActive(true);
        Transform trans = o.transform;
        Text nameText = trans.Find("nameText").GetComponent<Text>();
        Text countText = trans.Find("countText").GetComponent<Text>();
        Text statusText = trans.Find("statusText").GetComponent<Text>();
        nameText.text = "序号: " + i;
        countText.text = "人数: " + num;
        if (status==1)
        {
            statusText.color = Color.black;
            statusText.text = "状态: " + "准备中";
        }
        else
        {
            statusText.color = Color.red;
            statusText.text = "状态: " + "开战中";
        }
        //添加按钮事件
        Button btn = trans.Find("JoinBtn").GetComponent<Button>();
        //改名字方便后面监听，但是不改按钮上面的文字
        btn.name=i.ToString();
        //因为加入监听的函数里面有参数所以我们要用匿名委托
        btn.onClick.AddListener(
            delegate ()
            {
                OnJoinBtnClick(btn.name);
            }
            );
    }
    //刷新按钮
    public void OnReflashClick()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("GetRoomList");
        NetMgr.srvConn.Send(proto);
    }
    //加入房间按钮
    public void OnJoinBtnClick(string name)
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("EnterRoom");
        proto.AddInt(int.Parse(name));
        //为点击按钮增加一个回调函数，当然，是一次性的
        NetMgr.srvConn.Send(proto,OnJoinBtnBack);
        Debug.Log("请求进入房间 "+name);
    }
    //加入房间的回调函数
    public void OnJoinBtnBack(ProtocolBase protocol)
    {
        ProtocolBytes pro = (ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start, ref start);
        int ret = pro.GetInt(start, ref start);
        if (ret==0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("","成功进入房间! ");
            PanelMgr.instance.OpenPanel<RoomPanel>("");
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "进入房间失败! ");
        }
    }
    //新建按钮
    public void OnNewClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CreateRoom");
        NetMgr.srvConn.Send(protocol, OnNewBack);
    }
    //新建按钮的回调函数
    public void OnNewBack(ProtocolBase protocol)
    {
        ProtocolBytes pro = (ProtocolBytes)protocol;
        int start = 0;
        string name = pro.GetString(start, ref start);
        int ret = pro.GetInt(start, ref start);
        if (ret == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("","创建成功! ");
            PanelMgr.instance.OpenPanel<RoomPanel>("");
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "创建失败! ");
        }
    }
    //登出按钮
    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Logout");
        NetMgr.srvConn.Send(protocol,OnCloseBack);
    }
    //登出的回调函数
    public void OnCloseBack(ProtocolBase protocol)
    {
        PanelMgr.instance.OpenPanel<TipPanel>("", "登出成功! ");
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        NetMgr.srvConn.Close();
        //关闭该面板
        Close();
    }
}
