Shader "Unlit/GradientSkyShader"
{
    Properties
    {
        _TopZenithColor ("Top Zenith Color",    Color) = (0.5, 0.8, 0.9)
        _BtmZenithColor ("Bottom Zenith Color", Color) = (0.4, 0.7, 0.8)
        _HorizonWidth ("Horizon Width", float) = 1.0
        _HorizonLevel ("Horizon Level", float) = 0.0
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
                float4 vertex : SV_POSITION;
            };

            fixed4 _MainTex_ST;
            fixed3 _TopZenithColor;
            fixed3 _BtmZenithColor;
            fixed _HorizonWidth;
            fixed _HorizonLevel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed horizonGradient(fixed level) {
                float normalized_level = (level - _HorizonLevel) / _HorizonWidth + 0.5;
                return smoothstep(0.0, 1.0, normalized_level);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed alpha = horizonGradient(i.uv.y);
                fixed3 col = _TopZenithColor * alpha + _BtmZenithColor * (1.0 - alpha);
                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}
