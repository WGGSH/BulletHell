a
if time%5==0 then
  for y = 1, 12 do
    for x = 1, 12 do
      enemy.BulletCreate(0,0,0,0.05,PI/12*x,PI*2/12*y,1,0.5,0.5)
    end
  end
end
