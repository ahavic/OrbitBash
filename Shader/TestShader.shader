﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TestShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appData {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos: SV_POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            float4 _LightColor0;
            fixed4 _Color;
            sampler2D _MainTex;

            v2f vert(appData IN)
            {
                v2f OUT;
                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.normal = mul(float4(IN.normal, 0.0), unity_ObjectToWorld).xyz;
                OUT.texcoord = IN.texcoord;
                return OUT;
            }

            fixed4 frag(v2f IN) : COLOR
            {
                fixed4 texColor = tex2D(_MainTex, IN.texcoord);
                float3 normalDirection = normalize(IN.normal);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 diffuse = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));

                return texColor * _Color * float4(diffuse,1);
                
            }

            ENDCG
        }
    }
}
