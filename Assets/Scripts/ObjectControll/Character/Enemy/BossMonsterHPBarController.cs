using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossMonsterHPBarController : MonsterHPBarController
{
    [SerializeField] private Image _hpBarBackgroundImage;
    private float _hpBarBackgroundImageAlpha; 

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _hpBarBackgroundImageAlpha = _hpBarBackgroundImage.color.a;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(_basicMonster.HasTarget == false)
        {
            _hpBarImage.color = new Color(_hpBarImage.color.r,_hpBarImage.color.g,_hpBarImage.color.b,0);
            _hpBarBackgroundImage.color = new Color(_hpBarBackgroundImage.color.r,_hpBarBackgroundImage.color.g,_hpBarBackgroundImage.color.b,0);
        }
        else{
            _hpBarImage.color = new Color(_hpBarImage.color.r,_hpBarImage.color.g,_hpBarImage.color.b,255);
            _hpBarBackgroundImage.color = new Color(_hpBarBackgroundImage.color.r,_hpBarBackgroundImage.color.g,_hpBarBackgroundImage.color.b,_hpBarBackgroundImageAlpha);
            
        }
        base.Update();
    }
}
