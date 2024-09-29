using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BurnableButNotAbstract : Burnable
{
    public override void Burn()
    {
        Debug.Log(gameObject.name + " have been burn");
    }
}
