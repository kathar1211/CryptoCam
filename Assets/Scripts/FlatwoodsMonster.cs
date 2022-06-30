using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatwoodsMonster : Cryptid
{
    // Start is called before the first frame update
    void Start()
    {
        baseScore = 175;
        cryptidType = Constants.Flatwoods;
        StartUp();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (lockMovementSuper) { return; }
    }
}
