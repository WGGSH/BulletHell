-- サンプル007: time変数を利用する part2
-- time変数を使用して，時間経過で発射角度の変わる弾幕を作ることができます

if time % 3 == 0 then
  for x =1,16 do
    enemy.BulletCreate(
      0,0,0,
      0.10,PI*2/16*x+PI/180*time,0,
      1.0/16*x,0.5,0.5
    )
  end
end
