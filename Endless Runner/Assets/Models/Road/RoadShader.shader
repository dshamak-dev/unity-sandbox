Shader "Custom/RoadShader"
{
    Properties
    {
        // Road properties
        _RoadColor ("Road Color", Color) = (0.2, 0.2, 0.2, 1)
        _RoadTexture ("Road Texture", 2D) = "white" {}
        _RoadTextureScale ("Road Texture Scale", Float) = 1
        _RoadNormalMap ("Road Normal Map", 2D) = "bump" {}
        _RoadNormalScale ("Road Normal Scale", Float) = 1
        _RoadRoughness ("Road Roughness", Range(0, 1)) = 0.8
        
        // Sidewalk properties
        _SidewalkColor ("Sidewalk Color", Color) = (0.8, 0.8, 0.8, 1)
        _SidewalkTexture ("Sidewalk Texture", 2D) = "white" {}
        _SidewalkTextureScale ("Sidewalk Texture Scale", Float) = 1
        _SidewalkNormalMap ("Sidewalk Normal Map", 2D) = "bump" {}
        _SidewalkNormalScale ("Sidewalk Normal Scale", Float) = 1
        _SidewalkRoughness ("Sidewalk Roughness", Range(0, 1)) = 0.6
        
        // Road line properties
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _LineTexture ("Line Texture", 2D) = "white" {}
        _LineTextureScale ("Line Texture Scale", Float) = 1
        _LineNormalMap ("Line Normal Map", 2D) = "bump" {}
        _LineNormalScale ("Line Normal Scale", Float) = 1
        _LineRoughness ("Line Roughness", Range(0, 1)) = 0.9
        _LineEmission ("Line Emission", Float) = 0.5
        
        // Configuration properties
        _SidewalkWidth ("Sidewalk Width", Range(0, 0.5)) = 0.15
        _LineWidth ("Line Width", Range(0, 0.1)) = 0.02
        _LineRepeat ("Line Repeat", Float) = 5
        _LineOffset ("Line Offset", Float) = 0
        _LineDashRatio ("Line Dash Ratio", Range(0, 1)) = 0.5
        
        // Advanced properties
        _TransitionSharpness ("Transition Sharpness", Range(1, 20)) = 5
        _UVRotation ("UV Rotation", Range(0, 360)) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // Road properties
        fixed4 _RoadColor;
        sampler2D _RoadTexture;
        float _RoadTextureScale;
        sampler2D _RoadNormalMap;
        float _RoadNormalScale;
        float _RoadRoughness;
        
        // Sidewalk properties
        fixed4 _SidewalkColor;
        sampler2D _SidewalkTexture;
        float _SidewalkTextureScale;
        sampler2D _SidewalkNormalMap;
        float _SidewalkNormalScale;
        float _SidewalkRoughness;
        
        // Road line properties
        fixed4 _LineColor;
        sampler2D _LineTexture;
        float _LineTextureScale;
        sampler2D _LineNormalMap;
        float _LineNormalScale;
        float _LineRoughness;
        float _LineEmission;
        
        // Configuration properties
        float _SidewalkWidth;
        float _LineWidth;
        float _LineRepeat;
        float _LineOffset;
        float _LineDashRatio;
        
        // Advanced properties
        float _TransitionSharpness;
        float _UVRotation;

        // Function to rotate UV coordinates
        float2 rotateUV(float2 uv, float rotation)
        {
            float rad = rotation * 3.14159265359 / 180.0;
            float s = sin(rad);
            float c = cos(rad);
            float2x2 mat = float2x2(c, -s, s, c);
            return mul(mat, uv - 0.5) + 0.5;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Get UV coordinates with rotation applied
            float2 uv = rotateUV(IN.uv_MainTex, _UVRotation);
            
            // Calculate sidewalk and road areas
            float sidewalkMask = step(abs(uv.x - 0.5), _SidewalkWidth);
            float roadMask = 1.0 - sidewalkMask;
            
            // Calculate road lines
            float linePattern = frac((uv.y + _LineOffset) * _LineRepeat);
            float lineDash = step(linePattern, _LineDashRatio);
            float lineMask = roadMask * step(abs(uv.x - 0.5), _LineWidth) * lineDash;
            
            // Apply transition sharpness
            sidewalkMask = pow(sidewalkMask, _TransitionSharpness);
            roadMask = pow(roadMask, _TransitionSharpness);
            lineMask = pow(lineMask, _TransitionSharpness);
            
            // Sample road textures
            float2 roadUV = uv * _RoadTextureScale;
            fixed4 roadTex = tex2D(_RoadTexture, roadUV);
            fixed3 roadNormal = UnpackNormal(tex2D(_RoadNormalMap, roadUV));
            roadNormal.xy *= _RoadNormalScale;
            
            // Sample sidewalk textures
            float2 sidewalkUV = uv * _SidewalkTextureScale;
            fixed4 sidewalkTex = tex2D(_SidewalkTexture, sidewalkUV);
            fixed3 sidewalkNormal = UnpackNormal(tex2D(_SidewalkNormalMap, sidewalkUV));
            sidewalkNormal.xy *= _SidewalkNormalScale;
            
            // Sample line textures
            float2 lineUV = uv * _LineTextureScale;
            fixed4 lineTex = tex2D(_LineTexture, lineUV);
            fixed3 lineNormal = UnpackNormal(tex2D(_LineNormalMap, lineUV));
            lineNormal.xy *= _LineNormalScale;
            
            // Blend albedo
            fixed4 roadAlbedo = roadTex * _RoadColor;
            fixed4 sidewalkAlbedo = sidewalkTex * _SidewalkColor;
            fixed4 lineAlbedo = lineTex * _LineColor;
            
            fixed4 albedo = roadAlbedo * roadMask + sidewalkAlbedo * sidewalkMask + lineAlbedo * lineMask;
            
            // Blend normals
            fixed3 normal = roadNormal * roadMask + sidewalkNormal * sidewalkMask + lineNormal * lineMask;
            
            // Blend roughness
            float smoothness = (1.0 - _RoadRoughness) * roadMask + 
                              (1.0 - _SidewalkRoughness) * sidewalkMask + 
                              (1.0 - _LineRoughness) * lineMask;
            
            // Apply emission to lines
            fixed3 emission = lineAlbedo.rgb * _LineEmission * lineMask;
            
            // Set output properties
            o.Albedo = albedo.rgb;
            o.Normal = normal;
            o.Metallic = 0.0; // Non-metallic surface
            o.Smoothness = smoothness;
            o.Emission = emission;
            o.Alpha = albedo.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}