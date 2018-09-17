-- CS.UnityEngine.Debug.Log('hoge')
local x = CS.Enemy.XCount
local y = CS.Enemy.YCount
local time = CS.Enemy.frameCount
local PI = 3.14
local bullet = CS.Enemy

bullet.SetSpeed(0.05)
bullet.SetAngle1(PI/6*x+PI/180*time)
bullet.SetAngle2(PI*2/3*y)
bullet.SetColor(1.0/12*x)
