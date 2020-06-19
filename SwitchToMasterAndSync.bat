@echo off
setlocal enableextensions 

for /f "tokens=*" %%A in ('cd') do set verpath=%%A
FOR /F "tokens=*" %%F IN ('git diff-index --name-only HEAD --') DO (
   goto :error
)
goto :execute

:error
echo There are uncommited changes in %verpath%. Commit or stash the changes before executing this script.
pause
exit

:execute
echo ------------------------------------------------------------
echo Switch %verpath% to 'master'
echo ------------------------------------------------------------
echo.

git checkout master
echo.

echo ------------------------------------------------------------
echo Pull all branches
echo ------------------------------------------------------------
echo.

git pull --all
echo.

pause
exit
endlocal