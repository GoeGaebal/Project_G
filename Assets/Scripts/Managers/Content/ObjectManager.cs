using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Tilemaps;

public class ObjectManager
{
    public void SpawnGatherings(int count)
    {
        var minX = Managers.Map.CurrentMapInfo.MinX;
        var maxX = Managers.Map.CurrentMapInfo.MaxX;
        var minY = Managers.Map.CurrentMapInfo.MinY;
        var maxY = Managers.Map.CurrentMapInfo.MaxY;
        for (int i = 0; i < count; i++)
        {
            //랜덤 위치 스폰 (일단 겹치더라도 상관없이)
            Vector3Int pos = new Vector3Int()
            {
                x = Random.Range(minX, maxX),
                y = Random.Range(minY, maxY)
            };

            Managers.Resource.Instantiate("Gathering/Stone", pos, Quaternion.identity);
        }
    }

    public void SpawnLootings(int count, Vector3 spawnPos, float radious = 1.0f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randPos = Random.insideUnitCircle * radious;
            randPos.x += spawnPos.x;
            randPos.y += spawnPos.y;
            Managers.Resource.Instantiate("Gathering/Coin", randPos, Quaternion.identity);
        }
    }
}
