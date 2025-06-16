Shader "Nishiki/Nishiki_Shader_URP"
{
    Properties
    {
        [Toggle] _CustomizeColor("CustomizeColor", int) = 0
        [Toggle] _CustomizeMetallic("CustomizeMetallic", int) = 0
        [Toggle] _CustomizeRoughness("CustomizeRoughness", int) = 0
        [Toggle] _CustomizeEmission("CustomizeEmission", int) = 0
        [Space(20)]

        _CustomBaseColor("CustomBasecolor", Color) = (1, 1, 1, 1)
        _CustomPatternColor1("CustomPatternColor1", Color) = (1, 0, 0, 1)
        _CustomPatternColor2("CustomPatternColor2", Color) = (0, 0, 0, 1)
        [HDR] _CustomEmissionColor ("CustomEmissionColor", Color) = (0,0,0)
        _CustomGrungeIntensity("CustomGrungeIntensity", Range(0, 1)) = 0.8
        _DiffusePower("DiffusePower", Range(0, 1)) = 0
        [Space(10)]

        _PatternOffsetX("PatternOffset X", float) = 0
        _PatternOffsetY("PatternOffset Y", float) = 0
        _PatternScale("PatternScale", float) = 1
        [Space(20)]

        _CustomMetallic("CustomMetallicValue", Range(0, 1)) = 0
        _CustomRoughness("CustomRoughnessValue", Range(0, 2)) = 0.3
        [Space(50)]

        [NoScaleOffset] _BaseColorMap("BaseColorMap", 2D) = "white" {}
        [NoScaleOffset] _MetallicMap("MetallicMap", 2D) = "black" {}
        [NoScaleOffset] _RoughnessMap("RoughnessMap", 2D) = "white" {}
        [NoScaleOffset] _NormalMap("NormalMap", 2D) = "bump" {}
        [Space(10)]

        [NoScaleOffset] _GrungeMask("GrungeMask", 2D) = "white" {}
        [NoScaleOffset] _PatternMask1("PatternMask1", 2D) = "black" {}
        [NoScaleOffset] _PatternMask2("PatternMask2", 2D) = "black" {}
        [NoScaleOffset] _PatternMap("PatternMap", 2D) = "white" {}

        [Toggle(_OUTLINE)] _EnableOutLine("Enable OutLine", Float) = 0.0
        _OutLineColor ("OutLineColor", Color) = (0, 0, 0, 1)
        _OutlineWidth ("OutlineWidth", Range(0, 100)) = 0
        
        _DitherLevel("DitherLevel", Range(0, 16)) = 0
    }

    SubShader
    {
        HLSLINCLUDE
        #pragma target 3.0
        #pragma require geometry

        //#pragma geometry geom
        // make fog work
        #pragma multi_compile_fog

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            sampler2D _BaseColorMap;
            sampler2D _RoughnessMap;
            sampler2D _MetallicMap;
            sampler2D _NormalMap;

            sampler2D _GrungeMask;
            sampler2D _PatternMap;
            sampler2D _PatternMask1;
            sampler2D _PatternMask2;

            int _CustomizeColor;
            int _CustomizeMetallic;
            int _CustomizeRoughness;
            int _CustomizeEmission;

            half3 _CustomBaseColor;
            half3 _CustomPatternColor1;
            half3 _CustomPatternColor2;
            half3 _CustomEmissionColor;
            float _DiffusePower;

            half _CustomGrungeIntensity;

            half _PatternOffsetX;
            half _PatternOffsetY;
            half _PatternScale;

            half _CustomMetallic;
            half _CustomRoughness;

            half _DitherLevel;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Tags
            {
                "RenderType"="Opaque"
                "LightMode"="UniversalForward"
                "RenderPipeline"="UniversalPipeline"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"

            struct appdata
            {
                float4 pos: POSITION;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                half4 tangent : TANGENT;
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : TEXCOORD1; // 法線
                half3 tangent : TEXCOORD2; // 接線
                half3 binormal : TEXCOORD3; // 従法線
                float4 worldPos : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
                float4 positionSS : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                // o.pos = TransformObjectToHClip(v.vertex);
                // o.normal = TransformObjectToHClip(v.vertex);
                o.pos = TransformObjectToHClip(v.positionOS);
                o.positionSS = ComputeScreenPos(o.pos);
                o.uv = v.uv;

                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_TRANSFER_INSTANCE_ID(i, o);
                VertexPositionInputs vInput = GetVertexPositionInputs(v.pos.xyz);
                o.pos = vInput.positionCS;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.normal = TransformObjectToWorldNormal(v.normal);
                o.tangent = normalize(mul(unity_ObjectToWorld, v.tangent)).xyz;
                // 法線と接線から従法線を計算
                o.binormal = cross(v.normal, v.tangent) * v.tangent.w;
                o.binormal = normalize(mul(unity_ObjectToWorld, o.binormal));
                o.worldPos = v.pos;

                o.shadowCoord = GetShadowCoord(vInput);
                return o;
            }

            half3 CustomizeColor(float2 uv)
            {
                float2 offsetuv = uv + float2(_PatternOffsetX, _PatternOffsetY);

                offsetuv = 0.5 - offsetuv;
                offsetuv *= _PatternScale;
                offsetuv = 0.5 + offsetuv;

                half3 baseColor = _CustomBaseColor;
                half3 patternColor1 = _CustomPatternColor1;
                half3 patternColor2 = _CustomPatternColor2;

                half patternMap = tex2D(_PatternMap, uv).r;
                half grungeMask = tex2D(_GrungeMask, offsetuv).r;
                half patternMask1 = tex2D(_PatternMask1, offsetuv).r;
                half patternMask2 = tex2D(_PatternMask2, offsetuv).r;

                patternMask1 *= patternMap;
                patternMask2 *= patternMap;

                half3 color = lerp(baseColor, patternColor1, patternMask1);
                color = lerp(color, patternColor2, patternMask2);

                half grunge = lerp(_CustomGrungeIntensity, 1, grungeMask).r;

                color *= grunge;

                color = lerp(color, tex2D(_BaseColorMap, uv), tex2D(_MetallicMap, uv).r);

                return color;
            }

            // しきい値マップ
            static const float4x4 pattern =
            {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 565
            };
            static const int PATTERN_ROW_SIZE = 4;

            half4 frag(v2f i) : COLOR
            {
                // half3 defaultColor = tex2D(_BaseColorMap, i.uv);
                // half defaultMetallic = tex2D(_MetallicMap, i.uv);
                // half defaultRoughness = tex2D(_RoughnessMap, i.uv);
                //
                // half3 color = _CustomizeColor ? CustomizeColor(i.uv) : defaultColor;
                // half metallic = _CustomizeMetallic ? _CustomMetallic : defaultMetallic;
                // half roughness = 1 - (_CustomizeRoughness ? _CustomRoughness : defaultRoughness);
                //
                // return float4(color, 1.0);

                UNITY_SETUP_INSTANCE_ID(i);

                half3 defaultColor = tex2D(_BaseColorMap, i.uv);
                half defaultMetallic = tex2D(_MetallicMap, i.uv);
                half defaultRoughness = tex2D(_RoughnessMap, i.uv);

                float3 lightDir = normalize(_MainLightPosition.xyz);
                half3 lightColor = _MainLightColor.xyz;

                half3 normalmap = UnpackNormal(tex2D(_NormalMap, i.uv));
                half costomRoughness = _CustomRoughness;

                // 法線を合成
                float3 normal =
                    (i.tangent * normalmap.x * costomRoughness) + (i.binormal * normalmap.y *
                        costomRoughness) + (i.normal * normalmap.z);

                float t = dot(normal, lightDir);
                t = max(_DiffusePower, t); // t >= 0にする -> _DiffusePowerで影の強さを決める
                // 内積が0に近いほど黒くd
                float3 diffuseLight = lightColor * t;

                // ライト方向と法線ベクトルから反射ベクトルを計算
                float3 reflectVec = reflect(-lightDir, normal);
                float3 eyeVec = normalize(GetCameraPositionWS() - i.worldPos); // 視線ベクトル

                t = dot(reflectVec, eyeVec);
                t = max(0, t); // t >= 0にする
                //dt = pow(t, _Specular);

                float3 specularLight = lightColor * t;

                Light light = GetMainLight(i.shadowCoord);
                half shadow = light.shadowAttenuation * light.distanceAttenuation;

                float4 color = _CustomizeColor
                        ? half4(shadow, shadow, shadow, 1) * half4(CustomizeColor(i.uv).rgb, 1)
                        : half4(defaultColor.rgb, 1);

                color = _CustomizeEmission
                                                               ? color + half4(CustomizeColor(i.uv).rgb, 1) * half4(
                                                                   _CustomEmissionColor, 1)
                                                               : color;

                color.xyz *= (specularLight + diffuseLight);

                // カメラからの距離
                float dist = distance(i.worldPos, _WorldSpaceCameraPos);
                // スクリーン座標
                float2 screenPos = i.positionSS.xy / i.positionSS.w;
                // 画面サイズを乗算して、ピクセル単位に
                float2 screenPosInPixel = screenPos.xy * _ScreenParams.xy;

                // ディザリングテクスチャ用のUVを作成
                int ditherUV_x = (int)fmod(screenPosInPixel.x, PATTERN_ROW_SIZE);
                int ditherUV_y = (int)fmod(screenPosInPixel.y, PATTERN_ROW_SIZE);
                float dither = pattern[ditherUV_x, ditherUV_y];
                //float cameraDistance = distance(i.pos.xyz, GetCameraPositionWS());

                // 閾値が0以下なら描画しない
                clip(dither - _DitherLevel);

                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "OutLine"
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _OUTLINE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float _OutlineWidth;
            half4 _OutLineColor;

            struct Attributes
            {
                float4 positionOS: POSITION;
                float4 normalOS: NORMAL;
                float4 tangentOS: TANGENT;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float4 positionSS : TEXCOORD0;
            };

            // 閾値マップ
            static const float4x4 pattern =
            {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 565
            };
            static const int PATTERN_ROW_SIZE = 4;

            Varyings vert(Attributes v)
            {
                Varyings OUT;

                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);

                float3 normalWS = vertexNormalInput.normalWS;
                float3 normalCS = TransformWorldToHClipDir(normalWS);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS + float4(normalCS.xy * 0.001 * _OutlineWidth, 0, 0);

                OUT.positionSS = ComputeScreenPos(OUT.positionCS);

                return OUT;
            }

            half4 frag(Varyings IN): SV_Target
            {
                float4 col = _OutLineColor;

                // スクリーン座標
                float2 screenPos = IN.positionSS.xy / IN.positionSS.w;
                // 画面サイズを乗算して、ピクセル単位に
                float2 screenPosInPixel = screenPos.xy * _ScreenParams.xy;

                // ディザリングテクスチャ用のUVを作成
                int ditherUV_x = (int)fmod(screenPosInPixel.x, PATTERN_ROW_SIZE);
                int ditherUV_y = (int)fmod(screenPosInPixel.y, PATTERN_ROW_SIZE);
                float dither = pattern[ditherUV_x, ditherUV_y];

                #ifdef _OUTLINE
                // 閾値が0以下なら描画しない
                clip(dither - _DitherLevel);
                #else
                clip(dither - 16);
                #endif
                return col;
            }
            ENDHLSL
        }

        //        Pass
        //        {
        //            Tags
        //            {
        //                "LightMode"="ShadowCaster"
        //            }
        //
        //            CGPROGRAM
        //            #pragma vertex vert
        //            #pragma fragment frag
        //            #pragma multi_compile_shadowcaster
        //
        //            #include "UnityCG.cginc"
        //
        //            struct appdata
        //            {
        //                float4 vertex : POSITION;
        //                float3 normal : NORMAL;
        //                float2 texcoord : TEXCOORD0;
        //            };
        //
        //            struct v2f
        //            {
        //                V2F_SHADOW_CASTER;
        //            };
        //
        //            v2f vert(appdata v)
        //            {
        //                v2f o;
        //                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
        //                return o;
        //            }
        //
        //            fixed4 frag(v2f i) : SV_Target
        //            {
        //                SHADOW_CASTER_FRAGMENT(i)
        //            }
        //            ENDCG
        //        }

        // アウトラインの描画
        //        Pass
        //        {
        //            Tags{ "LightMode" = "UniversalForward" }
        //            
        //            HLSLPROGRAM
        //            #pragma vertex vert
        //            #pragma fragment frag
        //
        //            v2f vert (appdata v)
        //            {
        //                v2f o;
        //                // アウトラインの分だけ拡大
        //                o.vertex = TransformObjectToHClip(v.vertex * (1 +_OutlineWidth));
        //                return o;
        //            }
        //
        //            half4 frag(v2f i) : SV_Target
        //            {
        //                return _OutlineColor;
        //            }
        //            ENDHLSL
        //        }
    }
    Fallback "Diffuse"
}