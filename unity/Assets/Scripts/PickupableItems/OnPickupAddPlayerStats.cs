using UnityEngine;

[RequireComponent (typeof(LootItem))]
public class OnPickupAddPlayerStats : MonoBehaviour {

    [SerializeField] private int moneyIncrease;
    [SerializeField] private int healthPotionsIncrease;
    [SerializeField] private int damagePotionsIncrease;
    [SerializeField] private int speedPotionsIncrease;

    private PlayerState playerState;

    private void Start() {
        GameState gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state found."); return; }

        playerState = gameState.GetPlayerState();
        GetComponent<LootItem>().pickupEvent.AddListener(AddStats);
    }

    private void AddStats() {
        playerState.ChangeMoney(moneyIncrease);
        playerState.ChangeHealthPotions(healthPotionsIncrease);
        playerState.ChangeDamagePotions(damagePotionsIncrease);
        playerState.ChangeSpeedPotions(speedPotionsIncrease);
    }
}