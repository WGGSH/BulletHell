-- サンプル006: time変数を利用する part1
-- time変数を使用して，時間経過によって色が変化する弾幕を作ることができます．

if time % 3 == 0 then
  for x =1,16 do
    enemy.BulletCreate(
      0,0,0,
      0.20,PI*2/16*x,0,
      (time/100)%1,0.5,0.5
    )
  end
end
