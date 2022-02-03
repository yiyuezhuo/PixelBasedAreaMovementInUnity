using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using System.Diagnostics;


[CustomEditor(typeof(Mapshower))]
public class TextureBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        //Debug.Log("OnInspectorGUI");
        if(GUILayout.Button("Build Texture"))
        {
            Debug.Log("build texture");

            var mapshower = (Mapshower)target;
            var obj = mapshower.gameObject;

            var stopWatch = new System.Diagnostics.Stopwatch();

            stopWatch.Restart();

            var material = obj.GetComponent<Renderer>().sharedMaterial;
            var mainTex = material.GetTexture("_MainTex") as Texture2D;
            var mainArr = mainTex.GetPixels32();

            var width = mainTex.width;
            var height = mainTex.height;

            stopWatch.Stop();

            Debug.Log($"width:{width}, height:{height} Get Time:{stopWatch.Elapsed}");            

            stopWatch.Restart();

            var main2remap = new Dictionary<Color32, Color32>();
            var remapArr = new Color32[mainArr.Length];
            int idx = 0;
            for(int i=0; i<mainArr.Length; i++){
                var mainColor = mainArr[i];
                if(!main2remap.ContainsKey(mainColor)){
                    var low = (byte)(idx % 256);
                    var high = (byte)(idx / 256);
                    main2remap[mainColor] = new Color32(low, high, 0, 255);
                    idx++;
                }
                var remapColor = main2remap[mainColor];
                remapArr[i] = remapColor;
            }

            var paletteArr = new Color32[256*256];
            for(int i=0; i<paletteArr.Length; i++){
                paletteArr[i] = new Color32(255, 255, 255, 255);
            }

            var remapTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            remapTex.filterMode = FilterMode.Point;
            remapTex.SetPixels32(remapArr);
            remapTex.Apply(false);
            //material.SetTexture("_RemapTex", remapTex);

            var paletteTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            paletteTex.filterMode = FilterMode.Point;
            paletteTex.SetPixels32(paletteArr);
            paletteTex.Apply(false);
            //material.SetTexture("_PaletteTex", mapshower.paletteTex);

            stopWatch.Stop();
            Debug.Log($"Texture Creation: {stopWatch.Elapsed}");
            stopWatch.Restart();

            // Create Area Container

            var idArr = new int[remapArr.Length];
            for(int i=0; i<remapArr.Length; i++){
                var remapColor = remapArr[i];
                idArr[i] = (int)remapColor.r + ((int)remapColor.g) * 256;
            }

            var areaArr = new AreaRaw[main2remap.Count];
            for(int i=0; i<areaArr.Length; i++){
                areaArr[i] = new AreaRaw();
            }

            //AreaData baseArea;
            for(int i=0; i < idArr.Length; i++){
                var baseId = idArr[i];
                var baseArea = areaArr[baseId];

                var x = i % width;
                var y = i / height;
                
                baseArea.x += x;
                baseArea.y += y;
                baseArea.count += 1;
                
                if(x < width-1){
                    var rightId = idArr[i+1];
                    if(baseId != rightId){
                        var rightArea = areaArr[rightId];
                        baseArea.neighbors.Add(rightId);
                        rightArea.neighbors.Add(baseId);
                    }
                }

                if(y < height - 1){
                    var topId = idArr[i+width];
                    if(baseId != topId){
                        var topArea = areaArr[topId];
                        baseArea.neighbors.Add(topId);
                        topArea.neighbors.Add(baseId);
                    }
                }
            }

            foreach(var area in areaArr){
                area.center = new Vector2(area.x / area.count, area.y / area.count);
            }

            stopWatch.Stop();
            Debug.Log($"Build data: {stopWatch.Elapsed}");
            stopWatch.Restart();

            AssetDatabase.CreateAsset(remapTex, "Assets/GeneratedAssets/remapTex.asset");
            AssetDatabase.CreateAsset(paletteTex, "Assets/GeneratedAssets/paletteTex.asset");

            var areaDataContainer = new AreaDataContainer(areaArr);
            var jsonString = JsonUtility.ToJson(areaDataContainer);
            // Application.dataPath -> Assets
            System.IO.File.WriteAllText(Application.dataPath + "/GeneratedAssets/data.json", jsonString);

            stopWatch.Stop();
            Debug.Log($"Create asset: {stopWatch.Elapsed}");
            //stopWatch.Restart();

            Debug.Log($"jsonString.Length:{jsonString.Length} Content:{jsonString.Substring(0, Mathf.Min(jsonString.Length, 100))}");
            //AssetDatabase.CreateAsset(areaDataMap, "Assets/GeneratedAssets/data.asset");
        }

    }
}
