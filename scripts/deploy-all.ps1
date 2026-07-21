# deploy-all.ps1 — 1-click deploy cho ca backend + frontend tren staging (INSS-SINMOR RDP box)
#
# Gia dinh (xem memory inss-staging-deploy-state de biet chi tiet topology):
#   Backend git repo:  C:\Users\inss.sinmor\Desktop\Code_GITHUB_NISS\backend\TimorINSS-BackEnd-main\TimorINSS-BackEnd
#   Backend chay that: C:\Users\inss.sinmor\Desktop\Financial_Module_BugFix_Project\backend_13july2026 (port 5000)
#   Frontend git repo/chay that (sau khi da reconcile 2026-07-20):
#                      C:\Users\inss.sinmor\Desktop\Code_GITHUB_NISS\financial\TimorINSS-MInterno (port 4200)
#
# LUU Y: script nay chay backend qua `dotnet run` thay vi mo Visual Studio bam F5.
# Neu can debug (dat breakpoint...) thi van mo VS nhu cu - script nay chi danh cho deploy nhanh.
#
# Lan dau dung, nen chay tung doan tay (backend truoc, frontend sau) de chac an, sau do moi
# tin tuong chay nguyen script.

$ErrorActionPreference = "Stop"

# ============ BACKEND ============
Write-Host "`n===== BACKEND =====" -ForegroundColor Cyan

$backendGitRepo = "C:\Users\inss.sinmor\Desktop\Code_GITHUB_NISS\backend\TimorINSS-BackEnd-main\TimorINSS-BackEnd"
$backendRunFolder = "C:\Users\inss.sinmor\Desktop\Financial_Module_BugFix_Project\backend_13july2026"

Write-Host "Pulling backend git repo..."
git -C $backendGitRepo pull

Write-Host "Syncing code vao thu muc dang chay..."
robocopy $backendGitRepo $backendRunFolder /E /XD bin obj .vs Cache .config /XF appsettings*.json *.csproj.user | Out-Null

Write-Host "Dung backend cu (port 5000)..."
$conn5000 = Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction SilentlyContinue
if ($conn5000) {
    Stop-Process -Id $conn5000.OwningProcess -Force
    Start-Sleep -Seconds 2
    Write-Host "Da dung backend cu." -ForegroundColor Green
} else {
    Write-Host "Khong thay backend dang chay - bo qua." -ForegroundColor Yellow
}

Write-Host "Build backend..."
dotnet build $backendRunFolder --configuration Debug

Write-Host "Khoi dong lai backend..."
Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$backendRunFolder`" --no-build" -WorkingDirectory $backendRunFolder
Start-Sleep -Seconds 5
$checkBackend = Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction SilentlyContinue
if ($checkBackend) {
    Write-Host "==> BACKEND OK, dang lang nghe port 5000." -ForegroundColor Green
} else {
    Write-Host "==> CANH BAO: backend chua len port 5000, kiem tra cua so console moi mo." -ForegroundColor Red
}

# ============ FRONTEND ============
Write-Host "`n===== FRONTEND =====" -ForegroundColor Cyan

$frontendGitRepo = "C:\Users\inss.sinmor\Desktop\Code_GITHUB_NISS\financial\TimorINSS-MInterno"

Write-Host "Pulling frontend git repo..."
git -C $frontendGitRepo pull

Write-Host "Dung frontend cu (port 4200)..."
$conn4200 = Get-NetTCPConnection -LocalPort 4200 -State Listen -ErrorAction SilentlyContinue
if ($conn4200) {
    Stop-Process -Id $conn4200.OwningProcess -Force
    Start-Sleep -Seconds 2
    Write-Host "Da dung frontend cu." -ForegroundColor Green
} else {
    Write-Host "Khong thay frontend dang chay - bo qua." -ForegroundColor Yellow
}

Write-Host "Khoi dong lai frontend (cua so moi)..."
Start-Process -FilePath "powershell" -ArgumentList "-NoExit", "-Command", "cd '$frontendGitRepo'; ng serve --host 0.0.0.0 --port 4200 --disable-host-check"

Write-Host "`n===== XONG =====" -ForegroundColor Cyan
Write-Host "Backend + Frontend da duoc pull + build + restart. Doi ~1 phut roi kiem tra lai localhost:4200."
