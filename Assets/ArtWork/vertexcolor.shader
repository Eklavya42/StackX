Shader "vertexcolor"{
	Properties{
		_MainTex("Base (RGB)",2D)="white" {}
	}
	Subshader 
	{
		Pass{
			Lighting On
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] {
				combine texture*primary DOUBLE
			}
		}
	}
}