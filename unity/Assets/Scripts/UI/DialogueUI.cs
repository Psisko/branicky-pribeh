using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Plays dialogue. Doesn't support audio yet.
/// </summary>
public class DialogueUI : MonoBehaviour {
    [SerializeField] private Image portraitBranice;
    [SerializeField] private Image portraitNPC;
    [SerializeField] private TextMeshProUGUI dialogueTextField;
    [SerializeField] private float letterDelay = 0.05f;
    [SerializeField] private List<TextMeshProUGUI> buttonLabels;
    [SerializeField] private GameObject textContainer;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private GameObject playerData;

    [HideInInspector]
    public UnityEvent<string> dialogueEndEvent;

    private Coroutine textScroll;

    private DialogueSO dialogueData;
    private List<DialogueLine> currentLines;
    // Index of a next line
    private int nextLineIndex;
    private bool isDisplayingDialogue = false;
    private bool isEnding;
    private string output;
    private bool isFirstFrame = false;
    private Animator animator;
    private PlayerController playerController;
    private int selectedChoiceIndex = 0;
    private bool pickingChoice = false;
    private bool monologue;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) Debug.LogError("Player not found", gameObject); 
        playerController = playerGO.GetComponent<PlayerController>();
    }

    private void Update() {

        if (!isDisplayingDialogue) { return; }

        if (!isFirstFrame && Input.GetKeyDown(KeyCode.D)) {
            if (textScroll != null) {
                StopCoroutine(textScroll);
                textScroll = null;
                ShowLine(currentLines[nextLineIndex - 1]);
            } else {
                NextLine();
            }
        }

        if (pickingChoice == true)
        {
            portraitNPC.color = new Color32(100, 100, 100, 255);
            portraitBranice.color = new Color32(100, 100, 100, 255);

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedChoiceIndex = Mathf.Max(0, selectedChoiceIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedChoiceIndex = Mathf.Min(dialogueData.choices.Count - 1, selectedChoiceIndex + 1);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (selectedChoiceIndex < dialogueData.choices.Count)
                {
                    MakeChoice(selectedChoiceIndex);
                }
            }
            SetChoiceLabels();
        }

        isFirstFrame = false;
    }


    /// <summary>
    ///     Setups and starts the dialogue.
    /// </summary>
    /// <returns>False if another dialogue is already happening, true otherwise.</returns>
    public bool StartDialogue(DialogueSO data) {

        if (isDisplayingDialogue) {
            //Debug.LogError("Dialogue box is being used!", gameObject);
            return false;
        }

        isDisplayingDialogue = true;

        SwitchToText();
        animator.SetBool("Activated", true);
        SetupDialogue(data);
        NextLine();

        isFirstFrame = true;

        playerController.SetControlBlocked(true);

        return true;

    }

    private void EndDialogue() {
        animator.SetBool("Activated", false);
        isDisplayingDialogue = false;
        playerController.SetControlBlocked(false);
        playerData.SetActive(true);
        dialogueEndEvent.Invoke(output);
        selectedChoiceIndex = 0;
    }

    private void SwitchToText() {
        textContainer.SetActive(true);
        playerData.SetActive(false);
        buttonContainer.SetActive(false);
    }

    private void SwitchToChoices() {
        pickingChoice = true;
        textContainer.SetActive(false);
        buttonContainer.SetActive(true);
        playerData.SetActive(false);
    }

    // volá ji button Choice v HUDu s daným intem (0 až 5)
    public void MakeChoice(int choice) {

        pickingChoice = false;

        if (choice >= dialogueData.choices.Count) {
            return;
        }

        currentLines = dialogueData.choices[choice].dialogue;
        nextLineIndex = 0;
        isEnding = dialogueData.choices[choice].isEnding;
        if (isEnding) { output = dialogueData.choices[choice].output; }
        SwitchToText();
        NextLine();
    }

    public void NextLine() {

        if (nextLineIndex >= currentLines.Count) {
            if (isEnding) {
                EndDialogue();
            } else {
                SwitchToChoices();  
            }
            return;
        }
        // handles darkening portraits
        if(monologue)
        {
            portraitBranice.color = new Color32(255, 255, 255, 255);
            portraitNPC.color = new Color32(100, 100, 100, 0);
        }
        else
        {
            if (currentLines[nextLineIndex].speakerName == "Branice")
            {
                portraitNPC.color = new Color32(100, 100, 100, 255);
                portraitBranice.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                portraitNPC.color = new Color32(255, 255, 255, 255);
                portraitBranice.color = new Color32(100, 100, 100, 255);
            }
        }



        textScroll = StartCoroutine(PlayTextScroll(currentLines[nextLineIndex]));
        nextLineIndex++;
    }
    private IEnumerator PlayTextScroll(DialogueLine lineData) {

        dialogueTextField.text = lineData.speakerName + ": ";
        foreach (char c in lineData.text) { 
            yield return new WaitForSeconds(letterDelay);
            dialogueTextField.text += c;
        }
        textScroll = null;
    }

    private void ShowLine(DialogueLine lineData) {
        dialogueTextField.text = lineData.speakerName + ": ";
        dialogueTextField.text += lineData.text;
    }

    private void SetupDialogue(DialogueSO data) {

        dialogueData = data;
        currentLines = dialogueData.entryLines;
        portraitNPC.sprite = data.participantPortrait;
        monologue = data.monologue;
        nextLineIndex = 0;
        output = "";

        if (dialogueData.choices.Count == 0) {
            isEnding = true;
        } else {
            isEnding = false;
        }

        SetChoiceLabels();

    }

    // shows this many label choices
    private void SetChoiceLabels() {

        for (int i = 0; i < buttonLabels.Count; i++) {
            if (i < dialogueData.choices.Count)
            {
                buttonLabels[i].color = Color.white;
                if (i == selectedChoiceIndex)
                {
                    // highlighting selected choice
                    buttonLabels[i].text = "-> " + dialogueData.choices[i].choiceText;
                    buttonLabels[i].color = Color.yellow;
                }
                else
                {
                    buttonLabels[i].text = "-> " + dialogueData.choices[i].choiceText;
                }
                
            } 
            else 
            {
                buttonLabels[i].text = "";
            }

        }
    }


}
