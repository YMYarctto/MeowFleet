Shader "UI/FX_holeWrite"
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
        Tags { "Queue"="Transparent-10" "RenderType"="Transparent" }
        ZTest Always
        ZWrite Off
        ColorMask 0
        Blend SrcAlpha OneMinusSrcAlpha
        
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
            Comp Always
            Pass Replace
        }
        ColorMask 0

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}