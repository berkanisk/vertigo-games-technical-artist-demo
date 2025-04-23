Shader "Custom/OuterFlow"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveFrequency ("Wave Frequency", Float) = 4.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.05

        _FadeWidthX ("Edge Fade X", Float) = 0.5
        _FadeWidthY ("Edge Fade Y", Float) = 0.5

        _FlickerSpeed ("Flicker Speed", Float) = 4.0
        _FlickerStrength ("Flicker Strength", Float) = 0.3
        _Alpha ("Alpha", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _WaveSpeed;
            float _WaveFrequency;
            float _WaveAmplitude;

            float _FadeWidthX;
            float _FadeWidthY;

            float _FlickerSpeed;
            float _FlickerStrength;
            float _Alpha;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float wave = sin((uv.y * _WaveFrequency + _Time.y * _WaveSpeed)) * _WaveAmplitude;
                uv.x += wave;
                
                fixed4 col = tex2D(_MainTex, uv) * _Color;
                
                float edgeX = smoothstep(0.0, _FadeWidthX, uv.x) * (1.0 - smoothstep(1.0 - _FadeWidthX, 1.0, uv.x));
                
                float edgeY = smoothstep(0.0, _FadeWidthY, uv.y) * (1.0 - smoothstep(1.0 - _FadeWidthY, 1.0, uv.y));

                float edgeFade = edgeX * edgeY;
                
                float flicker = abs(sin(_Time.y * _FlickerSpeed)) * _FlickerStrength + (1.0 - _FlickerStrength);

                col.a *= edgeFade * flicker * _Alpha;

                return col;
            }

            ENDHLSL
        }
    }
}
