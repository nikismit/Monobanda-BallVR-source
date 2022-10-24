Shader "Custom/Pulsing_Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _HDRColorOne("HDRColor", Color) = (1,1,1,1)
        _HDRColorTwo("HDRColor", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        //float amount = sin(time) * 0.5 + 0.5; // amount will be in range [0..1]  float3 color = lerp(float3(1,0,0), originalColor, amount);


        struct Input
        {
            float2 uv_MainTex;
        };

        double sinus(double theta)
        {
            double PI = 3.14159265358979323846;
            double a = theta + PI / 2.0, b = PI * 2.0;
            theta = ((a > 0) ? a - b * ((int)(a / b)) : (-a + b * (((int)(a / b))))) - PI / 2.0;
            if (theta > PI / 2.0)
                theta = PI - theta;
            double x3 = (theta * theta * theta);
            double x5 = (x3 * theta * theta);
            return theta - x3 / 6.0 + x5 / 120.0;
        }

        double mod(double a, double b)
        {
            return (a > 0) ? a - b * ((int)(a / b)) : -(-a + b * (((int)(a / b))));
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _HDRColorOne;
        fixed4 _HDRColorTwo;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float4 TriangleWave(float4 In)
        {
            return 2.0 * abs(2 * (In - floor(0.5 + In))) - 1.0;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //float amount = _SinTime * 0.5 + 0.5; // amount will be in range [0..1]  float3 color = lerp(float3(1,0,0), originalColor, amount);
            float4 amount = TriangleWave(_Time * 2); // amount will be in range [0..1]  float3 color = lerp(float3(1,0,0), originalColor, amount);
            //float4 amount = float4(_SinTime.x * 4, _SinTime.y * 5, _SinTime.y * 5,0); // amount will be in range [0..1]  float3 color = lerp(float3(1,0,0), originalColor, amount);
            //float amount = sin(_Time.x) * 0.5 + 0.5; // amount will be in range [0..1]  float3 color = lerp(float3(1,0,0), originalColor, amount);
            //float amount = _Time.x; // amount will be in range [0..1]  float3 color = lerp(float3(1,0,0), originalColor, amount);

            //_Time.x

            float4 originalColor = _Color;
            //float3 color = lerp(float3(1, 0, 0), originalColor, amount);
            //fixed4 color = lerp(originalColor, float4(1, 0.2, 0.2, 0.2), amount * 1.5);
            fixed4 color = lerp(_HDRColorOne, _HDRColorTwo, amount * 1.5);

            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
