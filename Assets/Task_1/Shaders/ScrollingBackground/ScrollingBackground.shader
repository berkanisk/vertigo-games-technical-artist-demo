Shader "Custom/ScrollingBackground"
{
    Properties
    {
        _MainTex ("Icon Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 0.1
        _ScrollDirection ("Scroll Direction", Vector) = (0.0, 1.0, 0.0, 0.0)
        _Rotation ("Icon Rotation", Range(0,360)) = 0
        _IconColor ("Icon Color Tint", Color) = (1, 1, 1, 1)
        _ColorCenter ("Gradient Center Color", Color) = (0.2, 0.5, 1, 1)
        _ColorEdge ("Gradient Edge Color", Color) = (0, 0.1, 0.3, 1)
        _GradientPower ("Gradient Spread", Range(0.1, 10)) = 2.5
        _IconBlend ("Icon Alpha", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScrollSpeed;
            float4 _ScrollDirection;
            float _Rotation;
            float4 _IconColor;
            float4 _ColorCenter;
            float4 _ColorEdge;
            float _GradientPower;
            float _IconBlend;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenUV : TEXCOORD1;
            };

            float2 RotateUV(float2 uv, float angle, float2 pivot)
            {
                float rad = radians(angle);
                float s = sin(rad);
                float c = cos(rad);
                uv -= pivot;
                float2 rotatedUV = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                return rotatedUV + pivot;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 offset = _Time.y * _ScrollSpeed * _ScrollDirection.xy;
                float2 uv = TRANSFORM_TEX(v.uv, _MainTex);
                uv += offset;
                uv = RotateUV(uv, _Rotation, float2(0.5, 0.5));
                o.uv = uv;
                o.screenUV = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.screenUV, center);
                float t = saturate(pow(dist * 2.0, _GradientPower));
                fixed4 grad = lerp(_ColorCenter, _ColorEdge, t);
                
                fixed4 icon = tex2D(_MainTex, i.uv) * _IconColor;
                icon.a *= _IconBlend;
                
                return lerp(grad, icon, icon.a);
            }
            ENDCG
        }
    }
}
