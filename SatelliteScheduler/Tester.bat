@echo off

SETLOCAL ENABLEDELAYEDEXPANSION
SET PATH=C:\Users\luca2\source\repos\SatelliteScheduler\SatelliteScheduler\day1_0\
SET PROGRAM=C:\Users\luca2\source\repos\SatelliteScheduler\SatelliteScheduler\bin\Release\netcoreapp3.1\SatelliteScheduler.exe

FOR /L %%k IN (1,10,100) DO (
    FOR /L %%n IN (1,10,100) DO (
	SET "KRUIN=%%k"
	SET "NOISE=%%n"
        !PROGRAM! !PATH! 0 !KRUIN! !NOISE! 100
    )
)

pause