using Fusion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Menu MenuPrefab;
    [SerializeField] SessionButton SessionButtonPrefab;

    [Header("References")]
    [SerializeField] LobbyComponent LobbyComponent;
    [SerializeField] Transform SessionsParent;

    private bool isSessionListOutdated;

    void Start()
    {
        CreateSessionButtons(LobbyComponent.SessionList);

        StartListeningToChanges();

    }

    private void StartListeningToChanges()
    {
        LobbyComponent.SessionListUpdatedEvent.AddListener(MarkSessionListAsOutdated);

        Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(_ =>
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
            });

        });

    }

    private void DestroyAllCurrentButtons()
    {
        var allButtons = SessionsParent.GetComponentsInChildren<SessionButton>();
        foreach (var button in allButtons) { Destroy(button.gameObject); }
    }

    public void BackToMenu()
    {
        this.Close();
        Instantiate(MenuPrefab);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

}