@echo off

for /f "tokens=*" %%A in ('cd') do set verpath=%%A

echo ------------------------------------------------------------
echo Cleanup stale remote branches for %verpath%
echo ------------------------------------------------------------

git remote prune origin 
echo.

echo ------------------------------------------------------------
echo Remove all branches with gone tracked branches for %verpath%
echo ------------------------------------------------------------
echo The following local branches will be deleted:
echo.

setlocal enableextensions 
for /f "tokens=*" %%a in ( 
'git branch -vv ^| findstr origin/.*:.*gone]' 
) do ( 
    for /f "tokens=1" %%b in ('
        "echo %%a"
    ') do (
    echo %%b
    )
)

echo.
:choice
set /P c=Do you want to continue deleting the branches[Y/N]?
if /I "%c%" EQU "Y" goto :somewhere
if /I "%c%" EQU "N" goto :somewhere_else
goto :choice


:somewhere

for /f "tokens=*" %%a in ( 
'git branch -vv ^| findstr origin/.*:.*gone]' 
) do ( 
    for /f "tokens=1" %%b in ('
        "echo %%a"
    ') do (
    git branch -D %%b
    )
)
echo.
echo "Successfully deleted all branches."
pause 
exit

:somewhere_else

echo.
echo "Deletion of local branches aborted."
pause 
exit
endlocal