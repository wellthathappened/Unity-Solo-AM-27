using UnityEngine;

public class Rifle : Weapon
{
    [Header("Rifle Stats")]
    public float SemiAutoROF = 1;
    public float FullAutoROF = .1f;

    public void changeFireMode()
    {
        if(fireModes >= 2)
        {
            currentFiremode++;

            if(currentFiremode >= fireModes)
            {
                currentFiremode = 0;
            }

            // Semi-Auto
            if(currentFiremode == 0)
            {
                holdToAttack = false;

                rof = SemiAutoROF;
            }

            // Full Auto
            else if(currentFiremode == 1)
            {
                holdToAttack = true;

                rof = FullAutoROF;
            }
        }
    }
}
