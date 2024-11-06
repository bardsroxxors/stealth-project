Shader "SS_BuildingMaster_01"
{
    Properties
    {
        [Normal][NoScaleOffset]_Texture2D("Texture2D", 2D) = "bump" {}
        _Colour_X("Colour X", Color) = (1, 0, 0, 0)
        _Colour_Y("Colour Y", Color) = (0, 1, 0, 0)
        _Colour_Z("Colour Z", Color) = (0, 0, 1, 0)
        _Smooth1("Smooth1", Float) = 0
        _Smooth2("Smooth2", Float) = 1
        _Mask_Strength_Z("Mask Strength Z", Float) = 1
        _Mask_Strength_Y("Mask Strength Y", Float) = 1
        _Mask_Strength_X("Mask Strength X", Float) = 15
        _Mask_Min_X("Mask Min X", Float) = 0
        _Mask_Max_X("Mask Max X", Float) = 1
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D("Texture2D", 2D) = "white" {}
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteLitSubTarget"
        }
        Pass
        {
            Name "Sprite Lit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        //Cull Off
        //Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        //ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_SCREENPOSITION
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITELIT
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
             float4 screenPosition;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 ObjectSpacePosition;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float4 screenPosition : INTERP2;
             float3 positionWS : INTERP3;
             float3 normalWS : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.screenPosition.xyzw = input.screenPosition;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.screenPosition = input.screenPosition.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D_TexelSize;
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Smooth1;
        float _Smooth2;
        float _Mask_Strength_Z;
        float _Mask_Min_X;
        float _Mask_Strength_Y;
        float _Mask_Max_X;
        float _Mask_Strength_X;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        SAMPLER(sampler_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_Smoothstep_float3(float3 Edge1, float3 Edge2, float3 In, out float3 Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float(float A, float B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float4 SpriteMask;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Float_63545e2f5be047c9b414de1d23f9a383_Out_0_Float = 128;
            float3 _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpacePosition, (_Float_63545e2f5be047c9b414de1d23f9a383_Out_0_Float.xxx), _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3);
            float _Split_88324feb289f47e2abf97821d55cb4ab_R_1_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[0];
            float _Split_88324feb289f47e2abf97821d55cb4ab_G_2_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[1];
            float _Split_88324feb289f47e2abf97821d55cb4ab_B_3_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[2];
            float _Split_88324feb289f47e2abf97821d55cb4ab_A_4_Float = 0;
            float2 _Vector2_96d00da521444683a070a101fcb3cf73_Out_0_Vector2 = float2(_Split_88324feb289f47e2abf97821d55cb4ab_R_1_Float, _Split_88324feb289f47e2abf97821d55cb4ab_G_2_Float);
            float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).GetTransformedUV(_Vector2_96d00da521444683a070a101fcb3cf73_Out_0_Vector2) );
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_R_4_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.r;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_G_5_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.g;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_B_6_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.b;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_A_7_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.a;
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_R_1_Float = IN.VertexColor[0];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_G_2_Float = IN.VertexColor[1];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_B_3_Float = IN.VertexColor[2];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_A_4_Float = IN.VertexColor[3];
            float _Property_de15eac7d1354d57a4866ee4640e2aa5_Out_0_Float = _Mask_Min_X;
            float _Property_e24198d70bac46b0b1b467f63416e5d2_Out_0_Float = _Mask_Max_X;
            float _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float;
            Unity_Clamp_float(_Split_b6373367e7a44a05bf573c7e0ca41b69_R_1_Float, _Property_de15eac7d1354d57a4866ee4640e2aa5_Out_0_Float, _Property_e24198d70bac46b0b1b467f63416e5d2_Out_0_Float, _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float);
            float _Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float;
            Unity_Lerp_float(1, _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_R_4_Float, _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float, _Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float);
            float4 _Property_1313704ec16c4cc6b079dbcfd488c7d5_Out_0_Vector4 = _Colour_X;
            float4 _Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float.xxxx), _Property_1313704ec16c4cc6b079dbcfd488c7d5_Out_0_Vector4, _Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4);
            float _Property_415ba7edaa924b959b4b24743eea47fb_Out_0_Float = _Smooth1;
            float _Property_2308849ccfce4fada98b571ee8542c85_Out_0_Float = _Smooth2;
            float3 _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3;
            Unity_Absolute_float3(IN.WorldSpaceNormal, _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3);
            float3 _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3;
            Unity_Smoothstep_float3((_Property_415ba7edaa924b959b4b24743eea47fb_Out_0_Float.xxx), (_Property_2308849ccfce4fada98b571ee8542c85_Out_0_Float.xxx), _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3, _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3);
            float _Split_513e8602401d4bdda33d6ecef2aa347d_R_1_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[0];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_G_2_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[1];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_B_3_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[2];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_A_4_Float = 0;
            float _DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_B_3_Float, 1, _DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float);
            float4 _Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4, (_DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float.xxxx), _Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4);
            float4 _Property_5e191635a88749a2b7fbe6c3f73a95cc_Out_0_Vector4 = _Colour_Z;
            float _DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_R_1_Float, 1, _DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float);
            float4 _Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_5e191635a88749a2b7fbe6c3f73a95cc_Out_0_Vector4, (_DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float.xxxx), _Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4);
            float4 _Property_31d23e9460d4481ab2477c0c32d709b2_Out_0_Vector4 = _Colour_Y;
            float _DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_G_2_Float, 1, _DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float);
            float4 _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_31d23e9460d4481ab2477c0c32d709b2_Out_0_Vector4, (_DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float.xxxx), _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4);
            float4 _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4, _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4, _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4);
            float4 _Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4, _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4, _Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4);
            float4 _Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4;
            Unity_Saturate_float4(_Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4, _Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4);
            surface.BaseColor = (_Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4.xyz);
            surface.Alpha = 1;
            surface.SpriteMask = IsGammaSpace() ? float4(1, 1, 1, 1) : float4 (SRGBToLinear(float3(1, 1, 1)), 1);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteLitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Normal"
            Tags
            {
                "LightMode" = "NormalsRendering"
            }
        
        // Render State
        //Cull Off
        //Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        //ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITENORMAL
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 ObjectSpacePosition;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D_TexelSize;
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Smooth1;
        float _Smooth2;
        float _Mask_Strength_Z;
        float _Mask_Min_X;
        float _Mask_Strength_Y;
        float _Mask_Max_X;
        float _Mask_Strength_X;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        SAMPLER(sampler_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_Smoothstep_float3(float3 Edge1, float3 Edge2, float3 In, out float3 Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float(float A, float B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Float_63545e2f5be047c9b414de1d23f9a383_Out_0_Float = 128;
            float3 _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpacePosition, (_Float_63545e2f5be047c9b414de1d23f9a383_Out_0_Float.xxx), _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3);
            float _Split_88324feb289f47e2abf97821d55cb4ab_R_1_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[0];
            float _Split_88324feb289f47e2abf97821d55cb4ab_G_2_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[1];
            float _Split_88324feb289f47e2abf97821d55cb4ab_B_3_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[2];
            float _Split_88324feb289f47e2abf97821d55cb4ab_A_4_Float = 0;
            float2 _Vector2_96d00da521444683a070a101fcb3cf73_Out_0_Vector2 = float2(_Split_88324feb289f47e2abf97821d55cb4ab_R_1_Float, _Split_88324feb289f47e2abf97821d55cb4ab_G_2_Float);
            float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).GetTransformedUV(_Vector2_96d00da521444683a070a101fcb3cf73_Out_0_Vector2) );
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_R_4_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.r;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_G_5_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.g;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_B_6_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.b;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_A_7_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.a;
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_R_1_Float = IN.VertexColor[0];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_G_2_Float = IN.VertexColor[1];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_B_3_Float = IN.VertexColor[2];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_A_4_Float = IN.VertexColor[3];
            float _Property_de15eac7d1354d57a4866ee4640e2aa5_Out_0_Float = _Mask_Min_X;
            float _Property_e24198d70bac46b0b1b467f63416e5d2_Out_0_Float = _Mask_Max_X;
            float _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float;
            Unity_Clamp_float(_Split_b6373367e7a44a05bf573c7e0ca41b69_R_1_Float, _Property_de15eac7d1354d57a4866ee4640e2aa5_Out_0_Float, _Property_e24198d70bac46b0b1b467f63416e5d2_Out_0_Float, _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float);
            float _Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float;
            Unity_Lerp_float(1, _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_R_4_Float, _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float, _Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float);
            float4 _Property_1313704ec16c4cc6b079dbcfd488c7d5_Out_0_Vector4 = _Colour_X;
            float4 _Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float.xxxx), _Property_1313704ec16c4cc6b079dbcfd488c7d5_Out_0_Vector4, _Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4);
            float _Property_415ba7edaa924b959b4b24743eea47fb_Out_0_Float = _Smooth1;
            float _Property_2308849ccfce4fada98b571ee8542c85_Out_0_Float = _Smooth2;
            float3 _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3;
            Unity_Absolute_float3(IN.WorldSpaceNormal, _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3);
            float3 _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3;
            Unity_Smoothstep_float3((_Property_415ba7edaa924b959b4b24743eea47fb_Out_0_Float.xxx), (_Property_2308849ccfce4fada98b571ee8542c85_Out_0_Float.xxx), _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3, _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3);
            float _Split_513e8602401d4bdda33d6ecef2aa347d_R_1_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[0];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_G_2_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[1];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_B_3_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[2];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_A_4_Float = 0;
            float _DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_B_3_Float, 1, _DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float);
            float4 _Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4, (_DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float.xxxx), _Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4);
            float4 _Property_5e191635a88749a2b7fbe6c3f73a95cc_Out_0_Vector4 = _Colour_Z;
            float _DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_R_1_Float, 1, _DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float);
            float4 _Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_5e191635a88749a2b7fbe6c3f73a95cc_Out_0_Vector4, (_DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float.xxxx), _Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4);
            float4 _Property_31d23e9460d4481ab2477c0c32d709b2_Out_0_Vector4 = _Colour_Y;
            float _DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_G_2_Float, 1, _DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float);
            float4 _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_31d23e9460d4481ab2477c0c32d709b2_Out_0_Vector4, (_DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float.xxxx), _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4);
            float4 _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4, _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4, _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4);
            float4 _Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4, _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4, _Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4);
            float4 _Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4;
            Unity_Saturate_float4(_Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4, _Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4);
            surface.BaseColor = (_Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4.xyz);
            surface.Alpha = 1;
            surface.NormalTS = IN.TangentSpaceNormal;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteNormalPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D_TexelSize;
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Smooth1;
        float _Smooth2;
        float _Mask_Strength_Z;
        float _Mask_Min_X;
        float _Mask_Strength_Y;
        float _Mask_Max_X;
        float _Mask_Strength_X;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        SAMPLER(sampler_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        // GraphFunctions: <None>
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            surface.Alpha = 1;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D_TexelSize;
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Smooth1;
        float _Smooth2;
        float _Mask_Strength_Z;
        float _Mask_Min_X;
        float _Mask_Strength_Y;
        float _Mask_Max_X;
        float _Mask_Strength_X;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        SAMPLER(sampler_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        // GraphFunctions: <None>
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            surface.Alpha = 1;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 TangentSpaceNormal;
             float3 ObjectSpacePosition;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D_TexelSize;
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Smooth1;
        float _Smooth2;
        float _Mask_Strength_Z;
        float _Mask_Min_X;
        float _Mask_Strength_Y;
        float _Mask_Max_X;
        float _Mask_Strength_X;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        SAMPLER(sampler_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_Smoothstep_float3(float3 Edge1, float3 Edge2, float3 In, out float3 Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DotProduct_float(float A, float B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Float_63545e2f5be047c9b414de1d23f9a383_Out_0_Float = 128;
            float3 _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3;
            Unity_Multiply_float3_float3(IN.ObjectSpacePosition, (_Float_63545e2f5be047c9b414de1d23f9a383_Out_0_Float.xxx), _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3);
            float _Split_88324feb289f47e2abf97821d55cb4ab_R_1_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[0];
            float _Split_88324feb289f47e2abf97821d55cb4ab_G_2_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[1];
            float _Split_88324feb289f47e2abf97821d55cb4ab_B_3_Float = _Multiply_bec534511d454c6498f26992388a72e9_Out_2_Vector3[2];
            float _Split_88324feb289f47e2abf97821d55cb4ab_A_4_Float = 0;
            float2 _Vector2_96d00da521444683a070a101fcb3cf73_Out_0_Vector2 = float2(_Split_88324feb289f47e2abf97821d55cb4ab_R_1_Float, _Split_88324feb289f47e2abf97821d55cb4ab_G_2_Float);
            float4 _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).samplerstate, UnityBuildTexture2DStructNoScale(_SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_Texture_1_Texture2D).GetTransformedUV(_Vector2_96d00da521444683a070a101fcb3cf73_Out_0_Vector2) );
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_R_4_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.r;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_G_5_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.g;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_B_6_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.b;
            float _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_A_7_Float = _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_RGBA_0_Vector4.a;
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_R_1_Float = IN.VertexColor[0];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_G_2_Float = IN.VertexColor[1];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_B_3_Float = IN.VertexColor[2];
            float _Split_b6373367e7a44a05bf573c7e0ca41b69_A_4_Float = IN.VertexColor[3];
            float _Property_de15eac7d1354d57a4866ee4640e2aa5_Out_0_Float = _Mask_Min_X;
            float _Property_e24198d70bac46b0b1b467f63416e5d2_Out_0_Float = _Mask_Max_X;
            float _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float;
            Unity_Clamp_float(_Split_b6373367e7a44a05bf573c7e0ca41b69_R_1_Float, _Property_de15eac7d1354d57a4866ee4640e2aa5_Out_0_Float, _Property_e24198d70bac46b0b1b467f63416e5d2_Out_0_Float, _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float);
            float _Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float;
            Unity_Lerp_float(1, _SampleTexture2D_cd494f44ee2e4a48aef8d8c7541d0cb6_R_4_Float, _Clamp_5bd7da27886e4b198c0ce89080f7776b_Out_3_Float, _Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float);
            float4 _Property_1313704ec16c4cc6b079dbcfd488c7d5_Out_0_Vector4 = _Colour_X;
            float4 _Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Lerp_06d874a3ba7f468dacd59c3a87b5b905_Out_3_Float.xxxx), _Property_1313704ec16c4cc6b079dbcfd488c7d5_Out_0_Vector4, _Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4);
            float _Property_415ba7edaa924b959b4b24743eea47fb_Out_0_Float = _Smooth1;
            float _Property_2308849ccfce4fada98b571ee8542c85_Out_0_Float = _Smooth2;
            float3 _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3;
            Unity_Absolute_float3(IN.WorldSpaceNormal, _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3);
            float3 _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3;
            Unity_Smoothstep_float3((_Property_415ba7edaa924b959b4b24743eea47fb_Out_0_Float.xxx), (_Property_2308849ccfce4fada98b571ee8542c85_Out_0_Float.xxx), _Absolute_25f61fa45bd347f79e520e5c00bff4e7_Out_1_Vector3, _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3);
            float _Split_513e8602401d4bdda33d6ecef2aa347d_R_1_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[0];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_G_2_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[1];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_B_3_Float = _Smoothstep_d36c9ff976e04d2bbdedd9c113758490_Out_3_Vector3[2];
            float _Split_513e8602401d4bdda33d6ecef2aa347d_A_4_Float = 0;
            float _DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_B_3_Float, 1, _DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float);
            float4 _Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_376756873ad54691874572a0b43b75a1_Out_2_Vector4, (_DotProduct_ebda34d289c94ddbaf287800a0d3c218_Out_2_Float.xxxx), _Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4);
            float4 _Property_5e191635a88749a2b7fbe6c3f73a95cc_Out_0_Vector4 = _Colour_Z;
            float _DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_R_1_Float, 1, _DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float);
            float4 _Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_5e191635a88749a2b7fbe6c3f73a95cc_Out_0_Vector4, (_DotProduct_b2aa380e780f49ecaf3a8afc15396626_Out_2_Float.xxxx), _Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4);
            float4 _Property_31d23e9460d4481ab2477c0c32d709b2_Out_0_Vector4 = _Colour_Y;
            float _DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float;
            Unity_DotProduct_float(_Split_513e8602401d4bdda33d6ecef2aa347d_G_2_Float, 1, _DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float);
            float4 _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_31d23e9460d4481ab2477c0c32d709b2_Out_0_Vector4, (_DotProduct_e33f70be0479446fb90f9a1138d3d297_Out_2_Float.xxxx), _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4);
            float4 _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_fd21b14a37bc4980a7a2e4b76b0dfbd8_Out_2_Vector4, _Multiply_e94213b485ca45cda0437b758de6c6b1_Out_2_Vector4, _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4);
            float4 _Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_f8d0d981ee5246168c415c16f8e3f69f_Out_2_Vector4, _Add_111439db138a4f049d56fd5655eba09e_Out_2_Vector4, _Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4);
            float4 _Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4;
            Unity_Saturate_float4(_Add_7d52388d03df45e584050508f3c3a71e_Out_2_Vector4, _Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4);
            surface.BaseColor = (_Saturate_ab09d63345d6469f8434dc9bac61722b_Out_1_Vector4.xyz);
            surface.Alpha = 1;
            surface.NormalTS = IN.TangentSpaceNormal;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}