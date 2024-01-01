using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BossMonster
{
    class ThunderSpell: ICastingSpell
    {
        BossMonster _bossMonster;
        public ThunderSpell(BossMonster bm)
        {
            _bossMonster = bm;
        }
        public void Spell()
        {
            List<Transform> tfs = _bossMonster.Thunders;
            foreach(Transform tf in tfs)
            {
                tf.gameObject.SetActive(true);
            }
            tfs[0].localPosition = new Vector2(0f,0.5f);
            tfs[1].localPosition = new Vector2(0.5f,0f);
            tfs[2].localPosition = new Vector2(0f,-0.5f);
            tfs[3].localPosition = new Vector2(-0.5f,0f);

            foreach(Transform tf in tfs)
            {
                tf.SetParent(null);
            }
            
        }
    }

    class TeleportTargetSpell:ICastingSpell
    {
        BossMonster _bossMonster;
        public TeleportTargetSpell(BossMonster bm)
        {
            _bossMonster = bm;
        }
        public void Spell()
        {
            if (_bossMonster.Target == null)
            {
                return;
            }
            _bossMonster.Target.transform.position = (Vector2)_bossMonster.transform.position + Vector2.right * 0.5f;
        }
    }
}
