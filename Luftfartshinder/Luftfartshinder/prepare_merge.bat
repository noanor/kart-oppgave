@echo off
git config core.pager ""
git add .
git commit -m "Add tutorial modal, organization filter, and restore original wheel icons"
git fetch origin main
git merge --no-commit --no-ff origin/main
if %ERRORLEVEL% EQU 0 (
    echo No conflicts detected
    git merge --abort
) else (
    echo Conflicts detected - please resolve manually
)
git push origin VS.11-MS

