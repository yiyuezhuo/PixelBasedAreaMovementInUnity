using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mapshower : MonoBehaviour, IGraph<Area>
{
    int width;
    int height;

    Color32[] remapArr;
    Texture2D paletteTex;

    //Color32 prevColor;
    //bool selectAny = false;
    Area prevArea;
    [SerializeField] TextAsset areaJsonData;

    AreaContainer areaContainer;

    public UnityEvent<Area> AreaSelected;

    // Start is called before the first frame update
    void Start()
    {
        var material = GetComponent<Renderer>().material;
        var mainTex = material.GetTexture("_MainTex") as Texture2D;

        width = mainTex.width;
        height = mainTex.height;

        var remapTex = material.GetTexture("_RemapTex") as Texture2D;
        remapArr = remapTex.GetPixels32();

        paletteTex = material.GetTexture("_PaletteTex") as Texture2D;

        var areaDataContainer = JsonUtility.FromJson<AreaDataContainer>(areaJsonData.text);
        areaContainer = new AreaContainer(areaDataContainer);

    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo)){
            var p = (Vector2)hitInfo.point;
            var selectingArea = Pos2Area(p);

            if(selectingArea != prevArea){
                //Debug.Log($"selectingArea={selectingArea}, prevArea={prevArea}");
                if(prevArea != null){
                    ChangeColor(prevArea, new Color32(255, 255, 255, 255));
                }
                prevArea = selectingArea;
                ChangeColor(selectingArea, new Color32(50, 0, 255, 255));
                paletteTex.Apply(false);

                Debug.Log($"AreaSelected Invoke {selectingArea}");
                AreaSelected.Invoke(selectingArea);
            }

            if(Input.GetMouseButtonDown(0)){
                Debug.Log($"mousePos={mousePos} p={p}");
            }
        }
    }

    void ChangeColor(Color32 remapColor, Color32 showColor){
        int xp = remapColor[0];
        int yp = remapColor[1];

        paletteTex.SetPixel(xp, yp, showColor);
    }

    void ChangeColor(Area area, Color32 showColor){
        //Debug.Log($"area.id={area.id}, area.ToRemapColor()={area.ToRemapColor()}");
        ChangeColor(area.ToRemapColor(), showColor);
    }

    public Area Pos2Area(Vector2 p){
        int x = (int)Mathf.Floor(p.x) + width / 2;
        int y = (int)Mathf.Floor(p.y) + height / 2;

        var remapColor = remapArr[x + y * width];
        var area = areaContainer.RemapColorToArea(remapColor);
        return area;
    }

    public float MoveCost(Area src, Area dst){
        return Vector2.Distance(src.center, dst.center);
    }
    // Vector3Int: (x, y, 0)
    // src and dst are expected to be neighbor.

    public float EstimateCost(Area src, Area dst){
        return MoveCost(src, dst);
    }
    // z of src and dst are expected to be 0
    
    public IEnumerable<Area> Neighbors(Area pos){
        foreach(var id in pos.neighbors){
            yield return areaContainer.IdToArea(id);
        }
        
    }

    public Vector3 CenterToWorld(Vector2 p){
        return new Vector3(p.x - width / 2, p.y - height / 2, 0);
    }
}
