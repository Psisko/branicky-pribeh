using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EventController))]
public class ChangeSceneEvent : MonoBehaviour
{

    [SerializeField] private string sceneName;

    private void Start() {
        GetComponent<EventController>().eventStart.AddListener(ChangeScene);
    }

    private void ChangeScene() {
        SceneManager.LoadScene(sceneName);
    }
}
