Shader "UI/FX_holeRead"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comp", Float) = 8
        _StencilOp ("Stencil Op", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        ColorMask RGBA

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION; 
                float4 color : COLOR;
            };
            struct v2f {
                float4 pos : SV_POSITION; 
                float4 color : COLOR;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half alpha = 0.2;
                alpha *= i.color.a;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}