using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FitakController : DamageableObject
{

    [SerializeField] private Animator animator;
    [SerializeField] private EventController eventController;

    /// <summary>
    /// Starts event after getting hit
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData)
    {
        animator.SetBool("IsScared", true);
        if (eventController != null)
            eventController.StartEvent();
        GetComponent<CircleCollider2D>().isTrigger = true;
        return new DamageResponseData(isSolid, false);
    }

}
