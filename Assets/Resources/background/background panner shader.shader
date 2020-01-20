// Shader created with Shader Forge Beta 0.15 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.15;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-2-RGB,emission-743-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33251,y:32760,ptlb:node_2,tex:7d0b7997bba18da43887dc6924dd71df,ntxv:0,isnm:False|UVIN-16-UVOUT;n:type:ShaderForge.SFN_Panner,id:16,x:33515,y:32717,spu:0,spv:1|UVIN-633-UVOUT,DIST-739-OUT;n:type:ShaderForge.SFN_TexCoord,id:633,x:33706,y:32717,uv:0;n:type:ShaderForge.SFN_Time,id:738,x:33853,y:32848;n:type:ShaderForge.SFN_Multiply,id:739,x:33683,y:32892|A-738-TSL,B-740-OUT;n:type:ShaderForge.SFN_Vector1,id:740,x:33921,y:33000,v1:0.25;n:type:ShaderForge.SFN_Add,id:743,x:32955,y:32888|A-2-RGB,B-747-RGB;n:type:ShaderForge.SFN_Multiply,id:745,x:33486,y:33014|A-16-UVOUT,B-746-OUT;n:type:ShaderForge.SFN_Vector1,id:746,x:33694,y:33034,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:747,x:33129,y:33015,ptlb:node_747,tex:7d0b7997bba18da43887dc6924dd71df,ntxv:0,isnm:False|UVIN-748-UVOUT;n:type:ShaderForge.SFN_Panner,id:748,x:33306,y:33070,spu:0.15,spv:1|UVIN-745-OUT,DIST-750-OUT;n:type:ShaderForge.SFN_Vector1,id:749,x:33936,y:33067,v1:0.15;n:type:ShaderForge.SFN_Multiply,id:750,x:33650,y:33162|A-738-TSL,B-749-OUT;proporder:2-747;pass:END;sub:END;*/

Shader "Shader Forge/background panner shader" {
    Properties {
        _node2 ("node_2", 2D) = "white" {}
        _node747 ("node_747", 2D) = "white" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node2; uniform float4 _node2_ST;
            uniform sampler2D _node747; uniform float4 _node747_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float4 node_738 = _Time + _TimeEditor;
                float2 node_16 = (i.uv0.rg+(node_738.r*0.25)*float2(0,1));
                float4 node_2 = tex2D(_node2,TRANSFORM_TEX(node_16, _node2));
                float3 lightFinal = (node_2.rgb+tex2D(_node747,TRANSFORM_TEX(((node_16*0.5)+(node_738.r*0.15)*float2(0.15,1)), _node747)).rgb)+UNITY_LIGHTMODEL_AMBIENT.xyz;
/// Final Color:
                return fixed4(lightFinal,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
