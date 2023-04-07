using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectManager
{
    public void GenerateGathering(int mapId)
    {
        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Resources.Load<GameObject>($"Prefabs/Map/{mapName}");
        if (go == null)
        {
            Debug.Log("Map을 불러오지 못했습니다.");
            return;
        }

        Tilemap tmBase = Util.FindChild<Tilemap>(go, "Grass_Tilemap", true);

        var cellBounds = tmBase.cellBounds;
        int xMin = cellBounds.xMin;
        int xMax = cellBounds.xMax;
        int yMin = cellBounds.yMin;
        int yMax = cellBounds.yMax;

        for (int i = 0; i < 5; i++)
        {
            //랜덤 위치 스폰 (일단 겹치더라도 상관없이)
            Vector3Int pos = new Vector3Int()
            {
                x = Random.Range(xMin, xMax),
                y = Random.Range(yMin, yMax)
            };

            Managers.Resource.Instantiate("Gathering/Bush", pos, Quaternion.identity);
        }
    }
}
