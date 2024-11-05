Shader "SS_BuildingMaster_01"
{
    Properties
    {
        [Normal][NoScaleOffset]_Texture2D("Texture2D", 2D) = "bump" {}
        _Mask_Bleed("Mask Bleed", Float) = 8
        _Colour_X("Colour X", Color) = (1, 0, 0, 0)
        _Colour_Y("Colour Y", Color) = (0, 1, 0, 0)
        _Colour_Z("Colour Z", Color) = (0, 0, 1, 0)
        [NoScaleOffset]_Texture_X("Texture X", 2D) = "white" {}
        _Texture_Tiling_X("Texture Tiling X", Vector) = (1, 1, 0, 0)
        _Multiply_X("Multiply X", Float) = 190.1
        _Max_X("Max X", Float) = 5
        [ToggleUI]_World_Aligned_X("World Aligned X?", Float) = 0
        [NoScaleOffset]_Texture_Y("Texture Y", 2D) = "white" {}
        _Texture_Tiling_Y("Texture Tiling Y", Vector) = (100, 100, 0, 0)
        _Multiply_Y("Multiply Y", Float) = 100
        _Max_Y("Max Y", Float) = 5
        [NoScaleOffset]_Texture_Z("Texture Z", 2D) = "white" {}
        _Texture_Tiling_Z("Texture Tiling Z", Vector) = (100, 100, 0, 0)
        _Multiply_Z("Multiply Z", Float) = 100
        _Max_Z("Max Z", Float) = 5
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
        #define VARYINGS_NEED_TANGENT_WS
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
             float4 tangentWS;
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
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 AbsoluteWorldSpacePosition;
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
             float4 texCoord0 : INTERP1;
             float4 color : INTERP2;
             float4 screenPosition : INTERP3;
             float3 positionWS : INTERP4;
             float3 normalWS : INTERP5;
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
            output.tangentWS = input.tangentWS.xyzw;
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
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Mask_Bleed;
        float2 _Texture_Tiling_X;
        float _Multiply_X;
        float4 _Texture_X_TexelSize;
        float _Max_X;
        float _World_Aligned_X;
        float4 _Texture_Y_TexelSize;
        float _Multiply_Y;
        float _Max_Y;
        float2 _Texture_Tiling_Y;
        float4 _Texture_Z_TexelSize;
        float2 _Texture_Tiling_Z;
        float _Multiply_Z;
        float _Max_Z;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        TEXTURE2D(_Texture_X);
        SAMPLER(sampler_Texture_X);
        TEXTURE2D(_Texture_Y);
        SAMPLER(sampler_Texture_Y);
        TEXTURE2D(_Texture_Z);
        SAMPLER(sampler_Texture_Z);
        
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
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float4(float4 In, float4 Min, float4 Max, out float4 Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_Power_float3(float3 A, float3 B, out float3 Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_ChannelMask_GreenBlue_float3 (float3 In, out float3 Out)
        {
        Out = float3(0, In.g, In.b);
        }
        
        void Unity_Contrast_float(float3 In, float Contrast, out float3 Out)
        {
            float midpoint = pow(0.5, 2.2);
            Out =  (In - midpoint) * Contrast + midpoint;
        }
        
        void Unity_ChannelMask_RedBlue_float3 (float3 In, out float3 Out)
        {
        Out = float3(In.r, 0, In.b);
        }
        
        void Unity_ChannelMask_RedGreen_float3 (float3 In, out float3 Out)
        {
        Out = float3(In.r, In.g, 0);
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float
        {
        float3 WorldSpaceNormal;
        float3 WorldSpaceTangent;
        float3 WorldSpaceBiTangent;
        float3 WorldSpacePosition;
        float3 AbsoluteWorldSpacePosition;
        };
        
        void SG_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float(Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float IN, out float2 Alpha_1)
        {
        float3 _Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3;
        {
        // Converting Position from AbsoluteWorld to Object via world space
        float3 world;
        world = GetCameraRelativePositionWS(IN.AbsoluteWorldSpacePosition.xyz);
        _Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3 = TransformWorldToObject(world);
        }
        float _Float_d8d5ee1b4a9342e0acdb554a6fb02662_Out_0_Float = 1024;
        float3 _Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3;
        Unity_Divide_float3(_Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3, (_Float_d8d5ee1b4a9342e0acdb554a6fb02662_Out_0_Float.xxx), _Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3);
        float3 _ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3;
        Unity_ChannelMask_GreenBlue_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3);
        float3 _Vector3_2050ca69d4a947c6af34ad61848f07c4_Out_0_Vector3 = float3(0, 0, 1);
        float3 _Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3;
        {
        // Converting Position from Tangent to Object via world space
        float3 world;
        {
        float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
        world = TransformTangentToWorldDir(_Vector3_2050ca69d4a947c6af34ad61848f07c4_Out_0_Vector3.xyz, tangentTransform, false).xyz + IN.WorldSpacePosition;
        }
        _Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3 = TransformWorldToObject(world);
        }
        float3 _Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3;
        Unity_Absolute_float3(_Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3, _Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3);
        float _Float_081ee3c0116543d09db94af274033691_Out_0_Float = 160;
        float3 _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3;
        Unity_Power_float3(_Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3, (_Float_081ee3c0116543d09db94af274033691_Out_0_Float.xxx), _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3);
        float3 _Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3;
        Unity_Divide_float3(_Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3, _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3, _Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3);
        float3 _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3;
        Unity_Contrast_float(_Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3, 512, _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3);
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_R_1_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[0];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_G_2_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[1];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_B_3_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[2];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_A_4_Float = 0;
        float3 _Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_B_3_Float.xxx), _Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3);
        float3 _ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3;
        Unity_ChannelMask_RedBlue_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3);
        float3 _Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_G_2_Float.xxx), _Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3);
        float3 _ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3;
        Unity_ChannelMask_RedGreen_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3);
        float3 _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_R_1_Float.xxx), _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3);
        float3 _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3;
        Unity_Add_float3(_Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3, _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3, _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3);
        float3 _Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3;
        Unity_Add_float3(_Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3, _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3, _Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3);
        Alpha_1 = (_Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3.xy);
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
            UnityTexture2D _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_X);
            float _Property_77eb78a1223b4d979751f37999821590_Out_0_Boolean = _World_Aligned_X;
            float3 _Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3;
            Unity_Branch_float3(_Property_77eb78a1223b4d979751f37999821590_Out_0_Boolean, IN.WorldSpacePosition, IN.ObjectSpacePosition, _Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3);
            float2 _Property_541d4f6f903f4acfa089dc1dafd910a1_Out_0_Vector2 = _Texture_Tiling_X;
            float2 _TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2;
            Unity_TilingAndOffset_float((_Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3.xy), _Property_541d4f6f903f4acfa089dc1dafd910a1_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2);
            float4 _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.tex, _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.samplerstate, _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2) );
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_R_4_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.r;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_G_5_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.g;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_B_6_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.b;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_A_7_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.a;
            float _Property_5f265aae98de4403bb5c9208511ff569_Out_0_Float = _Multiply_X;
            float _Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float;
            Unity_Multiply_float_float(_SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_R_4_Float, _Property_5f265aae98de4403bb5c9208511ff569_Out_0_Float, _Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float);
            float _Float_333f00e6bfaf4c47ad3d1dbb4135d894_Out_0_Float = 0;
            float _Property_36c5a516db034c0a8d0372b436599be8_Out_0_Float = _Max_X;
            float _Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float;
            Unity_Clamp_float(_Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float, _Float_333f00e6bfaf4c47ad3d1dbb4135d894_Out_0_Float, _Property_36c5a516db034c0a8d0372b436599be8_Out_0_Float, _Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float);
            float4 _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4 = _Colour_X;
            float4 _Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float.xxxx), _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4, _Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4);
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_R_1_Float = IN.VertexColor[0];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_G_2_Float = IN.VertexColor[1];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_B_3_Float = IN.VertexColor[2];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4, _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4, (_Split_77f9fff0abdc40e589d1ea67f577cb5d_R_1_Float.xxxx), _Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4);
            UnityTexture2D _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_Y);
            float3 _Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3;
            {
                // Converting Position from AbsoluteWorld to Object via world space
                float3 world;
                world = GetCameraRelativePositionWS(IN.AbsoluteWorldSpacePosition.xyz);
                _Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3 = TransformWorldToObject(world);
            }
            float _Float_533369613b334b55b0d474f423bf7b90_Out_0_Float = 1024;
            float3 _Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3, (_Float_533369613b334b55b0d474f423bf7b90_Out_0_Float.xxx), _Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3);
            float2 _Property_f4fa5733f5784bf09f8ce7f08e85d795_Out_0_Vector2 = _Texture_Tiling_Y;
            float2 _TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2;
            Unity_TilingAndOffset_float((_Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3.xy), _Property_f4fa5733f5784bf09f8ce7f08e85d795_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2);
            float4 _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.tex, _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.samplerstate, _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2) );
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_R_4_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.r;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_G_5_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.g;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_B_6_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.b;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_A_7_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.a;
            float _Property_07ccd7056f4349bfafd4c32f2bd37a37_Out_0_Float = _Multiply_Y;
            float4 _Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4, (_Property_07ccd7056f4349bfafd4c32f2bd37a37_Out_0_Float.xxxx), _Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4);
            float _Float_91bad711af0c4d269b062219652ebe74_Out_0_Float = 0;
            float _Property_8189dfe2f6ad47968458d6dff24d59c3_Out_0_Float = _Max_Y;
            float4 _Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4;
            Unity_Clamp_float4(_Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4, (_Float_91bad711af0c4d269b062219652ebe74_Out_0_Float.xxxx), (_Property_8189dfe2f6ad47968458d6dff24d59c3_Out_0_Float.xxxx), _Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4);
            float4 _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4 = _Colour_Y;
            float4 _Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4, _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4, _Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4);
            float _Split_0bda8a4c572445c1a0483058e9c288b1_R_1_Float = IN.VertexColor[0];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_G_2_Float = IN.VertexColor[1];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_B_3_Float = IN.VertexColor[2];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4, _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4, (_Split_0bda8a4c572445c1a0483058e9c288b1_G_2_Float.xxxx), _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4);
            float3 _Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3;
            Unity_Absolute_float3(IN.WorldSpaceNormal, _Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3);
            float _Property_04979e4fcff743349142e74d849424c0_Out_0_Float = _Mask_Bleed;
            float3 _Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3;
            Unity_Power_float3(_Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3, (_Property_04979e4fcff743349142e74d849424c0_Out_0_Float.xxx), _Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3);
            float3 _Vector3_67e65ec93a0b4446bad2ceb5932958ab_Out_0_Vector3 = float3(1, 1, 1);
            float _DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float;
            Unity_DotProduct_float3(_Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3, _Vector3_67e65ec93a0b4446bad2ceb5932958ab_Out_0_Vector3, _DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float);
            float3 _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3;
            Unity_Divide_float3(_Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3, (_DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float.xxx), _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3);
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_R_1_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[0];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_G_2_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[1];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_B_3_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[2];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_A_4_Float = 0;
            float4 _Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4, _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4, (_Split_c2b8d7a0273e49f3bf5df729c7fb7efd_G_2_Float.xxxx), _Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4);
            UnityTexture2D _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_Z);
            Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceNormal = IN.WorldSpaceNormal;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceTangent = IN.WorldSpaceTangent;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpacePosition = IN.WorldSpacePosition;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            float2 _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2;
            SG_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float(_SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2, _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2);
            float2 _Property_14646d1169cc4051872c6c93610e8c0e_Out_0_Vector2 = _Texture_Tiling_Z;
            float2 _TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2;
            Unity_TilingAndOffset_float(_SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2, _Property_14646d1169cc4051872c6c93610e8c0e_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2);
            float4 _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.tex, _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.samplerstate, _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2) );
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_R_4_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.r;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_G_5_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.g;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_B_6_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.b;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_A_7_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.a;
            float _Property_a1b218d2d10648a6888d1cc669044a03_Out_0_Float = _Multiply_Z;
            float4 _Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4, (_Property_a1b218d2d10648a6888d1cc669044a03_Out_0_Float.xxxx), _Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4);
            float _Float_27a00a0923a048048044cfccbfed1282_Out_0_Float = 0;
            float _Property_e6774f4d4fec4817a27051bab5029459_Out_0_Float = _Max_Z;
            float4 _Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4;
            Unity_Clamp_float4(_Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4, (_Float_27a00a0923a048048044cfccbfed1282_Out_0_Float.xxxx), (_Property_e6774f4d4fec4817a27051bab5029459_Out_0_Float.xxxx), _Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4);
            float4 _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4 = _Colour_Z;
            float4 _Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4, _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4, _Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4);
            float _Split_3baacdb441894745b42f4348c11980fd_R_1_Float = IN.VertexColor[0];
            float _Split_3baacdb441894745b42f4348c11980fd_G_2_Float = IN.VertexColor[1];
            float _Split_3baacdb441894745b42f4348c11980fd_B_3_Float = IN.VertexColor[2];
            float _Split_3baacdb441894745b42f4348c11980fd_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4, _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4, (_Split_3baacdb441894745b42f4348c11980fd_B_3_Float.xxxx), _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4);
            float4 _Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4, _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4, (_Split_c2b8d7a0273e49f3bf5df729c7fb7efd_B_3_Float.xxxx), _Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4);
            surface.BaseColor = (_Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4.xyz);
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
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpacePosition = input.positionWS;
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        
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
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 AbsoluteWorldSpacePosition;
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
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Mask_Bleed;
        float2 _Texture_Tiling_X;
        float _Multiply_X;
        float4 _Texture_X_TexelSize;
        float _Max_X;
        float _World_Aligned_X;
        float4 _Texture_Y_TexelSize;
        float _Multiply_Y;
        float _Max_Y;
        float2 _Texture_Tiling_Y;
        float4 _Texture_Z_TexelSize;
        float2 _Texture_Tiling_Z;
        float _Multiply_Z;
        float _Max_Z;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        TEXTURE2D(_Texture_X);
        SAMPLER(sampler_Texture_X);
        TEXTURE2D(_Texture_Y);
        SAMPLER(sampler_Texture_Y);
        TEXTURE2D(_Texture_Z);
        SAMPLER(sampler_Texture_Z);
        
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
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float4(float4 In, float4 Min, float4 Max, out float4 Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_Power_float3(float3 A, float3 B, out float3 Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_ChannelMask_GreenBlue_float3 (float3 In, out float3 Out)
        {
        Out = float3(0, In.g, In.b);
        }
        
        void Unity_Contrast_float(float3 In, float Contrast, out float3 Out)
        {
            float midpoint = pow(0.5, 2.2);
            Out =  (In - midpoint) * Contrast + midpoint;
        }
        
        void Unity_ChannelMask_RedBlue_float3 (float3 In, out float3 Out)
        {
        Out = float3(In.r, 0, In.b);
        }
        
        void Unity_ChannelMask_RedGreen_float3 (float3 In, out float3 Out)
        {
        Out = float3(In.r, In.g, 0);
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float
        {
        float3 WorldSpaceNormal;
        float3 WorldSpaceTangent;
        float3 WorldSpaceBiTangent;
        float3 WorldSpacePosition;
        float3 AbsoluteWorldSpacePosition;
        };
        
        void SG_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float(Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float IN, out float2 Alpha_1)
        {
        float3 _Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3;
        {
        // Converting Position from AbsoluteWorld to Object via world space
        float3 world;
        world = GetCameraRelativePositionWS(IN.AbsoluteWorldSpacePosition.xyz);
        _Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3 = TransformWorldToObject(world);
        }
        float _Float_d8d5ee1b4a9342e0acdb554a6fb02662_Out_0_Float = 1024;
        float3 _Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3;
        Unity_Divide_float3(_Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3, (_Float_d8d5ee1b4a9342e0acdb554a6fb02662_Out_0_Float.xxx), _Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3);
        float3 _ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3;
        Unity_ChannelMask_GreenBlue_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3);
        float3 _Vector3_2050ca69d4a947c6af34ad61848f07c4_Out_0_Vector3 = float3(0, 0, 1);
        float3 _Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3;
        {
        // Converting Position from Tangent to Object via world space
        float3 world;
        {
        float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
        world = TransformTangentToWorldDir(_Vector3_2050ca69d4a947c6af34ad61848f07c4_Out_0_Vector3.xyz, tangentTransform, false).xyz + IN.WorldSpacePosition;
        }
        _Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3 = TransformWorldToObject(world);
        }
        float3 _Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3;
        Unity_Absolute_float3(_Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3, _Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3);
        float _Float_081ee3c0116543d09db94af274033691_Out_0_Float = 160;
        float3 _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3;
        Unity_Power_float3(_Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3, (_Float_081ee3c0116543d09db94af274033691_Out_0_Float.xxx), _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3);
        float3 _Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3;
        Unity_Divide_float3(_Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3, _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3, _Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3);
        float3 _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3;
        Unity_Contrast_float(_Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3, 512, _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3);
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_R_1_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[0];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_G_2_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[1];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_B_3_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[2];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_A_4_Float = 0;
        float3 _Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_B_3_Float.xxx), _Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3);
        float3 _ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3;
        Unity_ChannelMask_RedBlue_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3);
        float3 _Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_G_2_Float.xxx), _Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3);
        float3 _ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3;
        Unity_ChannelMask_RedGreen_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3);
        float3 _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_R_1_Float.xxx), _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3);
        float3 _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3;
        Unity_Add_float3(_Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3, _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3, _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3);
        float3 _Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3;
        Unity_Add_float3(_Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3, _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3, _Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3);
        Alpha_1 = (_Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3.xy);
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
            UnityTexture2D _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_X);
            float _Property_77eb78a1223b4d979751f37999821590_Out_0_Boolean = _World_Aligned_X;
            float3 _Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3;
            Unity_Branch_float3(_Property_77eb78a1223b4d979751f37999821590_Out_0_Boolean, IN.WorldSpacePosition, IN.ObjectSpacePosition, _Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3);
            float2 _Property_541d4f6f903f4acfa089dc1dafd910a1_Out_0_Vector2 = _Texture_Tiling_X;
            float2 _TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2;
            Unity_TilingAndOffset_float((_Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3.xy), _Property_541d4f6f903f4acfa089dc1dafd910a1_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2);
            float4 _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.tex, _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.samplerstate, _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2) );
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_R_4_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.r;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_G_5_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.g;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_B_6_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.b;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_A_7_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.a;
            float _Property_5f265aae98de4403bb5c9208511ff569_Out_0_Float = _Multiply_X;
            float _Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float;
            Unity_Multiply_float_float(_SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_R_4_Float, _Property_5f265aae98de4403bb5c9208511ff569_Out_0_Float, _Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float);
            float _Float_333f00e6bfaf4c47ad3d1dbb4135d894_Out_0_Float = 0;
            float _Property_36c5a516db034c0a8d0372b436599be8_Out_0_Float = _Max_X;
            float _Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float;
            Unity_Clamp_float(_Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float, _Float_333f00e6bfaf4c47ad3d1dbb4135d894_Out_0_Float, _Property_36c5a516db034c0a8d0372b436599be8_Out_0_Float, _Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float);
            float4 _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4 = _Colour_X;
            float4 _Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float.xxxx), _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4, _Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4);
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_R_1_Float = IN.VertexColor[0];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_G_2_Float = IN.VertexColor[1];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_B_3_Float = IN.VertexColor[2];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4, _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4, (_Split_77f9fff0abdc40e589d1ea67f577cb5d_R_1_Float.xxxx), _Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4);
            UnityTexture2D _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_Y);
            float3 _Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3;
            {
                // Converting Position from AbsoluteWorld to Object via world space
                float3 world;
                world = GetCameraRelativePositionWS(IN.AbsoluteWorldSpacePosition.xyz);
                _Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3 = TransformWorldToObject(world);
            }
            float _Float_533369613b334b55b0d474f423bf7b90_Out_0_Float = 1024;
            float3 _Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3, (_Float_533369613b334b55b0d474f423bf7b90_Out_0_Float.xxx), _Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3);
            float2 _Property_f4fa5733f5784bf09f8ce7f08e85d795_Out_0_Vector2 = _Texture_Tiling_Y;
            float2 _TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2;
            Unity_TilingAndOffset_float((_Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3.xy), _Property_f4fa5733f5784bf09f8ce7f08e85d795_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2);
            float4 _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.tex, _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.samplerstate, _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2) );
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_R_4_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.r;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_G_5_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.g;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_B_6_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.b;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_A_7_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.a;
            float _Property_07ccd7056f4349bfafd4c32f2bd37a37_Out_0_Float = _Multiply_Y;
            float4 _Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4, (_Property_07ccd7056f4349bfafd4c32f2bd37a37_Out_0_Float.xxxx), _Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4);
            float _Float_91bad711af0c4d269b062219652ebe74_Out_0_Float = 0;
            float _Property_8189dfe2f6ad47968458d6dff24d59c3_Out_0_Float = _Max_Y;
            float4 _Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4;
            Unity_Clamp_float4(_Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4, (_Float_91bad711af0c4d269b062219652ebe74_Out_0_Float.xxxx), (_Property_8189dfe2f6ad47968458d6dff24d59c3_Out_0_Float.xxxx), _Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4);
            float4 _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4 = _Colour_Y;
            float4 _Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4, _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4, _Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4);
            float _Split_0bda8a4c572445c1a0483058e9c288b1_R_1_Float = IN.VertexColor[0];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_G_2_Float = IN.VertexColor[1];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_B_3_Float = IN.VertexColor[2];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4, _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4, (_Split_0bda8a4c572445c1a0483058e9c288b1_G_2_Float.xxxx), _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4);
            float3 _Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3;
            Unity_Absolute_float3(IN.WorldSpaceNormal, _Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3);
            float _Property_04979e4fcff743349142e74d849424c0_Out_0_Float = _Mask_Bleed;
            float3 _Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3;
            Unity_Power_float3(_Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3, (_Property_04979e4fcff743349142e74d849424c0_Out_0_Float.xxx), _Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3);
            float3 _Vector3_67e65ec93a0b4446bad2ceb5932958ab_Out_0_Vector3 = float3(1, 1, 1);
            float _DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float;
            Unity_DotProduct_float3(_Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3, _Vector3_67e65ec93a0b4446bad2ceb5932958ab_Out_0_Vector3, _DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float);
            float3 _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3;
            Unity_Divide_float3(_Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3, (_DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float.xxx), _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3);
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_R_1_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[0];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_G_2_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[1];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_B_3_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[2];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_A_4_Float = 0;
            float4 _Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4, _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4, (_Split_c2b8d7a0273e49f3bf5df729c7fb7efd_G_2_Float.xxxx), _Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4);
            UnityTexture2D _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_Z);
            Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceNormal = IN.WorldSpaceNormal;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceTangent = IN.WorldSpaceTangent;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpacePosition = IN.WorldSpacePosition;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            float2 _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2;
            SG_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float(_SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2, _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2);
            float2 _Property_14646d1169cc4051872c6c93610e8c0e_Out_0_Vector2 = _Texture_Tiling_Z;
            float2 _TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2;
            Unity_TilingAndOffset_float(_SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2, _Property_14646d1169cc4051872c6c93610e8c0e_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2);
            float4 _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.tex, _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.samplerstate, _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2) );
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_R_4_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.r;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_G_5_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.g;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_B_6_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.b;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_A_7_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.a;
            float _Property_a1b218d2d10648a6888d1cc669044a03_Out_0_Float = _Multiply_Z;
            float4 _Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4, (_Property_a1b218d2d10648a6888d1cc669044a03_Out_0_Float.xxxx), _Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4);
            float _Float_27a00a0923a048048044cfccbfed1282_Out_0_Float = 0;
            float _Property_e6774f4d4fec4817a27051bab5029459_Out_0_Float = _Max_Z;
            float4 _Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4;
            Unity_Clamp_float4(_Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4, (_Float_27a00a0923a048048044cfccbfed1282_Out_0_Float.xxxx), (_Property_e6774f4d4fec4817a27051bab5029459_Out_0_Float.xxxx), _Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4);
            float4 _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4 = _Colour_Z;
            float4 _Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4, _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4, _Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4);
            float _Split_3baacdb441894745b42f4348c11980fd_R_1_Float = IN.VertexColor[0];
            float _Split_3baacdb441894745b42f4348c11980fd_G_2_Float = IN.VertexColor[1];
            float _Split_3baacdb441894745b42f4348c11980fd_B_3_Float = IN.VertexColor[2];
            float _Split_3baacdb441894745b42f4348c11980fd_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4, _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4, (_Split_3baacdb441894745b42f4348c11980fd_B_3_Float.xxxx), _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4);
            float4 _Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4, _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4, (_Split_c2b8d7a0273e49f3bf5df729c7fb7efd_B_3_Float.xxxx), _Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4);
            surface.BaseColor = (_Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4.xyz);
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
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpacePosition = input.positionWS;
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        
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
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Mask_Bleed;
        float2 _Texture_Tiling_X;
        float _Multiply_X;
        float4 _Texture_X_TexelSize;
        float _Max_X;
        float _World_Aligned_X;
        float4 _Texture_Y_TexelSize;
        float _Multiply_Y;
        float _Max_Y;
        float2 _Texture_Tiling_Y;
        float4 _Texture_Z_TexelSize;
        float2 _Texture_Tiling_Z;
        float _Multiply_Z;
        float _Max_Z;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        TEXTURE2D(_Texture_X);
        SAMPLER(sampler_Texture_X);
        TEXTURE2D(_Texture_Y);
        SAMPLER(sampler_Texture_Y);
        TEXTURE2D(_Texture_Z);
        SAMPLER(sampler_Texture_Z);
        
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
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Mask_Bleed;
        float2 _Texture_Tiling_X;
        float _Multiply_X;
        float4 _Texture_X_TexelSize;
        float _Max_X;
        float _World_Aligned_X;
        float4 _Texture_Y_TexelSize;
        float _Multiply_Y;
        float _Max_Y;
        float2 _Texture_Tiling_Y;
        float4 _Texture_Z_TexelSize;
        float2 _Texture_Tiling_Z;
        float _Multiply_Z;
        float _Max_Z;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        TEXTURE2D(_Texture_X);
        SAMPLER(sampler_Texture_X);
        TEXTURE2D(_Texture_Y);
        SAMPLER(sampler_Texture_Y);
        TEXTURE2D(_Texture_Z);
        SAMPLER(sampler_Texture_Z);
        
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
        #define VARYINGS_NEED_TANGENT_WS
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
             float4 tangentWS;
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
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 ObjectSpacePosition;
             float3 WorldSpacePosition;
             float3 AbsoluteWorldSpacePosition;
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
             float4 texCoord0 : INTERP1;
             float4 color : INTERP2;
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
            output.tangentWS.xyzw = input.tangentWS;
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
            output.tangentWS = input.tangentWS.xyzw;
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
        float4 _Texture2D_TexelSize;
        float4 _Colour_X;
        float4 _Colour_Y;
        float4 _Colour_Z;
        float _Mask_Bleed;
        float2 _Texture_Tiling_X;
        float _Multiply_X;
        float4 _Texture_X_TexelSize;
        float _Max_X;
        float _World_Aligned_X;
        float4 _Texture_Y_TexelSize;
        float _Multiply_Y;
        float _Max_Y;
        float2 _Texture_Tiling_Y;
        float4 _Texture_Z_TexelSize;
        float2 _Texture_Tiling_Z;
        float _Multiply_Z;
        float _Max_Z;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Texture2D);
        SAMPLER(sampler_Texture2D);
        TEXTURE2D(_Texture_X);
        SAMPLER(sampler_Texture_X);
        TEXTURE2D(_Texture_Y);
        SAMPLER(sampler_Texture_Y);
        TEXTURE2D(_Texture_Z);
        SAMPLER(sampler_Texture_Z);
        
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
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Clamp_float4(float4 In, float4 Min, float4 Max, out float4 Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Absolute_float3(float3 In, out float3 Out)
        {
            Out = abs(In);
        }
        
        void Unity_Power_float3(float3 A, float3 B, out float3 Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_DotProduct_float3(float3 A, float3 B, out float Out)
        {
            Out = dot(A, B);
        }
        
        void Unity_Divide_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A / B;
        }
        
        void Unity_ChannelMask_GreenBlue_float3 (float3 In, out float3 Out)
        {
        Out = float3(0, In.g, In.b);
        }
        
        void Unity_Contrast_float(float3 In, float Contrast, out float3 Out)
        {
            float midpoint = pow(0.5, 2.2);
            Out =  (In - midpoint) * Contrast + midpoint;
        }
        
        void Unity_ChannelMask_RedBlue_float3 (float3 In, out float3 Out)
        {
        Out = float3(In.r, 0, In.b);
        }
        
        void Unity_ChannelMask_RedGreen_float3 (float3 In, out float3 Out)
        {
        Out = float3(In.r, In.g, 0);
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float
        {
        float3 WorldSpaceNormal;
        float3 WorldSpaceTangent;
        float3 WorldSpaceBiTangent;
        float3 WorldSpacePosition;
        float3 AbsoluteWorldSpacePosition;
        };
        
        void SG_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float(Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float IN, out float2 Alpha_1)
        {
        float3 _Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3;
        {
        // Converting Position from AbsoluteWorld to Object via world space
        float3 world;
        world = GetCameraRelativePositionWS(IN.AbsoluteWorldSpacePosition.xyz);
        _Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3 = TransformWorldToObject(world);
        }
        float _Float_d8d5ee1b4a9342e0acdb554a6fb02662_Out_0_Float = 1024;
        float3 _Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3;
        Unity_Divide_float3(_Transform_e9bec0e35831453a994f90e7b0453849_Out_1_Vector3, (_Float_d8d5ee1b4a9342e0acdb554a6fb02662_Out_0_Float.xxx), _Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3);
        float3 _ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3;
        Unity_ChannelMask_GreenBlue_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3);
        float3 _Vector3_2050ca69d4a947c6af34ad61848f07c4_Out_0_Vector3 = float3(0, 0, 1);
        float3 _Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3;
        {
        // Converting Position from Tangent to Object via world space
        float3 world;
        {
        float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
        world = TransformTangentToWorldDir(_Vector3_2050ca69d4a947c6af34ad61848f07c4_Out_0_Vector3.xyz, tangentTransform, false).xyz + IN.WorldSpacePosition;
        }
        _Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3 = TransformWorldToObject(world);
        }
        float3 _Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3;
        Unity_Absolute_float3(_Transform_bec2f16fc1ad4315b55113b169029f51_Out_1_Vector3, _Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3);
        float _Float_081ee3c0116543d09db94af274033691_Out_0_Float = 160;
        float3 _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3;
        Unity_Power_float3(_Absolute_b2182449a62f44808113365ada259ee2_Out_1_Vector3, (_Float_081ee3c0116543d09db94af274033691_Out_0_Float.xxx), _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3);
        float3 _Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3;
        Unity_Divide_float3(_Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3, _Power_2f521294f76e45ac8b563f020e192d78_Out_2_Vector3, _Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3);
        float3 _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3;
        Unity_Contrast_float(_Divide_b969b5ce97a54395a8e8e280383b32d2_Out_2_Vector3, 512, _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3);
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_R_1_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[0];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_G_2_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[1];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_B_3_Float = _Contrast_600f4e8d94244859adbd158287369cd4_Out_2_Vector3[2];
        float _Split_f23741fee00f4e28b3c8caf2939dd35a_A_4_Float = 0;
        float3 _Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_a0e64fdfddb540809b324da85474f6ea_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_B_3_Float.xxx), _Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3);
        float3 _ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3;
        Unity_ChannelMask_RedBlue_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3);
        float3 _Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_27749b8cadbd42f5991ec7cd4ad98b27_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_G_2_Float.xxx), _Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3);
        float3 _ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3;
        Unity_ChannelMask_RedGreen_float3 (_Divide_e749f4409b1e4d32848eefb2bf3c88fb_Out_2_Vector3, _ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3);
        float3 _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3;
        Unity_Multiply_float3_float3(_ChannelMask_781535be92614c1aa8ae024956bcfc07_Out_1_Vector3, (_Split_f23741fee00f4e28b3c8caf2939dd35a_R_1_Float.xxx), _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3);
        float3 _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3;
        Unity_Add_float3(_Multiply_1f36462bfff14a8fabe4ce5bb67f59e4_Out_2_Vector3, _Multiply_585d93bfd4854357a05b3f76c623991b_Out_2_Vector3, _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3);
        float3 _Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3;
        Unity_Add_float3(_Multiply_b9d2e30cdcf94763a7124fa853f93fe7_Out_2_Vector3, _Add_b117ee877fb641718fd739e492e47156_Out_2_Vector3, _Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3);
        Alpha_1 = (_Add_0f71d5ddfd914e3180fab7da8ea55391_Out_2_Vector3.xy);
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
            UnityTexture2D _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_X);
            float _Property_77eb78a1223b4d979751f37999821590_Out_0_Boolean = _World_Aligned_X;
            float3 _Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3;
            Unity_Branch_float3(_Property_77eb78a1223b4d979751f37999821590_Out_0_Boolean, IN.WorldSpacePosition, IN.ObjectSpacePosition, _Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3);
            float2 _Property_541d4f6f903f4acfa089dc1dafd910a1_Out_0_Vector2 = _Texture_Tiling_X;
            float2 _TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2;
            Unity_TilingAndOffset_float((_Branch_e1223826402a4fad99ccfb3763ee7a8e_Out_3_Vector3.xy), _Property_541d4f6f903f4acfa089dc1dafd910a1_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2);
            float4 _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.tex, _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.samplerstate, _Property_a4edc47ff33343a4a024241b9200bf8b_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_55569936ff134440b297bf2c879e31bb_Out_3_Vector2) );
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_R_4_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.r;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_G_5_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.g;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_B_6_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.b;
            float _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_A_7_Float = _SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_RGBA_0_Vector4.a;
            float _Property_5f265aae98de4403bb5c9208511ff569_Out_0_Float = _Multiply_X;
            float _Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float;
            Unity_Multiply_float_float(_SampleTexture2D_b0401ed42f864af68aa85a7d8ea4911e_R_4_Float, _Property_5f265aae98de4403bb5c9208511ff569_Out_0_Float, _Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float);
            float _Float_333f00e6bfaf4c47ad3d1dbb4135d894_Out_0_Float = 0;
            float _Property_36c5a516db034c0a8d0372b436599be8_Out_0_Float = _Max_X;
            float _Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float;
            Unity_Clamp_float(_Multiply_2302e228ae284cd0a84fea8303b995c9_Out_2_Float, _Float_333f00e6bfaf4c47ad3d1dbb4135d894_Out_0_Float, _Property_36c5a516db034c0a8d0372b436599be8_Out_0_Float, _Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float);
            float4 _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4 = _Colour_X;
            float4 _Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Clamp_95889a2995f2453d9b42fcfe014f1ef0_Out_3_Float.xxxx), _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4, _Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4);
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_R_1_Float = IN.VertexColor[0];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_G_2_Float = IN.VertexColor[1];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_B_3_Float = IN.VertexColor[2];
            float _Split_77f9fff0abdc40e589d1ea67f577cb5d_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_1dccef5a80e2418c87eec9f7c6cb443a_Out_2_Vector4, _Property_d5d42953fbb24b4eac60e46780ff36a3_Out_0_Vector4, (_Split_77f9fff0abdc40e589d1ea67f577cb5d_R_1_Float.xxxx), _Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4);
            UnityTexture2D _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_Y);
            float3 _Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3;
            {
                // Converting Position from AbsoluteWorld to Object via world space
                float3 world;
                world = GetCameraRelativePositionWS(IN.AbsoluteWorldSpacePosition.xyz);
                _Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3 = TransformWorldToObject(world);
            }
            float _Float_533369613b334b55b0d474f423bf7b90_Out_0_Float = 1024;
            float3 _Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Transform_14be69b1ff0a44c18f73a5124b6b849f_Out_1_Vector3, (_Float_533369613b334b55b0d474f423bf7b90_Out_0_Float.xxx), _Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3);
            float2 _Property_f4fa5733f5784bf09f8ce7f08e85d795_Out_0_Vector2 = _Texture_Tiling_Y;
            float2 _TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2;
            Unity_TilingAndOffset_float((_Multiply_fb581e5b18634598bc2b9f177eb751aa_Out_2_Vector3.xy), _Property_f4fa5733f5784bf09f8ce7f08e85d795_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2);
            float4 _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.tex, _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.samplerstate, _Property_5d2b8d6233a14620936b3da52556ec1c_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_865e2d5b8f004e64a45f1f4b433dabbb_Out_3_Vector2) );
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_R_4_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.r;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_G_5_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.g;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_B_6_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.b;
            float _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_A_7_Float = _SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4.a;
            float _Property_07ccd7056f4349bfafd4c32f2bd37a37_Out_0_Float = _Multiply_Y;
            float4 _Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_3cf4514c9c06426b821e0ed5cd42f777_RGBA_0_Vector4, (_Property_07ccd7056f4349bfafd4c32f2bd37a37_Out_0_Float.xxxx), _Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4);
            float _Float_91bad711af0c4d269b062219652ebe74_Out_0_Float = 0;
            float _Property_8189dfe2f6ad47968458d6dff24d59c3_Out_0_Float = _Max_Y;
            float4 _Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4;
            Unity_Clamp_float4(_Multiply_8f65d38321e8455cbc19f16c6f4dc962_Out_2_Vector4, (_Float_91bad711af0c4d269b062219652ebe74_Out_0_Float.xxxx), (_Property_8189dfe2f6ad47968458d6dff24d59c3_Out_0_Float.xxxx), _Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4);
            float4 _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4 = _Colour_Y;
            float4 _Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Clamp_7740fc0052db442d9761111bac90ef62_Out_3_Vector4, _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4, _Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4);
            float _Split_0bda8a4c572445c1a0483058e9c288b1_R_1_Float = IN.VertexColor[0];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_G_2_Float = IN.VertexColor[1];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_B_3_Float = IN.VertexColor[2];
            float _Split_0bda8a4c572445c1a0483058e9c288b1_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_e54b6da28f884825bab3e66c5fd250fa_Out_2_Vector4, _Property_4da3346cd0e14512b619f650a86c3589_Out_0_Vector4, (_Split_0bda8a4c572445c1a0483058e9c288b1_G_2_Float.xxxx), _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4);
            float3 _Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3;
            Unity_Absolute_float3(IN.WorldSpaceNormal, _Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3);
            float _Property_04979e4fcff743349142e74d849424c0_Out_0_Float = _Mask_Bleed;
            float3 _Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3;
            Unity_Power_float3(_Absolute_e6c893cd31f44f6d9ee56a9a89ae9220_Out_1_Vector3, (_Property_04979e4fcff743349142e74d849424c0_Out_0_Float.xxx), _Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3);
            float3 _Vector3_67e65ec93a0b4446bad2ceb5932958ab_Out_0_Vector3 = float3(1, 1, 1);
            float _DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float;
            Unity_DotProduct_float3(_Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3, _Vector3_67e65ec93a0b4446bad2ceb5932958ab_Out_0_Vector3, _DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float);
            float3 _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3;
            Unity_Divide_float3(_Power_dbd14e7d3c21402796cec3a4e3185a78_Out_2_Vector3, (_DotProduct_25540abbe18e43cab56607a793abdeeb_Out_2_Float.xxx), _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3);
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_R_1_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[0];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_G_2_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[1];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_B_3_Float = _Divide_0d4e48d4a2db4b48afbee3a4c66d965a_Out_2_Vector3[2];
            float _Split_c2b8d7a0273e49f3bf5df729c7fb7efd_A_4_Float = 0;
            float4 _Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_fa7b389972f043a78ce2f1ef37ab9bca_Out_3_Vector4, _Lerp_cff00c8b9f1445b28499856f61a29ff8_Out_3_Vector4, (_Split_c2b8d7a0273e49f3bf5df729c7fb7efd_G_2_Float.xxxx), _Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4);
            UnityTexture2D _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture_Z);
            Bindings_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceNormal = IN.WorldSpaceNormal;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceTangent = IN.WorldSpaceTangent;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.WorldSpacePosition = IN.WorldSpacePosition;
            _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2.AbsoluteWorldSpacePosition = IN.AbsoluteWorldSpacePosition;
            float2 _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2;
            SG_SBGCheaptriplanar_82958fd6428020c44bfdb5b5b5fb1ce6_float(_SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2, _SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2);
            float2 _Property_14646d1169cc4051872c6c93610e8c0e_Out_0_Vector2 = _Texture_Tiling_Z;
            float2 _TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2;
            Unity_TilingAndOffset_float(_SBGCheaptriplanar_f9562350432545bc9de168cf4ef50da2_Alpha_1_Vector2, _Property_14646d1169cc4051872c6c93610e8c0e_Out_0_Vector2, float2 (0, 0), _TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2);
            float4 _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.tex, _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.samplerstate, _Property_3a1c9b62306a4a1d80a378113aac07b2_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_96225466ac034ec3b3a216ee0f91d887_Out_3_Vector2) );
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_R_4_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.r;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_G_5_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.g;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_B_6_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.b;
            float _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_A_7_Float = _SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4.a;
            float _Property_a1b218d2d10648a6888d1cc669044a03_Out_0_Float = _Multiply_Z;
            float4 _Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_5df50a6409cf462187b01d32f421bd3a_RGBA_0_Vector4, (_Property_a1b218d2d10648a6888d1cc669044a03_Out_0_Float.xxxx), _Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4);
            float _Float_27a00a0923a048048044cfccbfed1282_Out_0_Float = 0;
            float _Property_e6774f4d4fec4817a27051bab5029459_Out_0_Float = _Max_Z;
            float4 _Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4;
            Unity_Clamp_float4(_Multiply_c8641e4b849041c8879c09f706b7f9ac_Out_2_Vector4, (_Float_27a00a0923a048048044cfccbfed1282_Out_0_Float.xxxx), (_Property_e6774f4d4fec4817a27051bab5029459_Out_0_Float.xxxx), _Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4);
            float4 _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4 = _Colour_Z;
            float4 _Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Clamp_1198deb405894dec876887d4a1e7267a_Out_3_Vector4, _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4, _Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4);
            float _Split_3baacdb441894745b42f4348c11980fd_R_1_Float = IN.VertexColor[0];
            float _Split_3baacdb441894745b42f4348c11980fd_G_2_Float = IN.VertexColor[1];
            float _Split_3baacdb441894745b42f4348c11980fd_B_3_Float = IN.VertexColor[2];
            float _Split_3baacdb441894745b42f4348c11980fd_A_4_Float = IN.VertexColor[3];
            float4 _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4;
            Unity_Lerp_float4(_Multiply_10d2eb066b924dfd8831e97dbcdfc4de_Out_2_Vector4, _Property_6721ec309bc849cb98b90a1a94cffac6_Out_0_Vector4, (_Split_3baacdb441894745b42f4348c11980fd_B_3_Float.xxxx), _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4);
            float4 _Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4;
            Unity_Lerp_float4(_Lerp_a762218cb5934e33a1e02870d8998773_Out_3_Vector4, _Lerp_4f19bf5bf75547ebab911b8ec123933a_Out_3_Vector4, (_Split_c2b8d7a0273e49f3bf5df729c7fb7efd_B_3_Float.xxxx), _Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4);
            surface.BaseColor = (_Lerp_442e1d6e93fd4830a59f16aff64691dc_Out_3_Vector4.xyz);
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
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpacePosition = input.positionWS;
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        
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