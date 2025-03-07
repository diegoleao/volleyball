using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : BaseView
{
    [SerializeField] Button QuickJoinButton;
    [SerializeField] Button LocalMultiplayer;

    void Start()
    {
        QuickJoinButton.interactable = false;
        CreateHiddenLobby();
        QuickJoingTextMesh().text = "JOIN (CHECKING FOR AVAILABLE SESSIONS...)";
#if !UNITY_STANDALONE_WIN
        LocalMultiplayer.gameObject.SetActive(false);
#endif

    }

    protected override void OnFirstExibition()
    {

    }

    protected override void OnClose()
    {
        GetView<LobbyScreen>()?.Close();

    }

    public void StartMatch(bool isHost)
    {
        if (isHost)
        {
            Provider.Instance.GameState.StartMultiplayerMatch($"Match {Guid.NewGuid()}", GameMode.Host);
            Close();
        }
        else
        {
            if (GetView<LobbyScreen>().QuickJoinFirstSession())
                Close();

        }

    }

    public void StartSinglePlayer()
    {
        Provider.Instance.GameplayFacade.StartSingleplayerMatch();
        Close();

    }

    public void StartLocalMultiplayer()
    {
        Provider.Instance.GameplayFacade.StartLocalMatch();
        Close();

    }

    public void EnterLobby()
    {
        GetView<LobbyScreen>().Show();
        this.Hide();

    }

    private void CreateHiddenLobby()
    {
        this.GetOrCreate<LobbyScreen>().SubscribeToChanges(SessionsUpdateHandler);

    }

    private void SessionsUpdateHandler(List<SessionInfo> newSessions)
    {
        QuickJoinButton.interactable = (newSessions.Count > 0);

        QuickJoingTextMesh().text
            = (QuickJoinButton.interactable) ? "JOIN (READY)" : "JOIN (CHECKING FOR AVAILABLE SESSIONS...)";

    }

    private TextMeshProUGUI QuickJoingTextMesh()
    {
        return QuickJoinButton.GetComponentInChildren<TextMeshProUGUI>();

    }

}