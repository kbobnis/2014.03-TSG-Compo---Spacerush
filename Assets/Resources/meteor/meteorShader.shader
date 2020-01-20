// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:2,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-2-RGB,spec-163-OUT,gloss-123-R,normal-22-RGB,emission-138-RGB;n:type:ShaderForge.SFN_Tex2d,id:2,x:33234,y:32569,ptlb:node_2,tex:f14062849a15afb4392f8f87aa6a6954,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:17,x:33362,y:32909,ptlb:node_17,tex:14118c1d114563f418b6b448b95da341,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:22,x:33221,y:33200,ptlb:node_22,tex:3c8fde208bef88f4ab38bf1daef9fd2b,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Color,id:123,x:33249,y:32745,ptlb:node_123,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Tex2d,id:138,x:33515,y:32875,ptlb:node_138,tex:559b896946656904c8c9b643ec1ea217,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:143,x:33201,y:32894|A-17-RGB,B-144-OUT;n:type:ShaderForge.SFN_Vector1,id:144,x:33493,y:33054,v1:2;n:type:ShaderForge.SFN_Color,id:162,x:33231,y:33045,ptlb:node_162,c1:0.7205882,c2:0.1271626,c3:0.1271626,c4:1;n:type:ShaderForge.SFN_Multiply,id:163,x:33011,y:32944|A-143-OUT,B-162-RGB;proporder:2-17-22-123-138-162;pass:END;sub:END;*/

Shader "Shader Forge/meteorShader" {
    Properties {
        _node2 ("node_2", 2D) = "white" {}
        _node17 ("node_17", 2D) = "white" {}
        _node22 ("node_22", 2D) = "bump" {}
        _node123 ("node_123", Color) = (0,0,0,1)
        _node138 ("node_138", 2D) = "white" {}
        _node162 ("node_162", Color) = (0.7205882,0.1271626,0.1271626,1)
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
            uniform sampler2D _node2; uniform float4 _node2_ST;
            uniform sampler2D _node17; uniform float4 _node17_ST;
            uniform sampler2D _node22; uniform float4 _node22_ST;
            uniform float4 _node123;
            uniform sampler2D _node138; uniform float4 _node138_ST;
            uniform float4 _node162;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_node22,TRANSFORM_TEX(i.uv0.xy, _node22))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float3 emissive = tex2D(_node138,TRANSFORM_TEX(i.uv0.xy, _node138)).rgb;
///////// Gloss:
                float gloss = exp2(_node123.r*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specular = attenColor * ((tex2D(_node17,TRANSFORM_TEX(i.uv0.xy, _node17)).rgb*2.0)*_node162.rgb) * pow(max(0,dot(halfDirection,normalDirection)),gloss);
                float3 lightFinal = diffuse * tex2D(_node2,TRANSFORM_TEX(i.uv0.xy, _node2)).rgb + specular + emissive;
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
            uniform sampler2D _node2; uniform float4 _node2_ST;
            uniform sampler2D _node17; uniform float4 _node17_ST;
            uniform sampler2D _node22; uniform float4 _node22_ST;
            uniform float4 _node123;
            uniform sampler2D _node138; uniform float4 _node138_ST;
            uniform float4 _node162;
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
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = UnpackNormal(tex2D(_node22,TRANSFORM_TEX(i.uv0.xy, _node22))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(_node123.r*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specular = attenColor * ((tex2D(_node17,TRANSFORM_TEX(i.uv0.xy, _node17)).rgb*2.0)*_node162.rgb) * pow(max(0,dot(halfDirection,normalDirection)),gloss);
                float3 lightFinal = diffuse * tex2D(_node2,TRANSFORM_TEX(i.uv0.xy, _node2)).rgb + specular;
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
