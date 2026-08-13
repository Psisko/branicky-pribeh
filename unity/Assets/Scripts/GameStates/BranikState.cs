using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BranikState {

    public static readonly int maxMachineCount = Enum.GetNames(typeof(BranikMachines)).Length;

    public UnityEvent changeEvent = new();

    private List<BranikMachines> ownedMachines = new List<BranikMachines>();



    public bool IsOwned(BranikMachines machine) {
        return ownedMachines.Contains(machine);
    }

    public void MarkAsOwned(BranikMachines machine) {
        if (IsOwned(machine)) {
            Debug.LogError($"Machine {machine} is already owned!");
            return;
        }

        ownedMachines.Add(machine);
        changeEvent.Invoke();
    }

    public bool AllMachinesOwned() {
        return ownedMachines.Count == maxMachineCount;
    }

    public float MachinesOwnedRatio() {
        return (float)ownedMachines.Count / maxMachineCount;
    }
}
