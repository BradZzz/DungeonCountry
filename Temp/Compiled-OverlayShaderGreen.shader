// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 4.9KB

// Skipping shader variants that would not be included into build of current scene.

Shader "Custom/OverlayShader" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
}
SubShader { 
 LOD 110
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }


 // Stats for Vertex shader:
 //       metal : 1 math
 //      opengl : 5 math, 1 texture
 // Stats for Fragment shader:
 //       metal : 5 math, 1 texture
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend DstColor SrcColor
  GpuProgramID 21239
Program "vp" {
SubProgram "opengl " {
// Stats: 5 math, 1 textures
"#version 120

#ifdef VERTEX

varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_COLOR = gl_Color;
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyw = xlv_COLOR.xyw;
  vec4 final_2;
  vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  tmpvar_1.z = 0.0;
  final_2.xyz = ((tmpvar_1.xyz * tmpvar_3.xyz) * 2.0);
  final_2.w = (xlv_COLOR.w * tmpvar_3.w);
  gl_FragData[0] = mix (vec4(0.5, 0.5, 0.5, 0.1), final_2, final_2.wwww);
}


#endif
"
}
SubProgram "metal " {
// Stats: 1 math
Bind "vertex" ATTR0
Bind "color" ATTR1
Bind "texcoord" ATTR2
ConstBuffer "$Globals" 64
Matrix 0 [glstate_matrix_mvp]
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float4 _glesColor [[attribute(1)]];
  float4 _glesMultiTexCoord0 [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half4 xlv_COLOR;
  half2 xlv_TEXCOORD0;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  float2 tmpvar_1;
  tmpvar_1 = _mtl_i._glesMultiTexCoord0.xy;
  half4 tmpvar_2;
  half2 tmpvar_3;
  tmpvar_2 = half4(_mtl_i._glesColor);
  tmpvar_3 = half2(tmpvar_1);
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * _mtl_i._glesVertex);
  _mtl_o.xlv_COLOR = tmpvar_2;
  _mtl_o.xlv_TEXCOORD0 = tmpvar_3;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform 	mat4x4 glstate_matrix_mvp;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
in  vec2 in_TEXCOORD0;
out vec4 vs_COLOR0;
out vec2 vs_TEXCOORD0;
vec4 u_xlat0;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
    u_xlat0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat0;
    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + u_xlat0;
    vs_COLOR0 = in_COLOR0;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    return;
}
#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform  sampler2D _MainTex;
in  vec4 vs_COLOR0;
in  vec2 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
vec4 u_xlat1;
lowp vec4 u_xlat10_1;
void main()
{
    u_xlat0.z = 0.0;
    u_xlat10_1 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat0.xyw = u_xlat10_1.xyw * vs_COLOR0.xyw;
    u_xlat0.xy = u_xlat0.xy * vec2(2.0, 2.0);
    u_xlat1 = u_xlat0 + vec4(-0.5, -0.5, -0.5, -0.100000001);
    SV_Target0 = u_xlat0.wwww * u_xlat1 + vec4(0.5, 0.5, 0.5, 0.100000001);
    return;
}
#endif
"
}
}
Program "fp" {
SubProgram "opengl " {
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 5 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  half4 xlv_COLOR;
  half2 xlv_TEXCOORD0;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]]
  ,   texture2d<half> _MainTex [[texture(0)]], sampler _mtlsmp__MainTex [[sampler(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  tmpvar_1.xyw = _mtl_i.xlv_COLOR.xyw;
  float4 final_2;
  float4 tex_3;
  half4 tmpvar_4;
  tmpvar_4 = _MainTex.sample(_mtlsmp__MainTex, (float2)(_mtl_i.xlv_TEXCOORD0));
  tex_3 = float4(tmpvar_4);
  tmpvar_1.z = half(0.0);
  final_2.xyz = (((float3)tmpvar_1.xyz * tex_3.xyz) * 2.0);
  final_2.w = ((float)_mtl_i.xlv_COLOR.w * tex_3.w);
  float4 tmpvar_5;
  tmpvar_5 = mix (float4(0.5, 0.5, 0.5, 0.1), final_2, final_2.wwww);
  _mtl_o._glesFragData_0 = half4(tmpvar_5);
  return _mtl_o;
}

"
}
SubProgram "glcore " {
"// shader disassembly not supported on glcore"
}
}
 }
}
Fallback "Diffuse"
}