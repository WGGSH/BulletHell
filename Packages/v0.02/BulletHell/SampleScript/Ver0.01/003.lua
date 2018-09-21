-- サンプル003: 弾の発射角度を設定する Part2
-- BulletCreateの第6引数(下記サンプルで"PI*2/12*x")が発射角度その2になります

if time % 30 == 0 then
  for y =1,12 do
    enemy.BulletCreate(
      0,0,0,
      0.05,0,PI*2/12*y,
      1.0,0.5,0.5
    )
  end
end
