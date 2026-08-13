using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DamageableObject : MonoBehaviour {

    [Header("Damageable Object")]
    [SerializeField] private List<Team> teams;
    [SerializeField, Tooltip("If true it should e.g. destroy projectiles and play hit sounds. " +
        "Set false mainly for destructible grass and other soft destructible objects.")] 
        public bool isSolid = true;

    public abstract DamageResponseData RecieveDamage(DamageData damageData);

    /// <summary>
    ///     Checks if this object is part of given team.
    /// </summary>
    /// <returns> True if it is a part of that team, false otherwise. </returns>
    public bool IsTeam(Team testedTeam) {
        return teams.Contains(testedTeam);
    }

    /// <summary>
    ///     Checks if this object is part of any of the given teams.
    /// </summary>
    /// <returns> True if it is a part of any team, false otherwise. </returns>
    public bool IsTeam(List<Team> testedTeams) {
        foreach (Team testedTeam in testedTeams) {
            if (IsTeam(testedTeam)) {
                return true;
            }
        }
        return false;
    }
}