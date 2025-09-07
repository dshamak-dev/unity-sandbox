Shader "Custom/AdvancedRoadShaderURP"
{
    Properties
    {
        // Road properties
        _RoadColor ("Road Color", Color) = (0.2, 0.2, 0.2, 1)
        _RoadTexture ("Road Texture", 2D) = "white" {}
        _RoadTextureScale ("Road Texture Scale", Float) = 1
        _RoadNormalMap ("Road Normal Map", 2D) = "bump" {}
        _RoadNormalScale ("Road Normal Scale", Float) = 1
        _RoadSmoothness ("Road Smoothness", Range(0, 1)) = 0.1
        
        // Sidewalk properties
        _SidewalkColor ("Sidewalk Color", Color) = (0.8, 0.8, 0.8, 1)
        _SidewalkTexture ("Sidewalk Texture", 2D) = "white" {}
        _SidewalkTextureScale ("Sidewalk Texture Scale", Float) = 1
        _SidewalkNormalMap ("Sidewalk Normal Map", 2D) = "bump" {}
        _SidewalkNormalScale ("Sidewalk Normal Scale", Float) = 1
        _SidewalkSmoothness ("Sidewalk Smoothness", Range(0, 1)) = 0.3
        
        // Road line properties
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _LineTexture ("Line Texture", 2D) = "white" {}
        _LineTextureScale ("Line Texture Scale", Float) = 1
        _LineNormalMap ("Line Normal Map", 2D) = "bump" {}
        _LineNormalScale ("Line Normal Scale", Float) = 1
        _LineSmoothness ("Line Smoothness", Range(0, 1)) = 0.8
        _LineEmission ("Line Emission", Float) = 0.5
        _LineWidth ("Line Width", Range(0, 0.1)) = 0.02
        
        // Border line properties (between road and sidewalk)
        _BorderLineColor ("Border Line Color", Color) = (1, 1, 1, 1)
        _BorderLineWidth ("Border Line Width", Range(0, 0.05)) = 0.01
        _BorderLineEmission ("Border Line Emission", Float) = 0.3
        _BorderLineSmoothness ("Border Line Smoothness", Range(0, 1)) = 0.7
        
        // Configuration properties
        [Toggle]_HasLeftSidewalk ("Has Left Sidewalk", Float) = 1
        _LeftSidewalkWidth ("Left Sidewalk Width", Range(0, 0.5)) = 0.15
        [Toggle]_HasRightSidewalk ("Has Right Sidewalk", Float) = 1
        _RightSidewalkWidth ("Right Sidewalk Width", Range(0, 0.5)) = 0.15
        
        _RoadLinesAmount ("Road Lines Amount", Int) = 1
        [Toggle]_HasDashedSeparator ("Has Dashed Separator", Float) = 1
        _LineRepeat ("Line Repeat", Float) = 5
        _LineDashRatio ("Line Dash Ratio", Range(0, 1)) = 0.5
        
        // Advanced properties
        _TransitionSharpness ("Transition Sharpness", Range(1, 20)) = 5
        _UVRotation ("UV Rotation", Range(0, 360)) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 positionWS   : TEXCOORD2;
                float4 shadowCoord  : TEXCOORD3;
                float4 positionCS   : SV_POSITION;
            };

            TEXTURE2D(_RoadTexture);
            SAMPLER(sampler_RoadTexture);
            TEXTURE2D(_RoadNormalMap);
            SAMPLER(sampler_RoadNormalMap);
            
            TEXTURE2D(_SidewalkTexture);
            SAMPLER(sampler_SidewalkTexture);
            TEXTURE2D(_SidewalkNormalMap);
            SAMPLER(sampler_SidewalkNormalMap);
            
            TEXTURE2D(_LineTexture);
            SAMPLER(sampler_LineTexture);
            TEXTURE2D(_LineNormalMap);
            SAMPLER(sampler_LineNormalMap);

            CBUFFER_START(UnityPerMaterial)
            // Road properties
            float4 _RoadColor;
            float _RoadTextureScale;
            float _RoadNormalScale;
            float _RoadSmoothness;
            
            // Sidewalk properties
            float4 _SidewalkColor;
            float _SidewalkTextureScale;
            float _SidewalkNormalScale;
            float _SidewalkSmoothness;
            
            // Road line properties
            float4 _LineColor;
            float _LineTextureScale;
            float _LineNormalScale;
            float _LineSmoothness;
            float _LineEmission;
            float _LineWidth;
            
            // Border line properties
            float4 _BorderLineColor;
            float _BorderLineWidth;
            float _BorderLineEmission;
            float _BorderLineSmoothness;
            
            // Configuration properties
            float _HasLeftSidewalk;
            float _LeftSidewalkWidth;
            float _HasRightSidewalk;
            float _RightSidewalkWidth;
            
            int _RoadLinesAmount;
            float _HasDashedSeparator;
            float _LineRepeat;
            float _LineDashRatio;
            
            // Advanced properties
            float _TransitionSharpness;
            float _UVRotation;
            CBUFFER_END

            // Function to rotate UV coordinates
            float2 rotateUV(float2 uv, float rotation)
            {
                float rad = rotation * PI / 180.0;
                float s = sin(rad);
                float c = cos(rad);
                float2x2 mat = float2x2(c, -s, s, c);
                return mul(mat, uv - 0.5) + 0.5;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = input.uv;
                
                output.shadowCoord = GetShadowCoord(vertexInput);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Get UV coordinates with rotation applied
                float2 uv = rotateUV(input.uv, _UVRotation);
                
                // Calculate sidewalk areas based on toggle states
                float sidewalkLeftMask = _HasLeftSidewalk > 0.5 ? step(uv.x, _LeftSidewalkWidth) : 0.0;
                float sidewalkRightMask = _HasRightSidewalk > 0.5 ? step(1.0 - uv.x, _RightSidewalkWidth) : 0.0;
                float sidewalkMask = max(sidewalkLeftMask, sidewalkRightMask);
                float roadMask = 1.0 - sidewalkMask;
                
                // Calculate border lines between road and sidewalk
                float borderLineLeft = _HasLeftSidewalk > 0.5 ? step(abs(uv.x - _LeftSidewalkWidth), _BorderLineWidth) : 0.0;
                float borderLineRight = _HasRightSidewalk > 0.5 ? step(abs(uv.x - (1.0 - _RightSidewalkWidth)), _BorderLineWidth) : 0.0;
                float borderLineMask = max(borderLineLeft, borderLineRight);
                
                // Calculate road lines
                float lineMask = 0.0;
                if (_RoadLinesAmount > 0 && roadMask > 0.0)
                {
                    // Calculate road area boundaries
                    float roadLeftEdge = _HasLeftSidewalk > 0.5 ? _LeftSidewalkWidth : 0.0;
                    float roadRightEdge = _HasRightSidewalk > 0.5 ? (1.0 - _RightSidewalkWidth) : 1.0;
                    float roadWidth = roadRightEdge - roadLeftEdge;
                    
                    // Create lines based on amount
                    for (int i = 0; i < _RoadLinesAmount; i++)
                    {
                        // Calculate line position (evenly spaced across road width)
                        float linePos = roadLeftEdge + roadWidth * (float(i + 1) / float(_RoadLinesAmount + 1));
                        
                        // Determine if line should be dashed or solid
                        float linePattern = _HasDashedSeparator > 0.5 ? frac(uv.y * _LineRepeat) : 1.0;
                        float lineDash = _HasDashedSeparator > 0.5 ? step(linePattern, _LineDashRatio) : 1.0;
                        
                        // Add this line to the mask
                        lineMask += step(abs(uv.x - linePos), _LineWidth) * lineDash * roadMask;
                    }
                    
                    // Clamp line mask to 0-1 range
                    lineMask = saturate(lineMask);
                }
                
                // Apply transition sharpness
                sidewalkMask = pow(sidewalkMask, _TransitionSharpness);
                roadMask = pow(roadMask, _TransitionSharpness);
                lineMask = pow(lineMask, _TransitionSharpness);
                borderLineMask = pow(borderLineMask, _TransitionSharpness);
                
                // Remove border line from road and sidewalk masks
                roadMask = roadMask * (1.0 - borderLineMask);
                sidewalkMask = sidewalkMask * (1.0 - borderLineMask);
                
                // Sample road textures
                float2 roadUV = uv * _RoadTextureScale;
                half4 roadTex = SAMPLE_TEXTURE2D(_RoadTexture, sampler_RoadTexture, roadUV);
                half4 roadNormalMap = SAMPLE_TEXTURE2D(_RoadNormalMap, sampler_RoadNormalMap, roadUV);
                half3 roadNormal = UnpackNormalScale(roadNormalMap, _RoadNormalScale);
                
                // Sample sidewalk textures
                float2 sidewalkUV = uv * _SidewalkTextureScale;
                half4 sidewalkTex = SAMPLE_TEXTURE2D(_SidewalkTexture, sampler_SidewalkTexture, sidewalkUV);
                half4 sidewalkNormalMap = SAMPLE_TEXTURE2D(_SidewalkNormalMap, sampler_SidewalkNormalMap, sidewalkUV);
                half3 sidewalkNormal = UnpackNormalScale(sidewalkNormalMap, _SidewalkNormalScale);
                
                // Sample line textures
                float2 lineUV = uv * _LineTextureScale;
                half4 lineTex = SAMPLE_TEXTURE2D(_LineTexture, sampler_LineTexture, lineUV);
                half4 lineNormalMap = SAMPLE_TEXTURE2D(_LineNormalMap, sampler_LineNormalMap, lineUV);
                half3 lineNormal = UnpackNormalScale(lineNormalMap, _LineNormalScale);
                
                // Blend albedo
                half4 roadAlbedo = roadTex * _RoadColor;
                half4 sidewalkAlbedo = sidewalkTex * _SidewalkColor;
                half4 lineAlbedo = lineTex * _LineColor;
                half4 borderLineAlbedo = _BorderLineColor;
                
                half4 albedo = roadAlbedo * roadMask + 
                              sidewalkAlbedo * sidewalkMask + 
                              lineAlbedo * lineMask +
                              borderLineAlbedo * borderLineMask;
                
                // Blend normals
                half3 normal = roadNormal * roadMask + 
                              sidewalkNormal * sidewalkMask + 
                              lineNormal * lineMask;
                // Border line uses road normal
                normal = lerp(normal, roadNormal, borderLineMask);
                
                // Blend smoothness
                half smoothness = _RoadSmoothness * roadMask + 
                                 _SidewalkSmoothness * sidewalkMask + 
                                 _LineSmoothness * lineMask +
                                 _BorderLineSmoothness * borderLineMask;
                
                // Apply emission to lines and border
                half3 emission = lineAlbedo.rgb * _LineEmission * lineMask +
                                borderLineAlbedo.rgb * _BorderLineEmission * borderLineMask;
                
                // Lighting calculations
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = normalize(normal);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = input.shadowCoord;
                
                SurfaceData surfaceInput = (SurfaceData)0;
                surfaceInput.albedo = albedo.rgb;
                surfaceInput.alpha = albedo.a;
                surfaceInput.emission = emission;
                surfaceInput.smoothness = smoothness;
                surfaceInput.metallic = 0.0; // Non-metallic surface
                surfaceInput.normalTS = normal;
                
                // Apply lighting
                half4 color = UniversalFragmentPBR(lightingInput, surfaceInput);
                return color;
            }
            ENDHLSL
        }
        
        // Shadow pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}