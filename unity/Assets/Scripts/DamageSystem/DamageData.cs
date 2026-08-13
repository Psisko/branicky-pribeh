using UnityEngine;

public struct DamageData {

    public int damage;
    public bool isParryable;
    public bool isKnockable;
    public Vector3 damageOrigin;

    public DamageData(int damage, bool isParryable, bool isKnockable, Vector3 damageOrigin) {
        this.damage = damage;
        this.isParryable = isParryable;
        this.isKnockable = isKnockable;
        this.damageOrigin = damageOrigin;
    }
}
