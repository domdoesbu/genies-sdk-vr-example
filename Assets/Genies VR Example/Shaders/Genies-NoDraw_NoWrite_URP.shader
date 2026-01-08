Shader "Hidden/Genies/NoDraw_NoWrite_URP"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Name "NoWrite"
            ZWrite Off
            ZTest Always
            Cull Off
            ColorMask 0
        }
    }
}
