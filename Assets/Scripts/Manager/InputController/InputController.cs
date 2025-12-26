using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    static GameAction _inputAction;
    public static GameAction InputAction { get => _inputAction; }

    InputActionAsset inputAsset;
    InputActionRebindingExtensions.RebindingOperation currentRebind;

    public UnityAction<(InputAction,int)> OnRebindStart;
    public UnityAction<(InputAction,int)> OnRebindComplete;
    public UnityAction OnLoadComplete;


    const string SAVE_KEY = "InputBindingOverrides";

    private static InputController _instance;
    public static InputController instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<InputController>();
                if (!_instance)
                {
                    return null;
                }
            }
            return _instance;
        }
    }

    public void Init()
    {
        _inputAction = new();
        inputAsset = _inputAction.asset;
        _inputAction.Enable();
        CloseAllActionMaps();
    }

    public void CloseAllActionMaps()
    {
        foreach (var action_map in _inputAction.asset.actionMaps)
        {
            action_map.Disable();
        }
        _inputAction.System.Enable();
    }

    public void SelectActionMap(params InputActionMap[] maps)
    {
        CloseAllActionMaps();
        foreach(var map in maps)
        {
            map.Enable();
        }
    }

    public void StartRebind(InputAction action, int bindingIndex)
    {
        CancelRebind();

        action.Disable();
        OnRebindStart?.Invoke((action,bindingIndex));

        currentRebind = action.PerformInteractiveRebinding(bindingIndex)
            .WithExpectedControlType("Button")
            .OnPotentialMatch(op =>
            {
                var control = op.selectedControl;
                var device = control.device;

                // ESC 键
                if (control.path == "<Keyboard>/escape")
                {
                    op.Cancel();
                    return;
                }

                // 任意鼠标输入（左键 / 右键 / 滚轮 / 移动）
                if (control.device is Mouse)
                {
                    // Mouse button 才算
                    if (control is UnityEngine.InputSystem.Controls.ButtonControl)
                    {
                        op.Cancel();
                        return;
                    }

                    // 鼠标移动 / 滚轮 → 忽略
                    return;
                }

                // 其他合法输入 → 正常完成
                op.Complete();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                action.Enable();
                OnRebindComplete?.Invoke((action,bindingIndex));
            })
            .OnComplete(op =>
            {
                op.Dispose();
                action.Enable();

                ResolveConflictAndSwap(action, bindingIndex);

                OnRebindComplete?.Invoke((action,bindingIndex));
            });

        currentRebind.Start();
    }

    void CancelRebind()
    {
        if (currentRebind != null)
        {
            currentRebind.Cancel();
            currentRebind.Dispose();
            currentRebind = null;
        }
    }

    void ResolveConflictAndSwap(InputAction targetAction, int targetBindingIndex)
    {
        string newPath = targetAction.bindings[targetBindingIndex].effectivePath;
        var map = targetAction.actionMap;

        foreach (var action in map.actions)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action == targetAction && i == targetBindingIndex)
                    continue;

                var binding = action.bindings[i];
                if (binding.effectivePath == newPath && !binding.isPartOfComposite)
                {
                    SwapBinding(action, i, targetAction, targetBindingIndex);
                    return;
                }
            }
        }
    }

    void SwapBinding(InputAction actionA, int indexA,InputAction actionB, int indexB)
    {
        string pathA = actionA.bindings[indexA].overridePath ?? actionA.bindings[indexA].path;

        string pathB = actionB.bindings[indexB].overridePath ?? actionB.bindings[indexB].path;

        actionA.ApplyBindingOverride(indexA, pathB);
        actionB.ApplyBindingOverride(indexB, pathA);
    }

    public void SaveBindings()
    {
        string json = inputAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("Save Binding");
    }

    public void LoadBindings()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            inputAsset.LoadBindingOverridesFromJson(PlayerPrefs.GetString(SAVE_KEY));
            Debug.Log("Load Binding");
        }

        OnLoadComplete.Invoke();
    }

    public void ResetToDefault()
    {
        inputAsset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(SAVE_KEY);
        OnLoadComplete.Invoke();
    }

    public string GetBindingDisplay(InputAction action, int bindingIndex = 0)
    {
        return action.GetBindingDisplayString(bindingIndex,InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }
}
