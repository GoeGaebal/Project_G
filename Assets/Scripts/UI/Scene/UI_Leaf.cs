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

    [SerializeField] private Color colorAvailable;
    [SerializeField] private Color colorUnavailable;
    [SerializeField] private float healPercentage;

    private static int _availableCount = 2;
    public static int AvailableCount{
        get{return _availableCount;}
        set{
            _availableCount = value;
            if(_availableCount <=0) _availableCount = 0;
        }
    }

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
            _imageList[i].GetComponent<Image>().color = colorAvailable;
        }
        for(int i = _availableCount;i<_imageList.Count;i++)
        {
            _imageList[i].GetComponent<Image>().color = colorUnavailable;
        }
    }

    public void HealPlayers()
    {
        Dictionary<int, Player>.ValueCollection playerComponents = Managers.Object.PlayerDict.Values; 
        foreach(Player playerComponent in playerComponents)
        {
            if(playerComponent == null) continue;
            Debug.Log(playerComponent.maxHP * (healPercentage)/100 * _availableCount + "healed");
            playerComponent.RestoreHP(playerComponent.maxHP * (healPercentage)/100 * _availableCount);
        }
    }
}
