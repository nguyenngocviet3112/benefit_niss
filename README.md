# TimorINSS — Local Development Setup

Hệ thống gồm 3 phần chạy độc lập qua Docker (không cần cài Node/.NET/SQL Server trực tiếp lên máy):

| Service | Container name | Image | Port (host) |
|---|---|---|---|
| Database (SQL Server) | `inss-db` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 |
| Backend API (.NET Core 3.1) | `inss-backend` | `mcr.microsoft.com/dotnet/sdk:3.1` | 5000 |
| Frontend Interno (Angular 11) | `inss_interno` | `node:14` | 4300 |

**Lý do dùng Docker cho frontend/backend:** máy dev đang cài Node/.NET bản mới hơn nhiều so với bản mà project này yêu cầu (Angular 11 cần Node ≤16, .NET project build với SDK 3.1) — chạy trực tiếp bằng `ng serve`/`dotnet run` trên máy sẽ lỗi (vd Node 18+ gây lỗi `No such module: http_parser`). Container cô lập đúng runtime version cần thiết, không phải cài lại môi trường mỗi lần.

File cấu hình: **`docker-compose.yml`** ở gốc repo — chứa đúng config (image/port/mount/env/network) của cả 3 service.

> **Lưu ý:** nếu 3 container `inss-db`/`inss-backend`/`inss_interno` đang chạy sẵn (tạo thủ công bằng `docker run`, không phải qua compose), thì `docker compose up` sẽ báo lỗi "container name already in use". Trường hợp đó chỉ cần `docker start <tên container>` (xem mục dưới) — `docker-compose.yml` chỉ dùng khi cần **tạo lại từ đầu** (sau `docker rm`) hoặc trên máy khác chưa có container nào.

```bash
docker compose up -d
```

## Khởi động nhanh (khi container đã tồn tại)

```bash
# 1. Mở Docker Desktop (nếu chưa chạy)
open -a Docker

# 2. Start theo đúng thứ tự: DB trước (đợi healthy) → backend → frontend
docker start inss-db
# đợi vài giây tới khi "healthy":
docker inspect --format='{{.State.Health.Status}}' inss-db

docker start inss-backend
docker start inss_interno
```

Kiểm tra đã lên chưa:
```bash
docker ps --filter "name=inss" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
curl -s -o /dev/null -w "frontend: %{http_code}\n" http://localhost:4300/
curl -s -o /dev/null -w "backend:  %{http_code}\n" http://localhost:5000/
```
Mở **http://localhost:4300/** để dùng app. Backend trả `404` ở `/` là bình thường (API không có route gốc) — miễn không phải "connection refused" là server đã chạy.

## Nếu container bị xoá — lệnh tạo lại từ đầu

**Database:**
```bash
docker run -d \
  --name inss-db \
  --network inss-benefit-app_default \
  -p 1433:1433 \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD='INSSdev@2024!' \
  -e MSSQL_PID=Developer \
  mcr.microsoft.com/mssql/server:2022-latest
```

**Backend** (yêu cầu đã build sẵn — repo này có sẵn `bin/Release/netcoreapp3.1/...` nên chạy được `--no-build` ngay, không cần build lại):
```bash
docker run -d \
  --name inss-backend \
  --network inss-benefit-app_default \
  -p 5000:5000 \
  -v "$(pwd)/backend/TimorINSS-BackEnd-main:/src" \
  -w /src/TimorINSS-BackEnd \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e "ConnectionStrings__sqlserverconnection=Server=inss-db,1433;Database=TimorINSSModuloContribuicoes;User ID=sa;Password=INSSdev@2024!;TrustServerCertificate=True;Encrypt=False" \
  -e DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  -e DOTNET_USE_POLLING_FILE_WATCHER=true \
  -e NUGET_XMLDOC_MODE=skip \
  mcr.microsoft.com/dotnet/sdk:3.1 \
  bash -c "dotnet run -c Release --no-build --no-launch-profile"
```

**Frontend Interno:**
```bash
docker run -d \
  --name inss_interno \
  -p 4300:4300 \
  -v "$(pwd)/financial/TimorINSS-MInterno:/app" \
  -w /app \
  node:14 \
  bash -c "npm install --legacy-peer-deps 2>&1 | tail -5 && npx ng serve --host 0.0.0.0 --port 4300 --disable-host-check"
```
Lần đầu chạy sẽ mất vài phút để `npm install`. Theo dõi log tới khi thấy `Compiled successfully`:
```bash
docker logs -f inss_interno
```

> `$(pwd)` giả định đang đứng ở thư mục gốc repo (`inss_contrib_finance`). Nếu chạy từ chỗ khác, thay bằng đường dẫn tuyệt đối tới repo.

## Lưu ý quan trọng — mount path

Container **phải mount đúng vào thư mục repo hiện tại** (`inss_contrib_finance/...`), không phải một thư mục tên cũ khác (`benefit_niss/...` — tên repo trước khi đổi tên local). Nếu thấy container start xong rồi exit ngay với lỗi kiểu:
- Frontend: `Cannot find module`, hoặc trang trắng không load được `src/`
- Backend: `Couldn't find a project to run. Ensure a project exists in /src/TimorINSS-BackEnd`

→ nghĩa là mount đang trỏ sai thư mục (thường do Docker tự tạo thư mục rỗng khi mount source không tồn tại). Cách sửa: `docker rm <container>` rồi chạy lại đúng lệnh `docker run` ở trên với `-v` trỏ về thư mục repo thật.

## Dừng lại khi xong việc

```bash
docker stop inss_interno inss-backend inss-db
```
(Dùng `stop` chứ không `rm` — giữ lại container để lần sau chỉ cần `docker start`, không phải cài `npm install`/tạo lại từ đầu.)
