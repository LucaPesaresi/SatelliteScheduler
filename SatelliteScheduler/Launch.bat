@echo off

SETLOCAL ENABLEDELAYEDEXPANSION
SET PATH=C:\Users\luca2\source\repos\SatelliteScheduler\SatelliteScheduler\day1_
SET PROGRAM=C:\Users\luca2\source\repos\SatelliteScheduler\SatelliteScheduler\bin\Release\netcoreapp3.1\SatelliteScheduler.exe

FOR /L %%x IN (0,1,9) DO (
    SET "NUM=%%x/"
    SET PATHCOMPLETE=%PATH%!NUM!
    FOR /L %%s IN (0,1,5) DO (
    SET "SEED=%%s"
        !PROGRAM! !PATHCOMPLETE! !SEED! 40 2 100
    )
)

pause