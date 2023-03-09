Shader "Custom/SphereDistanceColor" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Range(0.1, 10.0)) = 1.0
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
                    float distance : TEXCOORD0;
                };

                float4 _Color;
                float _Radius;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.distance = length(UnityObjectToWorldPos(v.vertex)) / _Radius;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    return _Color * (1.0 - i.distance);
                }
                ENDCG
            }
    }
}
