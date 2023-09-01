using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Leaf : UI_Scene
{

    enum GameObjects {
        Leaf1,
        Leaf2,
        Leaf3
    }

    [SerializeField] Color _colorAvailable;
    [SerializeField] Color _colorUnavailable;
    [SerializeField] float _healPercentage;

    static private int _availableCount = 2;

    [SerializeField] List<GameObject> _imageList;

    
    // Start is called before the first frame update
    void Start()
    {
         Bind<GameObject>(typeof(GameObjects));
        _imageList[0] = Get<GameObject>((int)GameObjects.Leaf1);
        _imageList[1] = Get<GameObject>((int)GameObjects.Leaf2);
        _imageList[2] = Get<GameObject>((int)GameObjects.Leaf3);


        for(int i = 0;i<_availableCount;i++)
        {
            _imageList[i].GetComponent<Image>().color = _colorAvailable;
        }
        for(int i = _availableCount;i<_imageList.Count;i++)
        {
            _imageList[i].GetComponent<Image>().color = _colorUnavailable;
        }

        // foreach(var playerObj in Managers.Network.PlayerList)
        // {
        //     Player playerComponent = playerObj.GetComponent<Player>();
        //     Debug.Log(playerComponent.maxHP * (_healPercentage)/100 * _availableCount + "healed");
        //     playerComponent.RestoreHP(playerComponent.maxHP * (_healPercentage)/100 * _availableCount);
        // }
    }


}
