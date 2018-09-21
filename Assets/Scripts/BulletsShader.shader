Shader "Custom/BulletsShader" {
	SubShader {
		// アルファを使う
    Tags { "Queue" = "Transparent" }
    ZWrite Off
    // Blend SrcAlpha OneMinusSrcAlpha
    Blend OneMinusSrcAlpha One
    Pass {
      CGPROGRAM
      // シェーダーモデルは5.0を指定
      #pragma target 5.0
      // シェーダー関数を設定 
      #pragma vertex vert
      #pragma geometry geom
      #pragma fragment frag
      #include "UnityCG.cginc"
      // テクスチャ
      sampler2D _Tex0;
      sampler2D _Tex1;
      // 弾の構造体
      struct BulletDX{
        float3 pos;
				float3 accel;
				float4 col;
        int count;
        float angle;
      };
      // 弾の構造化バッファ
      StructuredBuffer<BulletDX> Bullets;
      // 頂点シェーダからの出力
      struct VSOut {
        float4 pos : SV_POSITION;
        float2 tex : TEXCOORD0;
        float4 col : COLOR;
        float angle : ANGLE;
      };
      // 頂点シェーダ
			VSOut vert (uint id : SV_VertexID){
        // idを元に、弾の情報を取得
        VSOut output;
        output.pos = float4(Bullets[id].pos, 1);
        output.tex = float2(0, 0);
        output.col = Bullets[id].col;
        output.angle = Bullets[id].angle;
        return output;
	    }
      // ジオメトリシェーダ
      [maxvertexcount(4)]
      void geom (point VSOut input[1], inout TriangleStream<VSOut> outStream){
        VSOut output;
        // 全ての頂点で共通の値を計算しておく
        float4 pos = input[0].pos;
        float4 col = input[0].col;
        // 四角形になるように頂点を生産
        for(int x = 0; x < 2; x++){
          for(int y = 0; y < 2; y++){
            // ビルボード用の行列
            // float angle = 3.14/6;
            float4x4 billboardMatrix = UNITY_MATRIX_V;
            billboardMatrix._m03 = 
            billboardMatrix._m13 = 
            billboardMatrix._m23 =
            billboardMatrix._m33 = 0;
            
            // billboardMatrix._m11 = cos(input[0].angle);
            // billboardMatrix._m12 = -sin(input[0].angle);
            // billboardMatrix._m21 = sin(input[0].angle);
            // billboardMatrix._m22 = cos(input[0].angle);

            // 座標回転
            float2x2 rotMat = 0;
            rotMat._m00 = cos(input[0].angle);
            rotMat._m01 = -sin(input[0].angle);
            rotMat._m10 = sin(input[0].angle);
            rotMat._m11 = cos(input[0].angle);

            // テクスチャ座標
            float2 tex = float2(x, y);
            output.tex = tex;

            // float2 rotPos = float2(
            //   (x*2-1)*cos(input[0].angle)-(y*2-1)*sin(input[0].angle),
            //   (x*2-1)*sin(input[0].angle)+(y*2-1)*cos(input[0].angle)
            // )*0.2;
            float2 rotPos = float2(x*2-1,y*2-1)*0.2;

            // 頂点位置を計算
            output.pos = pos+ mul(float4(rotPos,0,1), billboardMatrix);
            //output.pos = pos+ mul(float4((tex * 2 - float2(1, 1)) * 0.2, 0, 1)*rotMat, billboardMatrix);
			          	output.pos = mul (UNITY_MATRIX_VP, output.pos);
            // 色
            output.col = col;
            // 角度
            output.angle = input[0].angle;
            // ストリームに頂点を追加
            outStream.Append (output);
			    }
		    }
        // トライアングルストリップを終了
        outStream.RestartStrip();
      }
			// ピクセルシェーダー
      fixed4 frag (VSOut i) : COLOR{
        // 出力はテクスチャカラーと頂点色
        float4 col = tex2D(_Tex0, i.tex) * i.col;
        // アルファが一定値以下なら中断
        // if(col.a < 0.3) discard;
        // 色を返す
        return col;
      }
      ENDCG
    } 
  }
}
