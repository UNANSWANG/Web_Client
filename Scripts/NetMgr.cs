using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMgr
{
    //管理客户端的连接
    public static Connection srvConn = new Connection();
    //如有需要平台连接可以再加
    //public static Connection platformConn = new Connection();

    public static void Update()
    {
        srvConn.Update();
        //platformConn.Updata();
    }
    //心跳
    public static ProtocolBase GetHeatBeatProtocol()
    {
        //具体的发送内容根据服务端设定进行改动
        ProtocolBytes protocol = new ProtocolBytes(); 
        protocol.AddString("HeatBeat");
        return protocol;
    }
}
