using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{

    private void Start()
    {
        Application.runInBackground = true;
        //PanelMgr.instance.OpenPanel<TitlePanel>("");
        // PanelMgr.instance.OpenPanel<RoomPanel>("");
        PanelMgr.instance.OpenPanel<LoginPanel>("");
    }
    void Update()
    {
        NetMgr.Update();
    }
}
