using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : PanelBase
{
    private Button startBtn;
    private Button infoBtn;

    #region 生命周期
    public override void init(params object[] args)
    {
        base.init(args);
        skinPath = "TitlePanel";
        layer = PanelMgr.PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        infoBtn = skinTrans.Find("InfoBtn").GetComponent<Button>();
        startBtn.onClick.AddListener(OnStartClick);
        infoBtn.onClick.AddListener(OnInfoClick);
    }
    #endregion

    public void OnStartClick()
    {
        //开始游戏
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        //关闭
        Close();
    }
    public void OnInfoClick()
    {
        PanelMgr.instance.OpenPanel<InfoPanel>("");
    }
}
