Shader "Custom/WelwiseCharacterShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Hue ("Hue Shift", Range(-1,1)) = 0
        _Saturation ("Saturation", Range(0,2)) = 1
        _Value ("Value", Range(0,2)) = 1
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _EmissionHue ("Emission Hue Shift", Range(-1,1)) = 0
        _EmissionSaturation ("Emission Saturation", Range(0,2)) = 1
        _EmissionValue ("Emission Value", Range(0,2)) = 1
        _Specular ("Specular Intensity", Range(0,1)) = 0.5

    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 tangent : TEXCOORD2;
                float3 bitangent : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 vertex : SV_POSITION;
                float3 lightDir : TEXCOORD5;
            };
            
            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _EmissionMap;
            float _Specular;
            float _Hue;
            float _Saturation;
            float _Value;
            float _EmissionHue;
            float _EmissionSaturation;
            float _EmissionValue;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.lightDir = normalize(UnityWorldSpaceLightDir(v.vertex));
                return o;
            }

            float3 AdjustSaturation(float3 color, float factor)
            {
                float luminance = dot(color, float3(0.299, 0.587, 0.114));
                return lerp(float3(luminance, luminance, luminance), color, factor);
            }
            
            float3 RGBtoHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }
            
            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                half4 emission = tex2D(_EmissionMap, i.uv);

                float emissionIntensity = dot(emission.rgb, float3(0.299, 0.587, 0.114));
                float saturationFactor = saturate(1.0 - emissionIntensity);
                
                col.rgb = AdjustSaturation(col.rgb, saturationFactor);

                float3 hsv = RGBtoHSV(col.rgb);
                hsv.x = frac(hsv.x + _Hue);
                hsv.y = saturate(hsv.y * _Saturation);
                hsv.z *= _Value;
                col.rgb = HSVtoRGB(hsv);
                
                float3 normalTex = UnpackNormal(tex2D(_NormalMap, i.uv));
                float3 normal = normalize(-normalTex.r * i.tangent + normalTex.g * i.bitangent + normalTex.b * i.normal);
                
                float diff = max(0, dot(normal, i.lightDir));
                
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * col.rgb;
                
                float3 viewDir = normalize(i.viewDir);
                float3 reflectDir = reflect(-i.lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0), 16) * _Specular;
                
               
                float3 emissionHSV = RGBtoHSV(emission.rgb);
                emissionHSV.x = frac(emissionHSV.x + _EmissionHue);
                emissionHSV.y = saturate(emissionHSV.y * _EmissionSaturation);
                emissionHSV.z *= _EmissionValue;
                emission.rgb = HSVtoRGB(emissionHSV);
                
                half3 finalColor = ambient + col.rgb * diff + spec + emission.rgb;
                return half4(finalColor, 1.0);
            }
            
            ENDCG
        }
    }
}
