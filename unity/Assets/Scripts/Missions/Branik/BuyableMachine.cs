using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyableMachine : MonoBehaviour {
    [SerializeField] private BranikMachines machine;
    [SerializeField] private string machineName;
    [SerializeField] private int cost;
    [SerializeField] private GameObject constructionGO;
    [SerializeField] private TextMeshPro constructionText;
    [SerializeField] private GameObject machineGO;
    [SerializeField] private Trigger buyTrigger;

    private BranikState branikState;
    private PlayerState playerState;

    private void Start() {

        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        GameState gameState = gameStateGO.GetComponent<GameState>();
        if (gameState == null) {
            Debug.LogError("No game state found.");
            return;
        }

        branikState = gameState.GetBranikState();
        playerState = gameState.GetPlayerState();

        if (branikState.IsOwned(machine)) {
            SetupOwnedState();
        } else {
            SetupUnownedState();
            buyTrigger.activationEvent.AddListener(Buy);
        }

    }


    private void Buy() {
        if (playerState.GetMoney() < cost) {
            return;
        }

        playerState.SetMoney(playerState.GetMoney() - cost);
        branikState.MarkAsOwned(machine);
        SetupOwnedState();
    }

    private void SetupOwnedState() {
        constructionGO.SetActive(false);
        machineGO.SetActive(true);
        buyTrigger.SetIsEnabled(false);
    }

    private void SetupUnownedState() {
        constructionGO.SetActive(true);
        constructionText.text = $"{machineName}\n{cost},-";
        machineGO.SetActive(false);
        buyTrigger.SetIsEnabled(true);
        buyTrigger.onlyOnce = false;
    }

}
