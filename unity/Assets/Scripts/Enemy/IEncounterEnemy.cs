using UnityEngine;
using UnityEngine.Events;

public interface IEncounterEnemy {
    public void EncounterEnemyInit(EncounterController ec);

    public void AddListenerToOnDeathEvent(UnityAction<Transform> call);
}