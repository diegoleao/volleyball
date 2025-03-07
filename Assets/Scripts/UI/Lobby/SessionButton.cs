
using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SessionButton : BaseView
{
    [SerializeField] TextMeshProUGUI Text;

    private SessionInfo sessionInfo;

    private UnityAction<SessionInfo> buttonClicked;

    protected override void OnFirstExibition()
    {

    }

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