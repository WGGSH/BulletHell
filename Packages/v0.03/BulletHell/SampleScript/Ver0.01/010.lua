-- サンプル010: 試作弾幕 part1
-- 「波と粒の境界」風弾幕です

for x =1,4 do
  enemy.BulletCreate(
    0,0,0,
    0.05,PI*2/4*x+time*time/10000,0,
    0.8,0.5,0.5
  )
end
