-- サンプル001: とりあえず弾を発射する
-- 変数time には,現在のフレーム数が格納されます

if time % 30 == 0 then
  enemy.BulletCreate(
    0,0,0,
    0.05,0,0,
    1.0,0.5,0.5
  )
end
