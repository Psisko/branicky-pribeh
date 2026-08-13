using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AreaTrigger : Trigger {

    private Transform player;

    protected override void Start()
    {

        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null ) {
            Debug.LogError("Player not found", gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.transform == player) {
            Activate();
        }

    }


}
