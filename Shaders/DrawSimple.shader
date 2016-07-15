// Works only with geometry.
//Shader "Custom/DrawSimple"{
//    SubShader {
//        CGPROGRAM
//        #pragma surface surf Lambert
//        struct Input {
//            float3 worldPos;
//        };
//
//        void surf(Input In, inout SurfaceOutput o) {
//            o.Emission = (1, 1, 1);
//        }
//        ENDCG
//    }
//    Fallback "Diffuse"
//}

// Works with transparent/cutouts textures.
Shader "Custom/DrawSimple"{
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = step(0.1,tex2D(_MainTex, i.texcoord));
                return col;
            }
        ENDCG
        }
    }
}
