using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon
{
    protected int targetLayerMask = 0;
    
    public bool CheckAttackLayer(int attackLayer)
    {
        return ((targetLayerMask & (1<<attackLayer))>0);
    }
    
    protected void AddTargetLayer(int layerNum)
    {
        targetLayerMask |= (1<<layerNum);
    }
    private void RemoveTargetLayer(int layerNum)
    {
        targetLayerMask &=  ~(1<<layerNum);
    }

    private void ResetTargetLayer()
    {
        targetLayerMask = 0;
    }


}
