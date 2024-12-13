Shader "Custom/Gradient"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GradientStart ("Gradient Start", Float) = 0.0
        _GradientEnd ("Gradient End", Float) = 1.0
        _ParentTransform ("Parent Transform", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float4 worldPos : TEXCOORD0;
            };

            // Properties
            float _GradientStart;
            float _GradientEnd;
            float4 _ParentTransform;

            // Vertex Shader
            v2f vert(appdata v)
            {
                v2f o;
                // Apply parent transformation (world matrix)
                float4 worldPosition = mul(unity_ObjectToWorld, v.vertex);
                worldPosition.xyz += _ParentTransform.xyz; // Assuming the parent translation

                o.pos = mul(UNITY_MATRIX_VP, worldPosition);
                o.color = v.color;

                // Pass world position to fragment shader
                o.worldPos = worldPosition;

                return o;
            }

            // Fragment Shader
            half4 frag(v2f i) : SV_Target
            {
                // Calculate distance from camera to current object
                float3 cameraPosition = _WorldSpaceCameraPos;
                float distance = length(i.worldPos.xyz - cameraPosition);

                // Create gradient based on distance
                float gradientFactor = saturate((distance - _GradientStart) / (_GradientEnd - _GradientStart));
                half4 finalColor = i.color * gradientFactor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
