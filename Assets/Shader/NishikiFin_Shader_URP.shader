Shader "Nishiki/NishikiFin_Shader_URP"
{
    Properties
    {
        [MaterialToggle] _CustomizeColor("CustomizeColor",int) = 0
        _CustomColor("CustomColor",color) = (1,1,1,1)
        [Space(10)]

        _Metallic("MetallicValue",Range(0,1)) = 0
        _Roughness("RoughnessValue",Range(0,2)) = 0.6
        [Space(30)]

        [NoScaleOffset] _AlphaMap("AlphaMap",2D) = "white"
        [NoScaleOffset] _BaseColorMap("BaseColorMap",2D) = "white"
        [NoScaleOffset] _NormalMap("NormalMap",2D) = "bump"
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                sampler2D _AlphaMap;
                sampler2D _BaseColorMap;
                sampler2D _NormalMap;

                int _CustomizeColor;

                half3 _CustomColor;
                //half _Metallic;

                half _Roughness;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                // o.pos = TransformObjectToHClip(v.vertex);
                // o.normal = TransformObjectToHClip(v.vertex);
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

            half4 frag(v2f i) : COLOR
            {
                // float2 uv = i.uv;
                //
                // half3 color = _CustomizeColor ? _CustomColor : tex2D(_BaseColorMap, uv);
                //
                // return float4(color, 1.0);

                UNITY_SETUP_INSTANCE_ID(i);

                half roughness = _Roughness;

                float3 lightDir = normalize(_MainLightPosition.xyz);
                half3 lightColor = _MainLightColor.xyz;

                half3 normalmap = UnpackNormal(tex2D(_NormalMap, i.uv));

                // 法線を合成
                float3 normal = (i.tangent * normalmap.x * roughness) + (i.binormal * normalmap.y * roughness) + (i.
                    normal * normalmap.z);

                float t = dot(normal, lightDir);
                t = max(0, t); // t >= 0にする
                // 内積が0に近いほど黒く
                float3 diffuseLight = lightColor * t;

                // ライト方向と法線ベクトルから反射ベクトルを計算
                float3 reflectVec = reflect(-lightDir, normal);
                float3 eyeVec = normalize(GetCameraPositionWS() - i.worldPos) * tex2D(_AlphaMap, i.uv); // 視線ベクトル

                t = dot(reflectVec, eyeVec);
                t = max(0, t); // t >= 0にする
                //t = pow(t, _Specular);

                float3 specularLight = lightColor * t;

                Light light = GetMainLight(i.shadowCoord);
                half shadow = light.shadowAttenuation * light.distanceAttenuation;

                half4 color = _CustomizeColor ? half4(_CustomColor.rgb, 1) : tex2D(_BaseColorMap, i.uv);
                color = color * half4(shadow, shadow, shadow, 1);
                color.a *= tex2D(_AlphaMap, i.uv).r * shadow; // AlphaMapを考慮してアルファ値を設定
                color.xyz *= (specularLight + diffuseLight);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}