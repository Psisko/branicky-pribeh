using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionsState {

    [SerializeField] private List<Mission> missions;

    private string mostRecentMission = "";

    /// <summary>
    ///     Checks if mission is completed.
    /// </summary>
    /// <returns>True if competed, false otherwise</returns>
    public bool IsCompleted(string missionName) {
        return GetMission(missionName).isCompleted;
    }

    /// <summary>
    ///     Checks if mission is available. Thah means the mission is not yet completed and
    ///     all required missions are completed.
    /// </summary>
    /// <returns>True if completed, false otherwise.</returns>
    public bool IsAvailable(string missionName) {
        Mission mission = GetMission(missionName);
        if (mission.isCompleted) {
            return false;
        }

        foreach (string requiredMissionName in mission.requirements) {
            if (!IsCompleted(requiredMissionName)) {
                return false;
            }
        }
        return true;
    }

    /// <returns>List of all currently available missions.</returns>
    public List<string> GetAllAvailable() {
        List<string> available = new();
        foreach (Mission mission in missions) {
            if (IsAvailable(mission.missionName)) {
                available.Add(mission.missionName);
            }
        }

        return available;
    }

    /// <summary>
    ///     Marks mission as completed.
    /// </summary>
    /// <param name="missionName">Mission to be marked.</param>
    public void MarkCompleted(string missionName) {
        GetMission(missionName).isCompleted = true;
        mostRecentMission = missionName;
    }

    /// <summary>
    ///     Returns name of the first scene of the mission.
    /// </summary>
    /// <returns>Scene name.</returns>
    public string GetMissionSceneName(string missionName) {
        return GetMission(missionName).sceneName;
    }

    /// <returns>Name of most recently compleetd mission.</returns>
    public string GetMostRecentMission() {
        return mostRecentMission;
    }

    private Mission GetMission(string missionName) {
        Mission mission = missions.Find(x => x.missionName == missionName);
        if (mission == null) {
            Debug.LogError($"There is no mission named \"{missionName}\"!");
        }
        return mission;
    }






    public string MissionsToString() {
        string str = "";
        foreach (Mission mission in missions) {
            str += $"Mission {mission.missionName}, completed = {mission.isCompleted}\n";
        }
        return str;
    }

}

[Serializable]
public class Mission {
    public string missionName;
    public List<string> requirements;
    [HideInInspector] 
        public bool isCompleted = false;
    public string sceneName;
}
