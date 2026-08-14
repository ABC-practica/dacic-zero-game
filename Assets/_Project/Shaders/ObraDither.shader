Shader "Hidden/ObraDither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _DarkColor ("Dark Color", Color) = (0,0,0,1)
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Float) = 4
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

                // Tile the noise texture across the screen based on pixel resolution
                float2 noiseUV = i.uv * _MainTex_TexelSize.zw / _NoiseScale;
                float threshold = tex2D(_NoiseTex, noiseUV).r;

                fixed4 outCol = (lum > threshold) ? _LightColor : _DarkColor;
                return outCol;
            }
            ENDCG
        }
    }
}