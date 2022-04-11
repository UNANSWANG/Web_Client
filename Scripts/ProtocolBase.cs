using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //协议的基类
public class ProtocolBase : MonoBehaviour
{
    public virtual ProtocolBase Decode(byte[] readBuffer, int start, int len)
    {
        //解码器，解码readbuffer从start开始的lenth字节
        return new ProtocolBase();
    }

    //编码器
    public virtual byte[] Encode()
    {
        return new byte[] { };
    }

    //协议名称，用于消息分发
    public virtual string GetName()
    {
        return "";
    }

    //描述
    public virtual string GetDesc()
    {
        return "";
    }
}
