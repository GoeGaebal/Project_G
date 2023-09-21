using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

public class MonsterHPBarController : MonoBehaviour
{
     protected Image _hpBarImage;
     protected BasicMonster _basicMonster;
    protected virtual void Start()
    {
        _basicMonster = transform.parent.parent.GetComponent<BasicMonster>();
        _hpBarImage = GetComponent<Image>();
        _hpBarImage.fillAmount = 1f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(_basicMonster == null) _basicMonster = transform.parent.parent.GetComponent<BasicMonster>();
        if(_hpBarImage == null) _hpBarImage = GetComponent<Image>();
       _hpBarImage.fillAmount = (_basicMonster.HP/_basicMonster.maxHP);
    }
}
