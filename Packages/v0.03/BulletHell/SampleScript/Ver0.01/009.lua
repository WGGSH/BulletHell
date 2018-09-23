-- サンプル009: 弾の発射位置を変更する
-- 第1~第3引数は弾の初期座標(x,y,z)に対応しています

if time % 3 == 0 then
  for x =1,8 do
    for i = -1,1,2 do
      enemy.BulletCreate(
        0,((time/10)%8)*i,0,
        0.03,PI*2/8*x+PI/180*time,0,
        1.0/16*x,0.5,0.5
      )
    end
  end
end
