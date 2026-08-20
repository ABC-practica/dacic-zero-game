
Shader "ShaderMan/Clouds"
{

    Properties{
        _Color("Sky Color", Color) = (0.6, 0.7, 0.8, 1)
        _CamPosition("Camera Position", Vector) = (0,0,0,0)
        _CamRotation("Camera Rotation", Vector) = (0,0,0,0)
        _Rotation("Camera Rotation", float) = 0
        _Iterations("Cloud Iterations", Range(100,750)) = 170
        _Brightness("Brightness", Range(0,10)) = 1
        _Density("Density", Range(0,100)) = 1
        _Zoom("Zoom", Range(0,5)) = 2
        _Scale("Scale", Range(0.01,10)) = 1
        _Height("Height", Range(0.01,10)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
        
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
            
                struct VertexInput {
                    fixed4 vertex : POSITION;
                    fixed2 uv:TEXCOORD0;
                    fixed4 tangent : TANGENT;
                    fixed3 normal : NORMAL;
                };
            
                struct VertexOutput {
                    fixed4 pos : SV_POSITION;
                    fixed2 uv:TEXCOORD0;
                };
                
                float4 _CamPosition;
                float4 _CamRotation;
                float4 _Color;
                float _Rotation;
                float _Iterations;
                float _Brightness;
                float _Density;
                float _Zoom;
                float _Scale;
                float _Height;
                
                fixed hash(in fixed2 uv)
                {
                    fixed2 res = frac(uv * fixed2(821.143, 173.321));
                    res += dot(res, res+23.13);
                    return frac(res.x*res.y);	
                }
                
                fixed noise(in fixed2 uv)
                {
                    fixed2 ipos = floor(uv);
                    fixed2 fpos = frac(uv);
                    
                    fixed a = hash(ipos + fixed2(0, 0));
                    fixed b = hash(ipos + fixed2(1, 0));
                    fixed c = hash(ipos + fixed2(0, 1));
                    fixed d = hash(ipos + fixed2(1, 1));
                    
                    fixed2 t = smoothstep(0.0, 1.0, fpos);
                    return lerp(lerp(a, b, t.x), lerp(c, d, t.x), t.y);
                }
                
                fixed fbm(in fixed2 p)
                {
                    fixed res = 0.0;
                    fixed amp = 0.5;
                    fixed freq = 2.0;
                    for (int i = 0; i < 4; ++i)
                    {
                        res += amp*noise(freq*p);
                        amp *= 0.5;
                        freq *= 2.0;
                    }
                    return res;
                }
            
                VertexOutput vert (VertexInput v)
                {
                    VertexOutput o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv = v.uv;
                    //VertexFactory
                    return o;
                }
                
                fixed4 frag(VertexOutput i) : SV_Target
                {
                
                    fixed2 uv = (2.0 * i.uv - 1) / 1;
                    
                    fixed3 ro = fixed3(0, 0.8, _Time.y) + _CamPosition;
                    fixed3 at = ro + fixed3(0, -0.2, 1) + _CamRotation;
                    
                    fixed3 cam_z = normalize(at - ro);
                    fixed3 cam_x = normalize(cross(fixed3(0,1,0), cam_z)) + float3(0,_Rotation,0);
                    fixed3 cam_y = cross(cam_z, cam_x);
                    fixed3 rd = normalize(uv.x * cam_x + uv.y * cam_y + _Zoom * cam_z);
                    
                    fixed3 sky_col = _Color;
                    fixed3 col = sky_col;
                    col -= 0.7*rd.y;
                    
                    for (fixed i = _Iterations; i > 100.0; --i)
                    {
                        fixed3 p = ro + 0.05*i*rd;
                        fixed f = p.y - 1.2*fbm(0.6*p.xz*_Scale)*_Height;
                        fixed density = -f;
                        if (density > 0.0)
                        {
                            col = lerp(col, 1.0 - _Brightness*density*sky_col.bgr, min(1.0, _Density*density*0.4));
                        }
                    }
                    return float4(col, 1);
                }
            ENDCG
        }
    }
}
