using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Render : MonoBehaviour
{
    public Texture2D tex;
    public Cubemap cumap;
    public Cubemap cumap1;
    public Texture matTex;
    public Material ma;
    public Texture2D texFix;
    private Byte[] bytes;
    int width;
    private void Start()
    {
        width = 32;
        cumap = new Cubemap(width,TextureFormat.ARGB32,false);
        cumap1 = new Cubemap(width,TextureFormat.ARGB32, false);
        //拿到材质组件
        //ma = gameObject.GetComponent<MeshRenderer>().material;
        //拿到material里面叫_MainTex的贴图
        //matTex = ma.GetTexture("_MainTex");
        tex = new Texture2D(width,width);
        texFix = new Texture2D(width,width);
        //bytes = new byte[Screen.height*Screen.width];
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Camera.main.RenderToCubemap(cumap);            
            //tex.SetPixels(cumap.GetPixels(CubemapFace.PositiveZ)) ;
            //拿到z方向上的贴图（是反的后面要反过来）
            Color[] cmmcolors = cumap.GetPixels(CubemapFace.PositiveZ);
            //给要复制的东西提供位置
            Color[] ReCmmcolors = new Color[cmmcolors.Length]; 
            //遍历贴图x方向把x方向上每个点的y值反过来(实现将图上下翻转)
            for (int i = 1; i < width + 1; i++)
                Array.Copy(cmmcolors, width * (width - i), ReCmmcolors, width * (i - 1), width);//上下翻转
            //将这个立方贴图的值给这个2d图
            tex.SetPixels(ReCmmcolors);
            tex.Apply();
            //img.sprite = Sprite.Create(tex, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            //ma.SetTexture("_MainTex", tex);
            bytes = tex.EncodeToPNG();
            string str2 = "";
            for (int i = 0; i < bytes.Length; i++) {
                int t = (int)bytes[i];
                str2 += t.ToString() + " ";
            }
            print("生成的图片 " + str2);
            /*texFix.LoadImage(bytes);
            texFix.Apply();
            cumap1.SetPixels(texFix.GetPixels(), CubemapFace.PositiveZ);
            cumap1.Apply();*/
            if (NetMgr.srvConn.status != Connection.Status.Connected)
            {
                string host = "192.168.1.15";
                int port = 1234;
                NetMgr.srvConn.proto = new ProtocolTexture();
                if (!NetMgr.srvConn.Connect(host, port))
                {
                    PanelMgr.instance.OpenPanel<TipPanel>("", "连接服务器失败 !");
                }
            }
            //发送
            ProtocolTexture pro = new ProtocolTexture();
            pro.AddString("Fw");
            pro.AddTex(bytes);
            Debug.Log("发送 " + pro.GetDesc());
            NetMgr.srvConn.Send(pro, OnTextureBack);
        }
    }
    public void OnTextureBack(ProtocolBase proto)
    {
        ProtocolTexture pro = (ProtocolTexture)proto;
        //ProtocolBytes pro = (ProtocolBytes)proto;
        int start = 0;
        //解析协议
        string name = pro.GetString(start, ref start);
        pro.GetTex();
        print(name+"  "+pro.texBytes.Length);
        texFix.LoadImage(pro.texBytes);
        texFix.Apply();
        cumap1.SetPixels(texFix.GetPixels(), CubemapFace.PositiveZ);
        cumap1.Apply();
    }
}
