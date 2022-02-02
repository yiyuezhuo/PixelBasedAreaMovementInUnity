using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mapshower))]
public class TextureBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        //Debug.Log("OnInspectorGUI");
        if(GUILayout.Button("Build Texture"))
        {
            Debug.Log("build texture");

            var mapshower = (Mapshower)target;
            var obj = mapshower.gameObject;

            var material = obj.GetComponent<Renderer>().sharedMaterial;
            var mainTex = material.GetTexture("_MainTex") as Texture2D;
            var mainArr = mainTex.GetPixels32();

            var width = mainTex.width;
            var height = mainTex.height;

            Debug.Log($"width:{width}, height:{height}");            

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
                mapshower.remapArr[i] = remapColor;
            }

            var paletteArr = new Color32[256*256];
            for(int i=0; i<paletteArr.Length; i++){
                paletteArr[i] = new Color32(255, 255, 255, 255);
            }

            var remapTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            remapTex.filterMode = FilterMode.Point;
            remapTex.SetPixels32(mapshower.remapArr);
            remapTex.Apply(false);
            //material.SetTexture("_RemapTex", remapTex);

            var paletteTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            paletteTex.filterMode = FilterMode.Point;
            paletteTex.SetPixels32(paletteArr);
            paletteTex.Apply(false);
            //material.SetTexture("_PaletteTex", mapshower.paletteTex);

            AssetDatabase.CreateAsset(remapTex, "Assets/GeneratedAssets/remapTex.asset");
            AssetDatabase.CreateAsset(paletteTex, "Assets/GeneratedAssets/paletteTex.asset");
            
        }
        /*
        var material = GetComponent<Renderer>().material;
        var mainTex = material.GetTexture("_MainTex") as Texture2D;
        var mainArr = mainTex.GetPixels32();

        width = mainTex.width;
        height = mainTex.height;

        var main2remap = new Dictionary<Color32, Color32>();
        remapArr = new Color32[mainArr.Length];
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
        material.SetTexture("_RemapTex", remapTex);

        paletteTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        paletteTex.filterMode = FilterMode.Point;
        paletteTex.SetPixels32(paletteArr);
        paletteTex.Apply(false);
        material.SetTexture("_PaletteTex", paletteTex);
        */
    }
}
