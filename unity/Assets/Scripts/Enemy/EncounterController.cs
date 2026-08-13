using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

// Requires EventCtrler and ends event
[RequireComponent(typeof(EventController))]
public class EncounterController : MonoBehaviour {

    [SerializeField] private List<EnemyBatch> batches;
    [SerializeField] private float spawnDelay;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<Transform> scatterPoints;

    [SerializeField] private int maxAttackers = 3;
    private int currentAttackers = 0;

    private List<Transform> livingEnemies = new();
    private bool allEnemiesSpawned = false;

    private EventController eventController;

    private void Awake() {
        eventController = GetComponent<EventController>();

    }

    private void Start() {
        InitialChecks();
        eventController.eventStart.AddListener(StartEncounter);
    }

    public bool IsAttackerSlotAvailable() {
        return currentAttackers < maxAttackers;
    }

    public void AddAttacker() {
        currentAttackers++;
    }

    public void RemoveAttacker() {
        currentAttackers--;
    }

    public List<Transform> GetEnemies() {
        return livingEnemies;
    }

    public List<Transform> GetScatterPoints() {
        return scatterPoints;
    }

    public void StartEncounter() {
        StartCoroutine(SpawningLoop());
    } 

    /// <summary>
    ///     Coroutine spawning all enemies
    /// </summary>
    private IEnumerator SpawningLoop() {
        foreach (var batch in batches) {
            for (int i = 0; i < batch.count; i++) {
                yield return new WaitForSeconds(spawnDelay);
                SpawnEnemy(batch.enemy);
            }
        }
        allEnemiesSpawned = true;
    }

    /// <summary>
    ///     Instantiate Enemy and setup it properly
    /// </summary>
    /// <param name="pf_enemy">Enemy to instantiate</param>
    private void SpawnEnemy(GameObject pf_enemy) {
        Transform enemy = Instantiate(pf_enemy, GetRandomSpawnPoint().position, Quaternion.identity).transform;
        livingEnemies.Add(enemy);
        IEncounterEnemy ee = enemy.GetComponent<IEncounterEnemy>();
        ee.EncounterEnemyInit(this);
        ee.AddListenerToOnDeathEvent(OnEnemyDeath);
    }
    /// <summary>
    /// Handles counting defeated enemies and ending event
    /// </summary>
    private void OnEnemyDeath(Transform enemy) {
        
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        if (gameStateGO == null) { Debug.LogError("No game state object found.", gameObject); return; }
        GameState gameState = gameStateGO.GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state script found.", gameObject); return; }
        PlayerState playerState = gameState.GetComponent<GameState>().GetPlayerState();
        if (gameState == null) { Debug.LogError("No player state found.", gameObject); return; }
        // add to enemies defeated
        playerState.AddDefeatedEnemies();


        livingEnemies.Remove(enemy);
        if (allEnemiesSpawned && livingEnemies.Count == 0) {
            eventController.EndEvent();
        }
    }

    private Transform GetRandomSpawnPoint() {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }



    private void InitialChecks() {
        if (spawnPoints.Count == 0) { Debug.LogError("Encounter has no spawn points.", gameObject); }
        if (scatterPoints.Count == 0) { Debug.LogError("Encounter has no scatter points.", gameObject); }
    }
}

[Serializable]
public struct EnemyBatch {
    public GameObject enemy;
    public int count;
}