Shader "Hidden/Kaleidoscope"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Segments ("Segments", Range(2, 24)) = 6
        _CenterX ("Center X", Range(0,1)) = 0.5
        _CenterY ("Center Y", Range(0,1)) = 0.5
        _Rotation ("Rotation (radians)", Float) = 0
        _Zoom ("Zoom", Range(0.1, 5)) = 1
        _CenterRadius ("Center Radius (untouched)", Range(0, 0.5)) = 0.1
        _FeatherWidth ("Feather Width", Range(0.001, 0.3)) = 0.05
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
            float _Segments;
            float _CenterX;
            float _CenterY;
            float _Rotation;
            float _Zoom;
            float _CenterRadius;
            float _FeatherWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(_CenterX, _CenterY);
                float2 uv = i.uv - center;

                // Correct for aspect ratio so wedges aren't stretched
                uv.x *= _ScreenParams.x / _ScreenParams.y;

                float radius = length(uv) / _Zoom;
                float angle = atan2(uv.y, uv.x) + _Rotation;

                float wedgeAngle = 6.2831853 / _Segments; // 2*PI / segments
                angle = fmod(angle, wedgeAngle);
                if (angle < 0) angle += wedgeAngle;

                // Mirror every other wedge so it tiles symmetrically
                angle = abs(angle - wedgeAngle * 0.5) ;

                float2 sampleUV;
                sampleUV.x = radius * cos(angle);
                sampleUV.y = radius * sin(angle);

                // Undo aspect correction, recenter
                sampleUV.x /= _ScreenParams.x / _ScreenParams.y;
                sampleUV += center;

                sampleUV = saturate(sampleUV);

                // Blend back to the untouched original UV near the center,
                // so a small circle in the middle stays normal/unwarped.
                float blend = smoothstep(_CenterRadius, _CenterRadius + _FeatherWidth, radius);

                float2 finalUV = lerp(i.uv, sampleUV, blend);

                return tex2D(_MainTex, finalUV);
            }
            ENDCG
        }
    }
}
