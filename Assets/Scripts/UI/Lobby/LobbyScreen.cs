using Fusion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

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

    private void CreateSessionButtons(List<SessionInfo> newSessionList)
    {
        DestroyAllCurrentButtons();
        SessionButton currentSessionButton;
        newSessionList.ForEach(sessionInfo =>
        {
            currentSessionButton = Instantiate(SessionButtonPrefab, this.SessionsParent);
            currentSessionButton.SetData(sessionInfo, (chosenSession) =>
            {
                this.LobbyComponent.JoinSession(sessionInfo.Name);
                this.Close();

            });

        });

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

}