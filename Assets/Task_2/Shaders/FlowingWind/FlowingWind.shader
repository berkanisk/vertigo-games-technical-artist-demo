Shader "Custom/FlowingWind"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _WaveAmplitude ("Wave Amplitude", Float) = 0.05
        _WaveFrequency ("Wave Frequency", Float) = 10.0
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _FresnelPower ("Fresnel Power", Float) = 4.0
        _Intensity ("Intensity", Float) = 1.0
        _FadeSpeed ("Fade Scroll Speed", Float) = 1.0
        _FadeWidth ("Fade Width", Float) = 0.4
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _Color;
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _FresnelPower;
            float _Intensity;
            float _FadeSpeed;
            float _FadeWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float wave = sin((uv.x * _WaveFrequency) + (_Time.y * _WaveSpeed)) * _WaveAmplitude;
                uv.y += wave;
                
                float fadePos = frac(1.0 - uv.x + _Time.y * _FadeSpeed);
                float fadeMask = smoothstep(0.0, _FadeWidth, fadePos) * (1.0 - smoothstep(1.0 - _FadeWidth, 1.0, fadePos));

                
                float noise = tex2D(_NoiseTex, uv).r;
                float mask = tex2D(_MainTex, uv).a;
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnel = pow(1.0 - saturate(dot(viewDir, normalize(i.worldNormal))), _FresnelPower);

                float alpha = noise * mask * fresnel * _Intensity * fadeMask;

                return float4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
