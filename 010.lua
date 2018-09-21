-- サンプル010: 試作弾幕 part1
-- 「波と粒の境界」風弾幕です

local localTime = 200*math.sin(time/500);

for x =1,4 do
  enemy.BulletCreate(
    0,0,0,
    0.05,PI*2/4*x+localTime*localTime/1000,0,
    0.8,0.5,0.5
  )
end
