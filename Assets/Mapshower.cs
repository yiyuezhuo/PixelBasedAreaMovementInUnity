using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapshower : MonoBehaviour
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
            var p = hitInfo.point;
            int x = (int)Mathf.Floor(p.x) + width / 2;
            int y = (int)Mathf.Floor(p.y) + height / 2;

            var remapColor = remapArr[x + y * width];
            var selectingArea = areaContainer.RemapColorToArea(remapColor);

            if(selectingArea != prevArea){
                //Debug.Log($"selectingArea={selectingArea}, prevArea={prevArea}");
                if(prevArea != null){
                    ChangeColor(prevArea, new Color32(255, 255, 255, 255));
                }
                prevArea = selectingArea;
                ChangeColor(selectingArea, new Color32(50, 0, 255, 255));
                paletteTex.Apply(false);
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
}
