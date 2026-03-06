Shader "URP/Particles/Blood Effect"
{
    Properties
    {
        [HDR]_BaseColor ("Base Color", Color) = (1,1,1,1)

        _MainTex ("Mask Texture (Blood)", 2D) = "white" {}
        _MaskStr ("Mask Strength", Range(0,1)) = 0.85
        _ChannelMask ("Mask Channel (RGBA)", Vector) = (1,0,0,0)

        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseAlphaStr ("Noise Strength", Range(0,1)) = 0.75
        _ChannelMask2 ("Noise Channel (RGBA)", Vector) = (1,0,0,0)

        _AlphaMin ("Alpha Clip Min", Range(0,1)) = 0.05
        _AlphaSoft ("Alpha Softness", Range(0.0001,1)) = 0.05

        _NoisePan ("Noise Pan XY", Vector) = (0.25, 0.15, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;     // particle color + alpha (lifetime)
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float2 uvNoise     : TEXCOORD1;
                float4 color       : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;

                float4 _MainTex_ST;
                float  _MaskStr;
                float4 _ChannelMask;

                float4 _NoiseTex_ST;
                float  _NoiseAlphaStr;
                float4 _ChannelMask2;

                float  _AlphaMin;
                float  _AlphaSoft;

                float4 _NoisePan;
            CBUFFER_END

            TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                float2 nUV = TRANSFORM_TEX(IN.uv, _NoiseTex);
                nUV += _Time.y * _NoisePan.xy;
                OUT.uvNoise = nUV;

                OUT.color = IN.color * _BaseColor;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 col = max(0.001h, IN.color);

                // Mask
                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                mask = saturate(lerp(1, mask, _MaskStr));
                half maskA = saturate(dot(mask, _ChannelMask));

                // Noise
                half4 n4 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uvNoise);
                half n = saturate(dot(n4, _ChannelMask2));
                n = saturate(lerp(1, n, _NoiseAlphaStr));

                half preA = maskA * n * col.a;

                // Soft clip
                half a = saturate((preA - _AlphaMin) / _AlphaSoft);
                col.a = a;

                // darken a bit
                col.rgb *= 0.9h;

                return col;
            }
            ENDHLSL
        }
    }
}