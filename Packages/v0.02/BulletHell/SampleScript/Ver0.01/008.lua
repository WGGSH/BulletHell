-- サンプル008: time変数を利用する part3
-- time変数を使用して，時間帯によって攻撃方法が変わる弾幕を作成できます

if time % 60 < 30 then
  for x =1,8 do
    enemy.BulletCreate(
      0,0,0,
      0.03,PI*2/8*x+PI/180*time,0,
      1.0/16*x,0.5,0.5
    )
  end
else
  for y =1,32 do
    enemy.BulletCreate(
      0,0,0,
      0.10,0,PI*2/32*y+PI/65*time,
      1.0/32*y,0.5,0.5
    )
  end
end
