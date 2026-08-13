using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimatorOutputManager : MonoBehaviour {
    public UnityEvent<PlayerAnimatorOutput> lightAttackEvent;
    public UnityEvent<PlayerAnimatorOutput> lightAttack1Event;
    public UnityEvent<PlayerAnimatorOutput> lightAttack2Event;

    public UnityEvent<PlayerAnimatorOutput> outputEvent;

    public void GiveOutput(PlayerAnimatorOutput output)
    {
        if (output == PlayerAnimatorOutput.lightAttackDamage || output == PlayerAnimatorOutput.lightAttackEnd)
        {
            lightAttackEvent.Invoke(output);
        }
        else if (output == PlayerAnimatorOutput.lightAttackDamage1 || output == PlayerAnimatorOutput.lightAttackEnd1)
        {
            lightAttack1Event.Invoke(output);
        }
        else if (output == PlayerAnimatorOutput.lightAttackDamage2 || output == PlayerAnimatorOutput.lightAttackEnd2)
        {
            lightAttack2Event.Invoke(output);
        }
        else
            outputEvent.Invoke(output); // Fallback for other animations
    }

}

public enum PlayerAnimatorOutput {
    lightAttackDamage,
    lightAttackEnd,
    lightAttackDamage1,
    lightAttackEnd1,
    lightAttackDamage2,
    lightAttackEnd2,
    heavyAttackDamage,
    heavyAttackEnd,
    potionEnd,
}