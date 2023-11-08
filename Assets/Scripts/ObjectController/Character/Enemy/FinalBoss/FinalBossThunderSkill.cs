using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public partial class FinalBossMonster
{
    class ThunderSpell:ICastingSpell
    {
        private FinalBossMonster _finalBossMonster;
        public ThunderSpell(FinalBossMonster fbm)
        {
            _finalBossMonster = fbm;
        }

        public void Spell()
        {
            List<KeyValuePair<int, Player>> playerList =  Managers.Object.PlayerDict.ToList();
            for(int i =0;i<playerList.Count;i++)
            {
                _finalBossMonster.thunders[i].SetActive(true);
                _finalBossMonster.thunders[i].transform.position = new Vector3(playerList[i].Value.transform.position.x, playerList[i].Value.transform.position.y+1.25f,playerList[i].Value.transform.position.z);
            }
            
        }

    }
}
