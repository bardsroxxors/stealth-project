Shader "Jonah/TestSurfaceShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        HLSLPROGRAM
        #pragma surface surf Unlit alpha
    }
        Pass
        {
            Name "Sprite Lit"
            Tags
          {
                "LightMode" = "Universal2D"
        }
            struct
              
        ENDHLSL
    }
    FallBack "Diffuse"
}