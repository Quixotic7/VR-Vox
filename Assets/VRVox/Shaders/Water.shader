// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:Standard,lico:0,lgpr:1,nrmq:1,nrsp:0,limd:2,spmd:1,trmd:1,grmd:0,uamb:False,mssp:False,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:7,culm:0,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:5147,x:32862,y:32681,varname:node_5147,prsc:2|diff-4901-OUT,spec-8485-OUT,normal-325-OUT,alpha-1556-A;n:type:ShaderForge.SFN_Color,id:1556,x:31997,y:32514,ptovrint:False,ptlb:Main Color,ptin:_MainColor,varname:node_1556,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Panner,id:5947,x:30920,y:32957,varname:node_5947,prsc:2,spu:0.01,spv:0.04;n:type:ShaderForge.SFN_Multiply,id:2804,x:31188,y:33018,varname:node_2804,prsc:2|A-5947-UVOUT,B-6290-OUT;n:type:ShaderForge.SFN_Vector1,id:6290,x:30968,y:33182,varname:node_6290,prsc:2,v1:10;n:type:ShaderForge.SFN_ValueProperty,id:2627,x:32046,y:33225,ptovrint:False,ptlb:Amp1,ptin:_Amp1,varname:node_2627,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9819,x:32272,y:33036,varname:node_9819,prsc:2|A-1469-OUT,B-2627-OUT;n:type:ShaderForge.SFN_Vector3,id:5073,x:32468,y:33195,varname:node_5073,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Multiply,id:7545,x:32641,y:33071,varname:node_7545,prsc:2|A-9819-OUT,B-5073-OUT;n:type:ShaderForge.SFN_Tex2d,id:20,x:31607,y:33035,ptovrint:False,ptlb:Noise Tex,ptin:_NoiseTex,varname:node_20,prsc:2,tex:bb6f221dbc3a45f45a4deb23eb00f301,ntxv:0,isnm:False|UVIN-2804-OUT;n:type:ShaderForge.SFN_Noise,id:2392,x:31345,y:33222,varname:node_2392,prsc:2|XY-9893-UVOUT;n:type:ShaderForge.SFN_Add,id:1469,x:31849,y:33136,varname:node_1469,prsc:2|A-20-RGB,B-9476-OUT;n:type:ShaderForge.SFN_Multiply,id:9476,x:31636,y:33184,varname:node_9476,prsc:2|A-2392-OUT,B-8327-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8327,x:31462,y:33294,ptovrint:False,ptlb:Noise Scale,ptin:_NoiseScale,varname:node_8327,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:9893,x:31007,y:33238,varname:node_9893,prsc:2,uv:0;n:type:ShaderForge.SFN_Cubemap,id:7717,x:31997,y:32732,ptovrint:False,ptlb:CubeMap,ptin:_CubeMap,varname:node_7717,prsc:2,cube:4cccc89ee3a8d466ab2515cf392ee055,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:3218,x:32443,y:32612,varname:node_3218,prsc:2|A-1556-RGB,B-1756-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7162,x:32183,y:32845,ptovrint:False,ptlb:ReflectionAmount,ptin:_ReflectionAmount,varname:node_7162,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1756,x:32283,y:32710,varname:node_1756,prsc:2|A-7717-RGB,B-7162-OUT;n:type:ShaderForge.SFN_Tex2d,id:2441,x:32302,y:32397,ptovrint:False,ptlb:RippleTexture,ptin:_RippleTexture,varname:node_2441,prsc:2,tex:bb6f221dbc3a45f45a4deb23eb00f301,ntxv:0,isnm:False|UVIN-1539-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:8485,x:32515,y:32790,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_8485,prsc:2,glob:False,v1:0.5;n:type:ShaderForge.SFN_Panner,id:1539,x:31997,y:32327,varname:node_1539,prsc:2,spu:0.01,spv:0.005;n:type:ShaderForge.SFN_ValueProperty,id:4017,x:32364,y:32571,ptovrint:False,ptlb:NormalAmount,ptin:_NormalAmount,varname:node_4017,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:325,x:32580,y:32511,varname:node_325,prsc:2|A-2441-RGB,B-4017-OUT;n:type:ShaderForge.SFN_Multiply,id:4901,x:32642,y:32641,varname:node_4901,prsc:2|A-2441-RGB,B-3218-OUT;proporder:1556-2627-20-8327-7717-7162-8485-2441-4017;pass:END;sub:END;*/

Shader "VRVox/Water" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _Amp1 ("Amp1", Float ) = 1
        _NoiseTex ("Noise Tex", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float ) = 0
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
        _ReflectionAmount ("ReflectionAmount", Float ) = 1
        _Specular ("Specular", Float ) = 0.5
        _RippleTexture ("RippleTexture", 2D) = "white" {}
        _NormalAmount ("NormalAmount", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform float4 _MainColor;
            uniform samplerCUBE _CubeMap;
            uniform float _ReflectionAmount;
            uniform sampler2D _RippleTexture; uniform float4 _RippleTexture_ST;
            uniform float _Specular;
            uniform float _NormalAmount;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_1466 = _Time + _TimeEditor;
                float2 node_1539 = (i.uv0+node_1466.g*float2(0.01,0.005));
                float4 _RippleTexture_var = tex2D(_RippleTexture,TRANSFORM_TEX(node_1539, _RippleTexture));
                float3 normalLocal = (_RippleTexture_var.rgb*_NormalAmount);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = (_RippleTexture_var.rgb*(_MainColor.rgb*(texCUBE(_CubeMap,viewReflectDirection).rgb*_ReflectionAmount)));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse * _MainColor.a + specular;
                fixed4 finalRGBA = fixed4(finalColor,_MainColor.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "ShaderForgeMaterialInspector"
}
