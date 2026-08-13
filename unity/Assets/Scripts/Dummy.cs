using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Trigger trigger;

    public void Bar() {
        Debug.Log("Dummy says \"Yahooo!\"", gameObject);
    }

}
