Shader "Custom/WaterShader"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        _F0 ("F0", Range(0.0, 1.0)) = 0.02
        _PerlinNoise ("PerlinNoise", Range(0.0, 10.0)) = 0.02
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+1"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
            "ShaderModel"="4.5"
        }
        LOD 100

        //Blend One Zero
        ZWrite On
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            //Name "FowerdLit"
            //            Tags
            //            {
            //                "RenderType" = "Transparent"
            //                "Queue" = "Transparent"
            //                "RenderPipeline"="UniversalPipeline"
            //                "LightMode" = "UniversalForward"
            //            }

            HLSLPROGRAM
            //#pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            //#pragma alpha:fade

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _F0;
                half _PerlinNoise;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float fogCoord : TEXCOORD3;
                float4 positionCS : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            half2 Random2(half2 st)
            {
                st = half2(dot(st, half2(127.1, 311.7)),
                           dot(st, half2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
            }

            float PerlinNoise(half2 st)
            {
                half2 p = floor(st);
                half2 f = frac(st);
                half2 u = f * f * (3.0 - 2.0 * f);

                float v00 = Random2(p + half2(0, 0));
                float v10 = Random2(p + half2(1, 0));
                float v01 = Random2(p + half2(0, 1));
                float v11 = Random2(p + half2(1, 1));

                return lerp(lerp(dot(v00, f - half2(0, 0)), dot(v10, f - half2(1, 0)), u.x),
                            lerp(dot(v01, f - half2(0, 1)), dot(v11, f - half2(1, 1)), u.x),
                            u.y) + 0.5f;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;

                float noise = PerlinNoise(output.positionCS);
                output.positionCS.y += (sin(noise * _PerlinNoise * vertexInput.positionCS.y + _Time * 100) / 2);
                //output.positionCS = output.positionCS;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
                half3 color = texColor.rgb * _BaseColor.rgb;
                half alpha = texColor.a * _BaseColor.a;

                half frenel = _F0 + (1.0 - _F0) * pow(1 - dot(normalize(input.viewDirWS), input.normalWS), 5);
                color += frenel;
                alpha += frenel;

                color = MixFog(color, input.fogCoord);

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Transparent/Diffuse"
}