-- サンプル011: 試作弾幕 part2
-- 「サイレントセレナ」風弾幕です

if time % 3 == 0 then
  if time % 30 < 15 then
    for y = 1,12 do
      for x =1,12 do
        enemy.BulletCreate(
          0,0,0,
          0.10,PI*2/12*x+PI/36*math.floor(time/15),PI*2/12*y,
          0.8,0.5,0.3
        )
      end
    end
  else
    for y = 1,12 do
      for x =1,12 do
        enemy.BulletCreate(
          0,0,0,
          0.10,PI*2/12*x+PI/36*math.floor(time/15)+PI*2/12/2,PI*2/12*y+PI*2/12/2,
          0.6,0.5,0.3
        )
      end
    end
  end
end
