Shader "Unlit/MapShower"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RemapTex ("Texture", 2D) = "white" {}
        _PaletteTex ("Texture", 2D) = "white" {}
        _TerrainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _RemapTex;
            sampler2D _PaletteTex;
            sampler2D _TerrainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // return tex2D(_MainTex, i.uv) - tex2D(_MainTex, i.uv + float2(0.001, 0));
                fixed4 c1 = tex2D(_MainTex, i.uv + float2(0.001, 0));
                fixed4 c2 = tex2D(_MainTex, i.uv - float2(0.001, 0));
                fixed4 c3 = tex2D(_MainTex, i.uv + float2(0, 0.001));
                fixed4 c4 = tex2D(_MainTex, i.uv - float2(0, 0.001));

                if(any(c1 != col) || any(c2 != col) || any(c3 != col) || any(c4 != col)){
                    if(col.r < 0.25 || c1.r < 0.25 || c2.r < 0.25 || c3.r < 0.25 || c4.r < 0.25){
                        return fixed4(1,0,0,1);
                    }
                    return fixed4(0,0,0,1);
                }
                fixed4 index = tex2D(_RemapTex, i.uv);
                fixed4 paletteValue = tex2D(_PaletteTex, index.xy * 255.0 / 256.0 + float2(0.001953125, 0.001953125));
                return pow(tex2D(_TerrainTex, i.uv), paletteValue.r);
                // return fixed4(1,1,1,1);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
