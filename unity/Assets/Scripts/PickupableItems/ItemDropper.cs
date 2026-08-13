using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDropper : MonoBehaviour {

    [SerializeField] private List<DropTableItem> dropTable;

    public void Drop() {
        foreach (DropTableItem item in dropTable) {
            float randRoll = Random.Range(0.0f, 1.0f);
            if (randRoll <= item.probability) {
                DropItem(item);
            }
        }

    }

    private void DropItem(DropTableItem item) {
        int count = Random.Range(item.minCount, item.maxCount + 1);
        for (int i = 0; i < count; i++) {
            Instantiate(item.itemPf, transform.position, Quaternion.identity);
        }

    }
}
