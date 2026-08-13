using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : DamageableObject {

    [SerializeField] private int lives;
    [SerializeField] private Sprite destroyedSprite;

    private bool isDestroyed = false;


    public override DamageResponseData RecieveDamage(DamageData damageData) {
        if (isDestroyed) return new DamageResponseData(false, false);

        lives -= damageData.damage;
        if (lives <= 0) {
            GetComponent<SpriteRenderer>().sprite = destroyedSprite;
            isDestroyed = true;
        }

        return new DamageResponseData(isSolid, false);
    }
}
