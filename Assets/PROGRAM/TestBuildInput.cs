using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuildManager.Instance.SelectTowerToBuild(0); // Fire
            Debug.Log("[Test] Pilih tower index 0 (Fire)");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BuildManager.Instance.SelectTowerToBuild(1); // Water
            Debug.Log("[Test] Pilih tower index 1 (Water)");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BuildManager.Instance.SelectTowerToBuild(2); // Wind
            Debug.Log("[Test] Pilih tower index 2 (Wind)");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            BuildManager.Instance.SelectTowerToBuild(3); // Earth
            Debug.Log("[Test] Pilih tower index 3 (Earth)");
        }
    }
}
