using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatuePuzzleController : MonoBehaviour {

    [SerializeField] private List<StatueController> statueControllers;
    public UnityEvent solutionCorrectEvent;

    private void Start() {
        foreach (var controller in statueControllers) {
            controller.changeEvent.AddListener(CheckSolution);
        }
    }

    private void CheckSolution() {
        foreach(var controller in statueControllers) {
            if (!controller.IsChoiceCorrect()) {
                return;
            }
        }

        foreach(var controller in statueControllers) {
            controller.DisableInteraction();
        }

        solutionCorrectEvent.Invoke();
    }

}