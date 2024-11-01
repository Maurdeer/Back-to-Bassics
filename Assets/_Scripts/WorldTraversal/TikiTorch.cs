using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TikiTorch : Burnable
{

    [SerializeField] private GameObject fire;
    private TorchPuzzleManager puzzleManager;
    private bool isBurning = false;

    public override void Burn()
    {
        if (isBurning) return;
        fire.SetActive(true);   
        isBurning = true;
        puzzleManager?.CheckAllBurning();
    }

    public override void Extinguish()
    {
        if (!isBurning) return;
        fire.SetActive(false);
        isBurning = false;
        puzzleManager?.CheckAllBurning();
    }

    public bool getBurning() { return isBurning; }

    public void setPuzzleManager(TorchPuzzleManager puzzleManager) { this.puzzleManager = puzzleManager; }
}
