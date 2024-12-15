Shader "Unlit/SelectionBox"
{
    Properties
    {
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _Top ("Top Level", Range(0, 1)) = 1.0
        _Scale ("Scale", Float) = 1.0
        [Toggle] _useWorldCoords("Use World Coordinates", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Enable transparency
            ZWrite Off // Disable depth writing
            Cull Off

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
                float4 local_pos : TEXCOORD1;
                float4 world_pos : TEXCOORD2;
            };

            float4 _Color;
            float _Transparency;
            float _Top;
            float _Scale;
            float _useWorldCoords;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.local_pos = v.vertex * 100.0;
                float4 local_pos = v.vertex;
                local_pos.w = _useWorldCoords;
                o.world_pos = mul(unity_ObjectToWorld, local_pos);
                o.world_pos.w = 1.0;
                return o;
            }

            float waves(float level, float local_height, float4 world_pos, float anim_direction, int iteration) {
                float true_level = level - 0.3 * (
                        sin(world_pos.x * (1.3 + iteration * 0.8) * _Scale - _Time * 60.0 * anim_direction) +
                        cos(world_pos.z * (1.3 + iteration * 0.8) * _Scale - _Time * 60.0 * anim_direction) + 2.0
                    ) / 4.0;
                return true_level < local_height ? 0.0 : 1.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col.a *= 
                waves(_Top * 3 / 3, i.uv.y, i.world_pos,  1.0, 0) * 1 +
                waves(_Top * 2 / 3, i.uv.y, i.world_pos, -1.0, 1) * 2 + 
                waves(_Top * 1 / 3, i.uv.y, i.world_pos,  1.0, 1) * 3; 
                col.a *= _Transparency / 6;
                return col;
            }
            ENDCG
        }
    }
}
