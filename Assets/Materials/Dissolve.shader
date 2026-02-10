Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex ("NoiseTexforDissolve", 2D) = "white" {}
        _Cut("AlphaCut",Range(0,1))=0
        _OutColor("OutColor",Color)=(1,1,1,1)
        _OutThinkness("OutThinkness",Range(1,1.5))=1.5

    }
    SubShader
    {
        //투명이 가능한 모드, 불투명을 먼저 그리고 투명을 그리도록 함
        Tags { "RenderType"="Transparent"  "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade
                                                        //투명도함수사용
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NoiseTex;
        };

        float _Cut;//디졸브 정도
        float4 _OutColor;//두께 색상
        float _OutThinkness;//두께



        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;

            fixed4 noise = tex2D(_NoiseTex,IN.uv_NoiseTex);
            
            float alpha;
            if(noise.r>=_Cut)
                alpha=1;//불투명
            else
                alpha=0;//투명
            
            if(_Cut==1)
            {
                alpha=0;
            }

            float outline;
            if(noise.r>=_Cut * _OutThinkness)
                outline=0;
            else
                outline=1;
            
            o.Emission = outline*_OutColor.rgb;
            o.Alpha = alpha;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
