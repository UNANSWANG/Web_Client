using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : PanelBase
{
    private Text text;
    private Button btn;
    string str = "";
    #region 生命周期
    public override void init(params object[] args)
    {
        base.init(args);
        skinPath = "TipPanel";
        layer = PanelMgr.PanelLayer.Tips;
        if (args.Length==1)
        {
            str = (string)args[0];
        }
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        text = skinTrans.Find("Text").GetComponent<Text>();
        btn = skinTrans.Find("Btn").GetComponent<Button>();
        text.text = str;
        btn.onClick.AddListener(OnBtnClick);
    } 
    #endregion
    //消息框的知道了按钮，点击便关闭该窗口
    public void OnBtnClick()
    {
        Close();
    }
}
