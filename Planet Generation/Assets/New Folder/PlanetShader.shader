Shader "Custom/DistanceColorShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader{
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    // Calculate distance from origin
                    float dist = length(v.vertex);

                    // Set vertex color based on distance
                    if (dist < 1.0) {
                        o.color = float4(0.0, 1.0, 0.0, 1.0); // Green for grass
                    }
     else if (dist < 2.0) {
      o.color = float4(0.5, 0.5, 0.5, 1.0); // Grey for stone
  }
else {
 o.color = float4(1.0, 1.0, 1.0, 1.0); // White for everything else
}

return o;
}

float4 frag(v2f i) : SV_Target {
    return i.color;
}
ENDCG
}
        }
            FallBack "Diffuse"
}