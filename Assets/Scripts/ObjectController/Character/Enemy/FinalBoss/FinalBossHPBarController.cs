using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FinalBossHPBarController : MonoBehaviour
{
    protected Image _hpBarImage;
    protected FinalBossMonster _finalBossMonster;
    protected virtual void Start()
    {

     
        _finalBossMonster = transform.root.GetComponent<FinalBossMonster>();
        _hpBarImage = GetComponent<Image>();
        _hpBarImage.fillAmount = 1f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
       _hpBarImage.fillAmount = (_finalBossMonster.HP/_finalBossMonster.maxHP);
    }
}
