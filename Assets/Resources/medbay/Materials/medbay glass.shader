// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,uamb:True,ufog:True,aust:True,igpj:True,qofs:0,lico:1,qpre:3,flbk:,rntp:2,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-6-RGB,alpha-21-OUT,emission-17-OUT;n:type:ShaderForge.SFN_Fresnel,id:2,x:33357,y:33016|NRM-3-OUT,EXP-4-OUT;n:type:ShaderForge.SFN_NormalVector,id:3,x:33567,y:33016,pt:False;n:type:ShaderForge.SFN_Vector1,id:4,x:33548,y:33259,v1:2;n:type:ShaderForge.SFN_Color,id:6,x:33062,y:32376,ptlb:Diffuse,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Cubemap,id:7,x:33517,y:32780,ptlb:cubem,cube:279e2de22fffd294bace4aaf626c6ae9,pvfc:0|DIR-12-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:12,x:33710,y:32747;n:type:ShaderForge.SFN_Add,id:17,x:33112,y:32866|A-18-OUT,B-7-RGB;n:type:ShaderForge.SFN_Multiply,id:18,x:33206,y:33080|A-2-OUT,B-19-RGB;n:type:ShaderForge.SFN_Color,id:19,x:33370,y:33167,ptlb:fresnel clor,c1:0.120891,c2:0.6323529,c3:0.4459322,c4:1;n:type:ShaderForge.SFN_Color,id:20,x:33287,y:32591,ptlb:Alpha,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:21,x:33095,y:32670|A-20-A,B-7-R;proporder:6-7-19-20;pass:END;sub:END;*/

Shader "Shader Forge/medbay glass" {
    Properties {
        _Diffuse ("Diffuse", Color) = (0.5,0.5,0.5,1)
        _cubem ("cubem", Cube) = "_Skybox" {}
        _fresnelclor ("fresnel clor", Color) = (0.120891,0.6323529,0.4459322,1)
        _Alpha ("Alpha", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Diffuse;
            uniform samplerCUBE _cubem;
            uniform float4 _fresnelclor;
            uniform float4 _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float4 node_7 = texCUBE(_cubem,viewReflectDirection);
                float3 emissive = ((pow(1.0-max(0,dot(i.normalDir, viewDirection)),2.0)*_fresnelclor.rgb)+node_7.rgb);
                float3 lightFinal = diffuse * _Diffuse.rgb + emissive;
/// Final Color:
                return fixed4(lightFinal,(_Alpha.a+node_7.r));
            }
            ENDCG
        }
        Pass {
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Diffuse;
            uniform samplerCUBE _cubem;
            uniform float4 _fresnelclor;
            uniform float4 _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 lightFinal = diffuse * _Diffuse.rgb;
                float4 node_7 = texCUBE(_cubem,viewReflectDirection);
/// Final Color:
                return fixed4(lightFinal,(_Alpha.a+node_7.r));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
