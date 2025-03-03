using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : BaseView
{
    [SerializeField] LobbyScreen LobbyPrefab;
    [SerializeField] Button QuickJoinButton;

    private LobbyScreen lobbyInstance;

    void Start()
    {
        QuickJoinButton.interactable = false;
        CreateHiddenLobby();
        QuickJoingTextMesh().text = "JOIN (CHECKING FOR AVAILABLE SESSIONS...)";
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
            if (lobbyInstance.QuickJoinFirstSession()) 
                Close();

        }

    }

    public void StartSinglePlayer()
    {
        //Provider.Instance.GameState.StartSingleplayerMatch();
        //Close();

    }

    public void StartLocalMultiplayer()
    {
        //Start
        //Close();

    }

    public void EnterLobby()
    {
        this.lobbyInstance.Show();
        this.Hide();
    }

    private void CreateHiddenLobby()
    {
        if (this.lobbyInstance == null)
        {
            this.lobbyInstance = Instantiate(LobbyPrefab);
            this.lobbyInstance.Initialize(this);
            this.lobbyInstance.SubscribeToSessionChanges(SessionsUpdateHandler);
        }

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

    public override void Close()
    {
        if (isClosed)
            return;

        this.lobbyInstance.Close();
        base.Close();

    }

}