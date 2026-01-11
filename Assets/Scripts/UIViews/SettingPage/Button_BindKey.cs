using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Button_BindKey : BaseButton_Setting
{
    public override UIView currentView => this;

    public override int ID => _id;

    public BindKey bindKey;
    (InputAction,int) bindInfo;
    TMP_Text text;
    const string WaitForInputStr = "请输入";

    static int _keyID=0;
    static int KeyID{
        get
        {
            return _keyID++;
        }
    }
    int _id = KeyID;

    public override void Init()
    {
        base.Init();
        text = transform.Find("key").GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        bindInfo = GetBindInfo(bindKey);
        InputController.instance.OnRebindStart += WaitForInputHandler;
        InputController.instance.OnRebindComplete += RefreshUIHandler;
        InputController.instance.OnLoadComplete += RefreshUIHandler;
    }

    void OnDisable()
    {
        if(!InputController.instance)return;

        InputController.instance.OnRebindStart-= WaitForInputHandler;
        InputController.instance.OnRebindComplete -= RefreshUIHandler;
        InputController.instance.OnLoadComplete -= RefreshUIHandler;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        InputController.instance.StartRebind(bindInfo.Item1,bindInfo.Item2);
    }

    void WaitForInputHandler((InputAction,int) info)
    {
        if(bindInfo!=info)return;
        RefreshUI(WaitForInputStr,true);
    }

    void RefreshUIHandler()
    {
        RefreshUI(InputController.instance.GetBindingDisplay(bindInfo.Item1,bindInfo.Item2),false);
    }

    void RefreshUIHandler((InputAction,int) info)
    {
        RefreshUIHandler();
    }

    void RefreshUI(string str,bool focus = false)
    {
        if (!this || !gameObject) return;
        isSelect = focus;
        text.text = str;
        DoExit();
    }

    public static (InputAction,int) GetBindInfo(BindKey key)
    {
        return key switch
        {
            BindKey.Rotate_clockwise=>(InputController.InputAction.PVEMap.Rotate,2),
            BindKey.Rotate_anticlockwise=>(InputController.InputAction.PVEMap.Rotate,1),
            BindKey.NextState=>(InputController.InputAction.PVEMap.NextState,0),
            _ => (InputController.InputAction.System.ESC,0),
        };
    }

    public enum BindKey
    {
        Rotate_clockwise,
        Rotate_anticlockwise,
        NextState,
    }
}
