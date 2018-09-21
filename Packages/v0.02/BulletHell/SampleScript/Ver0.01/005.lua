-- サンプル005: 弾の色を変更する
-- BulletCreateの第7~第9引数はそれぞれHSV表色系のH,S,Vに対応しています
-- 値はいずれも0~1の範囲で指定できます

if time % 30 == 0 then
  for x =1,36 do
    enemy.BulletCreate(
      0,0,0,
      0.05,PI*2/36*x,0,
      1.0/36*x,0.5,0.5
    )
  end
end
