﻿Shader "Hidden/Stylized Water/Caustics"
{
    Properties
    {
        _CausticsTexture("Texture", 2D) = "white" {}
        _CausticsStrength("Strength", Float) = 0
        _CausticsScale("Scale", Float) = 0.5
        _CausticsSpeed("Speed", Float) = 0
        _CausticsSplit("RGB Split", Float) = 0
        _CausticsShadowMask("Shadow Mask", Float) = 0
        _CausticsStart("Start", Float) = 0
        _CausticsEnd("End", Float) = 0
        _CausticsFade("Fade", Float) = 0
        _WaterLevel("WaterLevel", Float) = 0

        [HideInInspector] _SrcBlend("__src", Float) = 2.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
    }

    SubShader
    {
        ZWrite Off

        Pass
        {
            Blend [_SrcBlend] [_DstBlend], One Zero

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

            #pragma multi_compile _ DEBUG_CAUSTICS DEBUG_MASK

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionSS : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D(_CausticsTexture); SAMPLER(sampler_CausticsTexture);
            TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture);

            CBUFFER_START(UnityPerMaterial)
            half _CausticsScale;
            half _CausticsSpeed;
            half _CausticsSplit;
            half _CausticsShadowMask;
            half _CausticsStrength;
            half _CausticsStart;
            half _CausticsEnd;
            half _CausticsFade;
            half _WaterLevel;
            CBUFFER_END

            half4x4 _MainLightDirection;

            float2 Panner(float2 uv, float direction, float speed, float2 offset, float tiling)
            {
                direction = direction * 2 - 1;
                float2 d = normalize(float2(cos(PI * direction), sin(PI * direction)));
                return  (d * _Time.y * speed) + (uv * tiling) + offset;
            }

            float3 WorldPositionFromDepth(half2 positionSS, float depth)
            {
                float4x4 mat = UNITY_MATRIX_I_VP;
                #if UNITY_REVERSED_Z
                    mat._12_22_32_42 = -mat._12_22_32_42;              
                #else
                    depth = depth * 2 - 1;
                #endif
                float4 raw = mul(mat, float4(positionSS * 2 - 1, depth, 1));
                float3 positionWS = raw.rgb / raw.a;
                return positionWS;
            }

            float3 RGBSplit(float split, float2 uv)
            {
                float2 uv1 = uv + float2(split, split);
                float2 uv2 = uv + float2(split, -split);
                float2 uv3 = uv + float2(-split, -split);

                float r = SAMPLE_TEXTURE2D_LOD(_CausticsTexture, sampler_CausticsTexture, uv1, 0).r;
                float g = SAMPLE_TEXTURE2D_LOD(_CausticsTexture, sampler_CausticsTexture, uv2, 0).r;
                float b = SAMPLE_TEXTURE2D_LOD(_CausticsTexture, sampler_CausticsTexture, uv3, 0).r;

                return float3(r,g,b);
            }

            

            float3 Caustics(float4 uv, float split)
            {
                split *= 0.015;

                float3 tex1 = RGBSplit(split, uv.xy);
                float3 tex2 = RGBSplit(split, uv.zw);

                return 20 * min(tex1, tex2);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionSS = ComputeScreenPos(o.positionCS);
                return o;
            }
            
            real4 frag (v2f i) : SV_Target
            {
                float4 positionSS = i.positionSS / i.positionSS.w;
                
                // Get main light
                Light MainLight = GetMainLight();

                real depth = SampleSceneDepth(positionSS.xy);
                
                // world position from screen depth
                float4 positionWS = WorldPositionFromDepth(positionSS.xy, depth).xyzz;

                /* caustics uvs */
                float2 uv = mul(positionWS, _MainLightDirection).xy;
                float2 uv1 = Panner(uv, 1, _CausticsSpeed, 0, 1/_CausticsScale);
                float2 uv2 = Panner(uv, 1, _CausticsSpeed, 0, -1/_CausticsScale);

                /* caustics height mask*/
                half upperMask = -positionWS.y + (_WaterLevel - _CausticsStart);
                half lowerMask = positionWS.y - (_WaterLevel - _CausticsEnd);
                half heightMask = smoothstep(0, _CausticsFade, min(upperMask, lowerMask));

                /* shadow mask */
                half3 sceneColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, positionSS.xy).xyz;
                half sceneLuminance = Luminance(sceneColor);
                half shadowmask = lerp(1, sceneLuminance, _CausticsShadowMask);


                /* method 1*/
                _CausticsSplit *= 0.015;
                float3 tex1 = RGBSplit(_CausticsSplit, uv1);
                float3 tex2 = RGBSplit(_CausticsSplit, uv2);

                half3 caustics = min(tex1, tex2) * _CausticsStrength * 100;
                caustics *= shadowmask;
                caustics *= heightMask;

                /* method 2 */
                float4 A = SAMPLE_TEXTURE2D_LOD(_CausticsTexture, sampler_CausticsTexture, uv1, 0);
                float4 B = SAMPLE_TEXTURE2D_LOD(_CausticsTexture, sampler_CausticsTexture, uv2, 0);

                float CausticsDriver = (A.z * B.z) * 10 + A.z + B.z;
                CausticsDriver *= shadowmask;
                CausticsDriver *= heightMask;
                half3 Caustics = CausticsDriver * half3(A.w * 0.5, B.w * 0.75, B.x) * MainLight.color;
                
                #ifdef DEBUG_CAUSTICS
                    return real4(caustics, 1.0);
                #endif

                #ifdef DEBUG_MASK
                    return min(heightMask, shadowmask);
                #endif

                return real4(caustics + 1.0, 1.0);
            }
            ENDHLSL
        }
    }
}