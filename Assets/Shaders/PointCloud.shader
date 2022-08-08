// Don't remove the following line. It is used to bypass Unity
// upgrader change. This is necessary to make sure the shader 
// continues to compile on Unity 5.2
// Created by Team Tango
// Modified by Nick Castellucci to preserve color on iOS for
// LiveScan3D
// UNITY_SHADER_NO_UPGRADE
Shader "Tango/PointCloud" {
	Properties{
			point_size("Point Size", Float) = 5.0
	}
		SubShader{
		   Pass {
			  CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag

			  #include "UnityCG.cginc"

			  struct appdata
			  {
				 float4 vertex : POSITION;
				 // add color here because livescan3d transmits it
				 float4 color : COLOR;
			  };

			  struct v2f
			  {
				 float4 vertex : SV_POSITION;
				 float4 color : COLOR;
				 float size : PSIZE;
			  };

			  float4x4 depthCameraTUnityWorld;
			  float point_size;

			  v2f vert(appdata v)
			  {
				 v2f o;
				 o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				 o.size = point_size;

				 // Use color from appdata
				 o.color = v.color;
				 return o;
			  }

			  fixed4 frag(v2f i) : SV_Target
			  {
				 return i.color;
			  }
			  ENDCG
		   }
	}
}