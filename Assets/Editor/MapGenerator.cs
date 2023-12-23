using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.Tilemaps;
#endif

public class MapGenerator
{
#if UNITY_EDITOR

    // % (Ctrl), # (Shift), & (Alt)
    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        GenerateByPath("Assets/Resources/Data/Map");
    }

    private static void GenerateByPath(string pathPrefix)
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameObjects)
        {
            // Tilemap_Base가 더 크기 때문에 기준으로 삼는다
            Tilemap tmBase = Util.FindChild<Tilemap>(go, "Base_Tilemap", true);
            Tilemap tm = Util.FindChild<Tilemap>(go, "Collision_Tilemap", true);
            if (tmBase == null || tm == null)
            {
                Debug.Log($"{go.name}은 Base_Tilemap 혹은 Collision_Tilemap을 가지고 있지 않습니다.");
                continue;
            }
            StringBuilder sb = new();

            //json파일 생성
            using (var writer = File.CreateText($"{pathPrefix}/{go.name}.json"))
            {
                sb.Clear();
                sb.Append("{\n  \"mapdatas\": [\n");
                for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
                {
                    for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
                    {
                        sb.Append("    {\n");
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        sb.Append($"      \"key\" : \"{((ulong)(uint)y << Define.INT_SIZE_IN_BITS) | (uint)x}\",\n");
                        sb.Append(tile != null ? "     \"isCollision\" : \"1\"\n" : "     \"isCollision\" : \"0\"\n");
                        sb.Append("    },\n");
                    }
                }

                sb.Remove(sb.Length - 2, 1);
                sb.Append("  ]\n}");
                writer.WriteLine(sb.ToString());
            }
            Debug.Log($"{go.name} 변환 완료!");
        }
    }
#endif
}