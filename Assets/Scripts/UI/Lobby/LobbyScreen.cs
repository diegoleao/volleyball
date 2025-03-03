using Fusion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class LobbyScreen : BaseView
{
    [Header("Config")]
    [SerializeField] int UpdateDelay = 2;

    [Header("Prefabs")]
    [SerializeField] MainMenuScreen MenuPrefab;
    [SerializeField] SessionButton SessionButtonPrefab;

    [Header("References")]
    [SerializeField] LobbyComponent LobbyComponent;
    [SerializeField] Transform SessionsParent;

    private bool isSessionListOutdated;
    private MainMenuScreen menuInstance;

    public void Initialize(MainMenuScreen menuInstance)
    {
        this.Hide();

        this.menuInstance = menuInstance;

        CreateSessionButtons(LobbyComponent.SessionList);

        ListenToSessionChanges();

        LobbyComponent.Initialize();

    }

    public void SubscribeToSessionChanges(UnityAction<List<SessionInfo>> SessionListUpdated)
    {
        this.LobbyComponent.SessionListUpdatedEvent.AddListener(SessionListUpdated);

    }

    public bool QuickJoinFirstSession()
    {
        if (this.LobbyComponent.SessionList.Count == 0)
            return false;

        JoinSessionByName(this.LobbyComponent.SessionList[0].Name);

        return true;
    }

    private void JoinSessionByName(string sessionName)
    {
        this.LobbyComponent.JoinSession(sessionName);
        this.Close();
    }

    private void ListenToSessionChanges()
    {
        LobbyComponent.SessionListUpdatedEvent.AddListener(MarkSessionListAsOutdated);

        Observable.Interval(TimeSpan.FromSeconds(UpdateDelay)).Subscribe(_ =>
        {
            if (isSessionListOutdated)
            {
                CreateSessionButtons(LobbyComponent.SessionList);
                isSessionListOutdated = false;
            }
        });

    }

    private void MarkSessionListAsOutdated(List<SessionInfo> newSessionList)
    {
        isSessionListOutdated = true;

    }

    private void DestroyAllCurrentButtons()
    {
        if (this == null)
            return;

        var allButtons = SessionsParent.GetComponentsInChildren<SessionButton>();
        foreach (var button in allButtons) { Destroy(button.gameObject); }

    }

    private void CreateSessionButtons(List<SessionInfo> newSessionList)
    {
        DestroyAllCurrentButtons();
        SessionButton currentSessionButton;
        newSessionList.ForEach(sessionInfo =>
        {
            currentSessionButton = Instantiate(SessionButtonPrefab, this.SessionsParent);
            currentSessionButton.SetData(sessionInfo, (chosenSession) =>
            {
                JoinSessionByName(sessionInfo.Name);

            });

        });

    }

    public void BackToMenu()
    {
        this.Hide();
        menuInstance.Show();


    }

    public override void Close()
    {
        if (isClosed)
            return;

        base.Close();
        menuInstance.Close();

    }

}