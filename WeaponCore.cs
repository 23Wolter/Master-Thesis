using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCore : MonoBehaviour
{
    [SerializeField] Image weaponCoreProtonUI; 
    [SerializeField] Image weaponCoreNeutronUI;
    [SerializeField] Sprite[] protonSprites;
    [SerializeField] Sprite[] neutronSprites; 


    public void SetProtonUI(int index)
    {
        if (index > protonSprites.Length+1 || index <= 0) return;
        weaponCoreProtonUI.color = new Color32(255, 255, 255, 255);
        weaponCoreProtonUI.sprite = protonSprites[index-1]; 
    }

    public void SetNeutronUI(int index)
    {
        if (index > neutronSprites.Length+1 || index <= 0) return;
        weaponCoreNeutronUI.color = new Color32(255, 255, 255, 255);
        weaponCoreNeutronUI.sprite = neutronSprites[index-1]; 
    }
}
