using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : Trigger {

    private void Update() {

        Activate();
        Destroy(gameObject);
    }



}
