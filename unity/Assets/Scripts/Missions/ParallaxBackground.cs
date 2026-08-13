using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private ParallaxLayer[] layers;

    private Transform cameraTransform;
    private Vector3 initialCameraPos;

    private void Awake() {
        cameraTransform = Camera.main.transform;
        initialCameraPos = cameraTransform.position;
    }

    private void Start() {
        InitLayers();
    }

    private void FixedUpdate() {
        Vector3 camOffset = cameraTransform.position - initialCameraPos;
        foreach (ParallaxLayer l in layers) {
            l.image.position = l.initialPos + camOffset * l.speed;
        }
    }


    private void InitLayers() {
        foreach (ParallaxLayer l in layers) {
            l.initialPos = l.image.position;
        }
    }



    [Serializable]
    private class ParallaxLayer {
        public Transform image;
        [Range(-1f, 1f)] public float speed = 1;

        [HideInInspector] public Vector3 initialPos;
    }
}
