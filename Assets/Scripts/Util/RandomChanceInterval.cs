using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class used for cryptid behaviors that have an x chance of happening every y seconds
public class RandomChanceInterval
{
    private float timer; //keep track of time passing
    private float intervalInSeconds; //how often should we roll the dice on this random chance
    private float chanceToSucceed; //value between 0 and 1 representing a percent chance

    public RandomChanceInterval(float interval, float chance)
    {
        intervalInSeconds = interval;
        chanceToSucceed = chance;
    }

    public void SetChance(float chance)
    {
        chanceToSucceed = chance;
    }

    public void SetInterval(float interval)
    {
        intervalInSeconds = interval;
    }

    public bool UpdateTimerAndCheckSuccess()
    {
        timer += Time.deltaTime;
        if (timer >= intervalInSeconds)
        {
            timer = 0;
            float r = Random.Range(0.0f, 1.0f);
            return r <= chanceToSucceed;
        }

        return false;
    }

    public void ResetTimer()
    {
        timer = 0;
    }



}
