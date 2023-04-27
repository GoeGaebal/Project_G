using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : Weapon
{
   public MeleeWeaponController()
   {
        AddTargetLayer((int)EnumLayerMask.Monster);
   }
}
