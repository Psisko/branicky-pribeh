using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A part of a conversation, that the player can choose to play out
/// </summary>
[Serializable]
public struct DialogueChoice {
    [Tooltip("A text, that will appear in the choice menu.")]
    public string choiceText;
    [Tooltip("If true, this choice plays a dialogue and ends the conversation. " +
        "If false, this choice plays a dialogue and returns to choices.")]
    public bool isEnding;
    [Tooltip("If this choice is ending (isEneding is true), " +
        "then this string will be returned as a outcome of this conversation.")]
    public string output;
    public List<DialogueLine> dialogue;
}