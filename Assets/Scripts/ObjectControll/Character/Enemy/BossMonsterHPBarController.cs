using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossMonsterHPBarController : MonsterHPBarController
{
    [SerializeField] private Image _hpBarBackgroundImage;
    [SerializeField] float _HPBarAlpha;
    private Color _inactiveColor;
    private Color _activeColor;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _inactiveColor = new Color(_hpBarImage.color.r,_hpBarImage.color.g,_hpBarImage.color.b,0);
        _activeColor = new Color(_hpBarImage.color.r,_hpBarImage.color.g,_hpBarImage.color.b,_HPBarAlpha);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(_basicMonster.HasTarget == false)
        {
            _hpBarImage.color = _inactiveColor;
            _hpBarBackgroundImage.color = _inactiveColor;
        }
        else{
            _hpBarImage.color = _activeColor;
            _hpBarBackgroundImage.color = _activeColor;
            base.Update();
        }
    }
}
