Shader "Hidden/ObraDither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _DarkColor ("Dark Color", Color) = (0,0,0,1)
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Float) = 4
        _Softness ("Softness", Range(0.001, 0.2)) = 0.03
        _MinLum ("Min Luminance", Range(0,1)) = 0.0
        _Contrast ("Contrast", Range(1, 10)) = 1
        _DitherAmount ("Dither Amount", Range(0, 1)) = 1
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            sampler2D _NoiseTex;

            fixed4 _DarkColor;
            fixed4 _LightColor;
            float _NoiseScale;
            float _Softness;
            float _MinLum;
            float _Contrast;
            float _DitherAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = tex2D(_MainTex, i.uv).rgb;
                float lum = dot(col, float3(0.299, 0.587, 0.114));

                // Lift shadow floor so pure black areas can still show detail
                lum = lerp(_MinLum, 1.0, lum);

                // Push contrast so most pixels resolve clearly toward 0 or 1,
                // leaving only true midtones in the ambiguous "dither zone"
                lum = saturate((lum - 0.5) * _Contrast + 0.5);

                float2 noiseUV = i.uv * _MainTex_TexelSize.zw / _NoiseScale;
                float threshold = tex2D(_NoiseTex, noiseUV).r;

                // Pull the noise threshold toward a flat 0.5 as DitherAmount drops,
                // which shrinks how much the noise can actually flip a pixel —
                // less speckle, more solid fields of black/white.
                threshold = lerp(0.5, threshold, _DitherAmount);

                // Soft band around the threshold instead of a hard cutoff
                float edge = smoothstep(threshold - _Softness, threshold + _Softness, lum);
                fixed4 outCol = lerp(_DarkColor, _LightColor, edge);
                return outCol;
            }
            ENDCG
        }
    }
}
