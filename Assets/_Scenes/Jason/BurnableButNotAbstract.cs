using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BurnableButNotAbstract : Burnable
{
    static bool playerSlashing = false;
    bool extinguished = false;

    public bool destructibleByCombo;
    static string comboString;
    public string comboToBreak;

    public override void Burn()
    {
        Debug.Log(gameObject.name + " have been burn");
    }


    public override void Extinguish()
    {
        Debug.Log(gameObject.name + " has been extinguished");
    }

    public static void PlayerSlash(Vector2 slashDirection)
    {
        playerSlashing = true;
    }

    public static void PlayerSlashDone()
    {
        playerSlashing = false;
    }
    public static void PlayerCombo(string comboString)
    {
        playerSlashing = true;
        BurnableButNotAbstract.comboString = comboString;
    }

    public void OnTriggerStay(Collider other)
    {
        if (!extinguished && comboString == comboToBreak)
        {
            extinguished = true;
            Extinguish();
            BurnableButNotAbstract.comboString = "";
            
        }
            
    }
}
