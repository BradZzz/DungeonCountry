// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TextShader" {
Properties {
_Color ("Main Color", Color) = (1,1,1,0.5)
_SpecColor ("Spec Color", Color) = (1,1,1,1)
_Emission ("Emmisive Color", Color) = (0,0,0,0)
_Shininess ("Shininess", Range (0.01, 1)) = 0.7
_MainTex ("Base (RGB)", 2D) = "white" { }
_OutlineColor ("Outline Color", Color) = (0,0,0,1)
_Outline ("Outline Width", Float) = 0.005
}

SubShader {
Pass {
Material {
Diffuse [_Color]
Ambient [_Color]
Shininess [_Shininess]
Specular [_SpecColor]
Emission [_Emission]
}
Lighting On
SeparateSpecular On
SetTexture [_MainTex] {
constantColor [_Color]
Combine texture * primary DOUBLE, texture * constant
}
}
Pass {
Name "Outline"
Tags { "LightMode" = "Always"}
Lighting Off
Cull Front
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

struct v2f
{
float4 pos : SV_POSITION;
};

float _Outline;

v2f vert(appdata_base v) {
v2f o;
o.pos = UnityObjectToClipPos(v.vertex);
float3 norm = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
norm.x *= UNITY_MATRIX_P[0][0];
norm.y *= UNITY_MATRIX_P[1][1];
o.pos.xy += norm.xy * o.pos.z * _Outline;
return o;
}

float4 _OutlineColor;

float4 frag(v2f i) :COLOR {
return float4(_OutlineColor.rgb,0);
}
ENDCG
}
}
} 