Shader "Unlit/StilyzedFlame"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0.5, 0.1, 1)
        _Waviness ("Waviness", Float) = 0.05
        _WavingSpeed ("Animation Speed", Float) = 10.0
        _NoiseScale ("Noise Scale", Float) = 10.0
        [Toggle] _Inverted ("Inverted", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front

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

            fixed4 _Color;
            fixed4 _Color_ST;

            fixed _Waviness;
            fixed _WavingSpeed;
            fixed _NoiseScale;
            fixed _Inverted;

            float noise(float4 position) {
                float4 world_pos = mul(unity_ObjectToWorld, position);
                return
                    sin(world_pos.x * 2.0 * _NoiseScale + _Time * 2.0 * _WavingSpeed) / 2.0 + 
                    cos(world_pos.y * 3.0 * _NoiseScale + _Time * _WavingSpeed) / 2.0;
            }

            v2f vert (appdata v)
            {
                v2f o;
                fixed strength = max(0, v.vertex.y) * _Waviness * abs(noise(v.vertex));
                fixed4 shift = fixed4(0, strength, 0, 1);
                o.vertex = UnityObjectToClipPos(v.vertex + shift);
                o.uv = TRANSFORM_TEX(v.uv, _Color);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
