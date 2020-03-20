// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-108-OUT,normal-230-RGB,transm-105-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33400,y:32327,ptlb:Diffuse,tex:0b6f2f5ca338bac45931987289c21bd4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Time,id:94,x:33886,y:32616;n:type:ShaderForge.SFN_Append,id:95,x:33886,y:32743|A-96-OUT,B-97-OUT;n:type:ShaderForge.SFN_Slider,id:96,x:34026,y:32709,ptlb:U speed,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:97,x:34051,y:32803,ptlb:V speed,min:0,cur:2,max:2;n:type:ShaderForge.SFN_TexCoord,id:98,x:34237,y:32886,uv:0;n:type:ShaderForge.SFN_Multiply,id:99,x:33732,y:32708|A-94-T,B-95-OUT;n:type:ShaderForge.SFN_Add,id:100,x:33562,y:32804|A-99-OUT,B-98-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:102,x:33389,y:32804,ptlb:gradient,tex:d2a4400aca181724cbb09df3d2a5d862,ntxv:0,isnm:False|UVIN-100-OUT;n:type:ShaderForge.SFN_Multiply,id:105,x:33412,y:32555|A-106-RGB,B-102-RGB;n:type:ShaderForge.SFN_Color,id:106,x:33619,y:32416,ptlb:Heat Color,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Add,id:108,x:33212,y:32431|A-2-RGB,B-105-OUT;n:type:ShaderForge.SFN_Tex2d,id:230,x:33073,y:32742,ptlb:Normal,tex:92984929e344c9041b92bb78f1e3021b,ntxv:3,isnm:True;proporder:2-96-97-102-106-230;pass:END;sub:END;*/

Shader "Shader Forge/cooler_pipe_shader" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Uspeed ("U speed", Range(0, 2)) = 0
        _Vspeed ("V speed", Range(0, 2)) = 0
        _gradient ("gradient", 2D) = "white" {}
        _HeatColor ("Heat Color", Color) = (1,0,0,1)
        _Normal ("Normal", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Uspeed;
            uniform float _Vspeed;
            uniform sampler2D _gradient; uniform float4 _gradient_ST;
            uniform float4 _HeatColor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0.xy, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = pow( max(0.0, NdotL ), 1 );
                float4 node_94 = _Time + _TimeEditor;
                float4 node_102 = tex2D(_gradient,TRANSFORM_TEX(((node_94.g*float2(_Uspeed,_Vspeed))+i.uv0.rg), _gradient));
                float3 node_105 = (_HeatColor.rgb*node_102.rgb);
                float3 backLight = pow( max(0.0, -NdotL ), 1 ) * node_105;
                float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
                float3 lightFinal = diffuse * (tex2D(_Diffuse,TRANSFORM_TEX(i.uv0.xy, _Diffuse)).rgb+node_105);
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
        Pass {
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Uspeed;
            uniform float _Vspeed;
            uniform sampler2D _gradient; uniform float4 _gradient_ST;
            uniform float4 _HeatColor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0.xy, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = pow( max(0.0, NdotL ), 1 );
                float4 node_94 = _Time + _TimeEditor;
                float4 node_102 = tex2D(_gradient,TRANSFORM_TEX(((node_94.g*float2(_Uspeed,_Vspeed))+i.uv0.rg), _gradient));
                float3 node_105 = (_HeatColor.rgb*node_102.rgb);
                float3 backLight = pow( max(0.0, -NdotL ), 1 ) * node_105;
                float3 diffuse = (forwardLight+backLight) * attenColor;
                float3 lightFinal = diffuse * (tex2D(_Diffuse,TRANSFORM_TEX(i.uv0.xy, _Diffuse)).rgb+node_105);
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
