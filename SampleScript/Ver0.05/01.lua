-- Ver0.05 サンプル01: 弾のキャッシュ登録
-- BulletCreate後に enemy.AddCache() を呼び出すと変数 cache に作成した弾のインデックスが保持されます.
-- キャッシュ化した弾にアクセスする場合, 変数 cacheSize にはキャッシュ化された弾の総数が入り,
-- enemy.getCache(index) で弾のデータを取得することができます.
-- この方法では,発射した弾の情報を後から取得することは可能ですが,キャッシュ化した弾の情報を書き換えることはできません.
-- キャッシュ化した弾の情報を書き換える必要がある場合,enemy.SetVelocity(Index,...)などの関数を使用し,
-- Indexに変数 cache[value] を渡すことでキャッシュ化した弾の情報を書き換えることができます
-- valueにはキャッシュ化した弾全てを参照する際のループカウンタなどを入れてください

if time == 1 then
  for i=0,5 do
    enemy.BulletCreate(
      0,0,0,
      0.05,PI*2/6*i,0,
      1.0/8*i,0.5,0.5,
      3.0
    )
    enemy.AddCache()
  end

  for i=0,1 do
    enemy.BulletCreate(
      0,0,0,
      0.05,0,PI*i+PI/2,
      1.0/8*6+1.0/8*i,0.5,0.5,
      3.0
    )
    enemy.AddCache()
  end
end

if time == 61 then
  for i = 0,cacheSize do
    enemy.SetVelocity(cache[i],Vector3(0,0,0))
  end
end

if time >= 76 and time % 2 ==0 then
  for i = 0, cacheSize do
    local option = enemy.GetCache(i)
    for x = 0,2 do
      for y = 0,1 do
        enemy.BulletCreate(
          option.pos.x,option.pos.y,option.pos.z,
          0.05,PI*2/3*x+PI/180*time,(PI/180*time+PI/180*45)*(y*2-1),
          1.0/cacheSize*i,0.5,0.3
        )
      end
    end
  end
end
