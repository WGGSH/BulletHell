-- Ver0.02 サンプル02: 加速度 part2
-- 加速度を弾の発射方向と関係なく一定の値にすることで，重力のように落下する弾を
-- このサンプルでは，東方風神録の洩矢諏訪子『ケロちゃん風雨に負けず』のような弾幕を生成しています

for x =1,6 do
  for y = 1,3 do
    enemy.BulletCreate(
      0,0,0,
      0.15-0.02*y+0.03*math.sin(time/20),PI*2/6*x+PI*2/6*math.sin(time/15),PI/3+PI/24*y,
      0.0009,0,-PI/2,
      0.4+0.05*y,0.5,0.5
    )
  end
end