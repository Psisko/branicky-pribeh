using System.Collections;
using UnityEngine;

public abstract class PlayerAction : MonoBehaviour {
    public bool IsActive { get; protected set; } = false;

    private Coroutine coroutine;

    /// <summary>
    ///     Starts a action
    /// </summary>
    public abstract void StartAction();

    /// <summary>
    ///     Interrupts this action and returns everything to default state.
    /// </summary>
    public abstract void InterruptAction();

        
}
