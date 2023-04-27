using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxWeaponController : Weapon
{
   public PickaxWeaponController()
   {
        AddTargetLayer((int)EnumLayerMask.Mineral);
   }
}
