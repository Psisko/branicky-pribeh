using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LootItem : MonoBehaviour {

    [SerializeField] private Transform sprite;
    [SerializeField, Tooltip("How long the item should travel.")] 
        private float travelTime = 0.5f;
    [SerializeField, Tooltip("How far can the item move after spawning.")] 
        private float spread = 2;
    [SerializeField, Range(0f, 5f), Tooltip("How bouncy the item should be.")]
        private float bounceForce = 1;
    [SerializeField]
    private float pullForce = 10;

    public UnityEvent pickupEvent;

    private float dropTime;
    private Vector2 initialPos;
    private Vector2 destinationPos;
    private float initialSpriteHeight;
    private Transform player;

    private void Start() {
        travelTime *= Random.Range(0.5f, 1.5f);
        bounceForce *= Random.Range(0.5f, 1.5f);
        dropTime = Time.time;
        initialPos = transform.position;
        Vector2 offset = Random.insideUnitCircle * spread;
        offset.y *= 0.5f;
        destinationPos = (Vector2)transform.position + offset;
        initialSpriteHeight = sprite.localPosition.y;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() {

        Vector2 toPlayer = player.position - transform.position;

        if ((Time.time - dropTime) < travelTime * 2) { 

            float percent = Mathf.Clamp((Time.time - dropTime) / travelTime, 0, 1);
            transform.position = Vector2.Lerp(initialPos, destinationPos, percent);

            Vector2 newSpritePos = sprite.localPosition;
            newSpritePos.y = initialSpriteHeight;
            newSpritePos.y += Mathf.Abs(Mathf.Sin(3*Mathf.PI * percent)) * (1-percent) * bounceForce;
            sprite.localPosition = newSpritePos;

        } else {
            if (toPlayer.sqrMagnitude < spread * spread + 1) {
                Vector2 offset = (1 / toPlayer.magnitude) * pullForce * Time.fixedDeltaTime * toPlayer;
                transform.position += (Vector3)offset;
            }
        }

        if (toPlayer.sqrMagnitude < 0.25) {
            PickupItem();
        }
    }

    private void PickupItem() {
        player.GetComponent<PlayerController>().GetAudioManager().PlaySoundEffect("pickup");
        pickupEvent.Invoke();
        Destroy(gameObject);
    }


}
