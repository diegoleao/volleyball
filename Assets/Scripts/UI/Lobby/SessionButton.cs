
using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SessionButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;

    private SessionInfo sessionInfo;

    private UnityAction<SessionInfo> buttonClicked;

    public void SetData(SessionInfo sessionInfo, UnityAction<SessionInfo> clickedAction)
    {
        this.sessionInfo = sessionInfo;
        Text.text = $"Join Match: {sessionInfo.Name}";
        this.buttonClicked = clickedAction;

    }

    public void EnterSession()
    {
        buttonClicked?.Invoke(this.sessionInfo);

    }

}