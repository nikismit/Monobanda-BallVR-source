Shader "Unlit/TrailVFX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorOne("HDRColor", Color) = (1,1,1,1)
        _ColorTwo("HDRColor", Color) = (1,1,1,1)

        _texSpeed("texSpeed", Float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            //#pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            //#include "noiseSimplex.cginc"

            fixed4 _ColorOne;
            fixed4 _ColorTwo;
            float2 _texSpeed;
            //float2 one = (1, 1);
            /*
            float lerp(float a, float b, float t) {
                return a + (b - a) * t;
            };
            */

            //--Noise--

            struct Input {
                float3 worldPos;
            };

            float rand(float3 value) {
                //make value smaller to avoid artefacts
                float3 smallValue = sin(value);
                //get scalar value from 3d vector
                float random = dot(smallValue, float3(12.9898, 78.233, 37.719));
                //make value more random by making it bigger and then taking teh factional part
                random = frac(sin(random) * 143758.5453);
                return random;
            };

            //----

            void OneMinus(float4 In, out float4 Out)
            {
                Out = 1 - In;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = (v.uv * _MainTex_ST.xy) + float2(5, 0) * _Time.y;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            { 
                //float ns = snoise(v);

                //float2 newUV : TEXCOORD0;

                //float4 uvOut = OneMinus(i.uv);
                float4 colorOut = lerp(_ColorOne, _ColorTwo, i.uv.x);

                float2 speed = _Time.xy * _texSpeed;

                //float noise = rand(Input);

                float subtractCol = i.uv.x - 0.75;

                //float offset = TilingAndOffset(one, speed)



                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv) * _ColorOne;
                fixed4 col = tex2D(_MainTex, colorOut) * subtractCol;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
