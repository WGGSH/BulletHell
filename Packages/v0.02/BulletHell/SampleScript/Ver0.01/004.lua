-- サンプル004: 弾の発射角度を設定する Part3
-- BulletCreateの第5,第6引数(2つの角度)を組み合わせることで，任意の方向に弾を発射できます
-- 第4引数は弾の速度で，第4~第6引数から弾の移動ベクトルを算出します

-- 第4引数: speed
-- 第5引数: angle1
-- 第6引数: angle2
-- とした時，発射する弾のベクトルは
-- x: speed*cos(angle1)*cos(angle2)
-- y: speed*sin(angle2)
-- z: speed*sin(angle1)*cos(angle2)

-- となります

if time % 30 == 0 then
  for y =1,24 do
    for x =1,12 do
      enemy.BulletCreate(
        0,0,0,
        0.05,PI/12*x,PI*2/24*y,
        1.0,0.5,0.5
      )
    end
  end
end
