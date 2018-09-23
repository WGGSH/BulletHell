-- Ver0.03 サンプル01: 弾のサイズ設定その1
-- BulletCreateに第10引数(加速度使用時は第13引数)を設定することで,弾の大きさを設定できます
-- 引数を渡さない場合は自動的に1になります

for x =1,6 do
  enemy.BulletCreate(
    0,0,0,
    0.10,PI*2/6*x+PI/180*time,0,
    0.8,0.5,0.5,
    math.sin(PI/180*time+PI*2/3*x)+2
  )
end
