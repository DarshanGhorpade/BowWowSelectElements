@echo off
setlocal

REM ============================================================
REM Parameters
REM %1 = ConfigurationName
REM %2 = TargetDir
REM %3 = TargetName
REM %4 = ProjectDir
REM %5 = TargetPath
REM %6 = ProjectName
REM ============================================================

SET "ProjectName=%~6"
SET "Year=%ProjectName:~-4%"
SET "ADDINS_BASE=C:\ProgramData\Autodesk\Revit\Addins\%Year%"
SET "ADDINS_PROJ=%ADDINS_BASE%\BowWowSelection"

echo.
echo ============================================================
echo Starting Post-Build Script
echo Project : %ProjectName%
echo Config  : %~1
echo Source  : %~2
echo Target  : %ADDINS_PROJ%
echo ============================================================
echo.

IF NOT EXIST "%ADDINS_PROJ%" (
    mkdir "%ADDINS_PROJ%"
)

echo Copying main assembly...

copy /Y "%~2%~3.dll" "%ADDINS_PROJ%\"

IF ERRORLEVEL 1 (
    echo ERROR: Failed to copy %~3.dll
    exit /b 1
)

echo Copying dependencies...

for %%f in ("%~2*.dll") do (
    if /I NOT "%%~nxf"=="%~3.dll" (
        echo Copying %%~nxf
        copy /Y "%%f" "%ADDINS_PROJ%\"

        IF ERRORLEVEL 1 (
            echo ERROR: Failed to copy %%~nxf
            exit /b 1
        )
    )
)

echo Copying addin manifest...

copy /Y "%~4SampleAddInFiles\%Year%\Selection.Revit.%Year%.addin" "%ADDINS_BASE%\"

IF ERRORLEVEL 1 (
    echo ERROR: Failed to copy "Selection.Revit.%Year%.addin"
    exit /b 1
)

echo.
echo Post-Build Script completed successfully.
echo.

endlocal
exit /b 0