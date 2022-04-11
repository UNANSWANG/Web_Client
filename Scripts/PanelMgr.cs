using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PanelMgr : MonoBehaviour
{
    //单例
    public static PanelMgr instance;
    //画板
    public GameObject canvas;
    //面板
    public Dictionary<string, PanelBase> dict;
    //层级
    public Dictionary<PanelLayer, Transform> layerDict;


    ///分层类型
    public enum PanelLayer 
    { 
        //面板
        Panel,
        //提示
        Tips,
    }


    //开始
    public void Awake()
    {
        instance = this;
        InitLayer();
        dict = new Dictionary<string, PanelBase>();
    }

    //初始化层级
    public void InitLayer()
    {
        //画布
        canvas = GameObject.Find("Canvas");
        if (canvas==null)
        {
            Debug.LogError("PanelMgr.InitLayer Fail, canvas is null");
        }
        //各个层级
        layerDict = new Dictionary<PanelLayer, Transform>();
        foreach (PanelLayer item in Enum.GetValues(typeof(PanelLayer)))
        {
            string name = item.ToString();
            Transform transform = canvas.transform.Find(name);
            if (transform==null)
            {
                Debug.LogError("dsad");
            }
            layerDict.Add(item,transform);
        }
    }
    //打开面板
    public void OpenPanel<T>(string skinPath,params object[] args) where T:PanelBase
    {
        //已经打开
        string name = typeof(T).ToString();
        if (dict.ContainsKey(name))
        {
            return;
        }
        //面板脚本
        PanelBase panel = canvas.AddComponent<T>();
        panel.init(args);
        dict.Add(name,panel);
        //加载皮肤
        skinPath = (skinPath != "" ? skinPath : panel.skinPath);
        GameObject skin = Resources.Load<GameObject>(skinPath);
        if (skin==null)
        {
            Debug.LogError("PanelMgr.OpenPanel Fail, skin is null skinPath= "+skinPath);
        }
        panel.skin = (GameObject)Instantiate(skin);
        //坐标，以canvas下面的panel为父物体
        Transform skinTrans = panel.skin.transform;
        PanelLayer layer= panel.layer;
        Transform parant = layerDict[layer];
        skinTrans.SetParent(parant,false);
        //panel的生命周期
        panel.OnShowing();

        panel.OnShowed();
    }
    //关闭面板
    public void ClosePanel(string name)
    {
        PanelBase panel = (PanelBase)dict[name];
        if (panel==null)
        {
            return;
        }
        panel.OnClosing();
        dict.Remove(name);
        panel.OnClosed();
        GameObject.Destroy(panel.skin);
        Component.Destroy(panel);
    }
}
