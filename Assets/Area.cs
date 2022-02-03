using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaRaw
{
    // Start is called before the first frame update
    //public Color32 remapColor;
    public HashSet<int> neighbors = new HashSet<int>();
    public float x;
    public float y;
    public int count;
    public Vector2 center;
}

[System.Serializable]
public class AreaData
{
    public List<int> neighbors;
    public List<float> center;
    public AreaData(AreaRaw raw)
    {
        neighbors = new List<int>();
        foreach(var nei in raw.neighbors){
            neighbors.Add(nei);
        }
        center = new List<float>();
        center.Add(raw.center.x);
        center.Add(raw.center.y);
    }
}

[System.Serializable]
public class AreaDataContainer
{
    public List<AreaData> areaList;

    public AreaDataContainer(IEnumerable<AreaRaw> areaRawList){
        areaList = new List<AreaData>();

        foreach(var areaRaw in areaRawList){
            areaList.Add(new AreaData(areaRaw));
        }
    }
}

public class Area
{
    public int id;
    public HashSet<int> neighbors;
    public Vector2 center;

    public Area(int id, AreaData data){
        this.id = id;
        neighbors = new HashSet<int>();
        foreach(var nei in neighbors){
            neighbors.Add(nei);
        }
        center = new Vector2(data.center[0], data.center[1]);
    }

    public Color32 ToRemapColor(){
        return new Color32((byte)(id % 256), (byte)(id / 256), 255, 255);
    }
}

public class AreaContainer{
    List<Area> areaList;

    public AreaContainer(AreaDataContainer areaDataContainer){
        var idx = 0;
        areaList = new List<Area>();
        foreach(var areaData in areaDataContainer.areaList){
            areaList.Add(new Area(idx, areaData));
            idx++;
        }
    }
    
    public Area RemapColorToArea(Color32 remapColor){
        var id = (int)remapColor.r + ((int)remapColor.g)*256;
        return areaList[id];
    }
}

