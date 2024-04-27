#pragma vertex vert
#pragma fragment frag

#pragma multi_compile _EnableOutLine

            
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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

    #if defined _EnableOutLine
                // 閾値が0以下なら描画しない
                clip(dither - _DitherLevel);
    #else
    clip(dither - 16);
    #endif
    return col;
}
