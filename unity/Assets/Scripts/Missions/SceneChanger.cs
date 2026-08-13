using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Trigger trigger;
    [SerializeField] private string sceneName;

    private void Start() {
        trigger.activationEvent.AddListener(ChangeScene);
    }

    private void ChangeScene() {
        SceneManager.LoadScene(sceneName);
    }
}
