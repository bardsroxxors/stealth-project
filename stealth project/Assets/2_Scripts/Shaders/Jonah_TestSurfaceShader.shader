Shader "Jonah/TestSurfaceShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            texture2D _MainTex;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragmentOutput {
                float4 color : SV_Target;
            };

            struct VertexOutput {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            FragmentOutput frag (VertexOutput i) {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                FragmentOutput o;
                o.color = col;
                return o;
            }

            ENDHLSL
        }
    }
}