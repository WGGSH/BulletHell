-- サンプル002: 弾の発射角度を設定する Part1
-- BulletCreateの第5引数(下記サンプルで"PI*2/12*x")が発射角度その1になります

if time % 30 == 0 then
  for x =1,12 do
    enemy.BulletCreate(
      0,0,0,
      0.05,PI*2/12*x,0,
      1.0,0.5,0.5
    )
  end
end
