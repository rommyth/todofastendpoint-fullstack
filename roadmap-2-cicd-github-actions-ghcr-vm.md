# Roadmap CI/CD GitHub Actions + GHCR + Ubuntu VM

Status:

```text
SELESAI
```

Dokumen ini mencatat alur CI/CD yang sudah berhasil diterapkan untuk project Todo FastEndpoints + React.

## Target Akhir

```text
push code ke GitHub
-> GitHub Actions menjalankan CI
-> Docker image backend/frontend dibuat
-> image dipush ke GitHub Container Registry (GHCR)
-> deploy job berjalan di self-hosted runner pada VM Ubuntu
-> VM pull image terbaru
-> VM menjalankan docker compose up -d
```

## Yang Sudah Dilakukan

### 1. CI Dasar

CI dasar sudah berhasil untuk:

```text
Backend Build
Frontend Build
```

Backend menjalankan:

```bash
dotnet restore
dotnet build --configuration Release --no-restore
```

Frontend menjalankan:

```bash
npm ci
npm run build
```

Lint frontend sempat disiapkan, tetapi saat ini dibuat opsional/commented di workflow:

```yaml
# - name: Lint frontend
#   run: npm run lint
```

### 2. Build dan Push Image ke GHCR

Workflow sudah membangun image backend dan frontend lalu mengirimnya ke GHCR.

Image backend:

```text
ghcr.io/owner/repository/backend:latest
ghcr.io/owner/repository/backend:<commit-sha>
```

Image frontend:

```text
ghcr.io/owner/repository/frontend:latest
ghcr.io/owner/repository/frontend:<commit-sha>
```

Nama image berasal dari variable workflow:

```yaml
BACKEND_IMAGE: ghcr.io/${{ github.repository }}/backend
FRONTEND_IMAGE: ghcr.io/${{ github.repository }}/frontend
```

### 3. Masalah SSH dari GitHub-Hosted Runner

VM lokal/NAT hanya bisa diakses dari komputer lokal dengan:

```bash
ssh hiromi@127.0.0.1 -p 2222
```

Alamat `127.0.0.1` dari GitHub-hosted runner berarti runner GitHub itu sendiri, bukan VM lokal.

Kesimpulan:

```text
GitHub-hosted runner tidak bisa SSH langsung ke VM lokal/NAT tanpa IP public.
```

### 4. Solusi Self-Hosted Runner

Solusi final:

```text
deploy job dijalankan oleh self-hosted runner yang dipasang di VM Ubuntu
```

Dengan ini GitHub tidak perlu SSH ke VM.

Alurnya:

```text
GitHub-hosted runner:
CI + build image + push image ke GHCR

Self-hosted runner di VM:
docker login ghcr.io
docker compose pull
docker compose up -d
```

Deploy job memakai:

```yaml
runs-on: self-hosted
```

### 5. Runner Berjalan Sebagai Service

Runner bisa dijalankan manual dengan:

```bash
./run.sh
```

Namun mode final yang dipakai adalah service:

```bash
sudo ./svc.sh install
sudo ./svc.sh start
```

Keuntungan:

```text
runner berjalan di background
terminal tidak perlu tetap terbuka
runner otomatis hidup lagi setelah VM reboot
```

### 6. Compose Server Menggunakan Image GHCR

Di VM, `docker-compose.yml` tidak memakai `build`.

Compose server memakai `image`, misalnya:

```yaml
services:
  backend-1:
    image: ghcr.io/owner/repository/backend:latest
    ports:
      - "5030:5030"

  backend-2:
    image: ghcr.io/owner/repository/backend:latest
    ports:
      - "5031:5030"

  frontend:
    image: ghcr.io/owner/repository/frontend:latest
    ports:
      - "8000:80"

  redis:
    image: redis:8-alpine
    ports:
      - "6379:6379"
```

Dengan pola ini:

```text
VM tidak build image sendiri
VM hanya pull image dari GHCR
deploy lebih cepat
hasil deploy konsisten dengan image yang dibuat CI/CD
```

## Workflow Final

File workflow:

```text
.github/workflows/ci.yml
```

Job yang ada:

```text
backend
frontend
docker-publish
deploy
```

Urutan job:

```text
backend + frontend
-> docker-publish
-> deploy
```

Deploy job final:

```yaml
deploy:
  name: Deploy to Ubuntu VM
  runs-on: self-hosted
  needs: [docker-publish]

  if: github.event_name == 'push' && github.ref == 'refs/heads/main'

  steps:
    - name: Deploy with Docker Compose
      run: |
        cd ~/todofastendpoint-fullstack
        echo "${{ secrets.GHCR_TOKEN }}" | docker login ghcr.io -u "${{ secrets.GHCR_USERNAME }}" --password-stdin
        docker compose pull
        docker compose up -d
```

## Secrets GitHub Yang Dipakai

Lokasi setup:

```text
GitHub repository
-> Settings
-> Secrets and variables
-> Actions
-> New repository secret
```

Secrets yang masih dibutuhkan dalam flow final:

```text
GHCR_USERNAME
GHCR_TOKEN
```

Secrets SSH tidak lagi wajib karena deploy berjalan langsung di self-hosted runner VM:

```text
SSH_HOST
SSH_USER
SSH_KEY
SSH_PORT
```

### `GHCR_USERNAME`

Isi:

```text
username GitHub yang punya akses ke package GHCR
```

Pengaruh:

```text
menentukan user yang dipakai VM untuk login ke GHCR
```

### `GHCR_TOKEN`

Isi:

```text
Personal Access Token GitHub dengan permission read:packages
```

Pengaruh:

```text
memberi izin ke VM untuk pull image private dari GHCR
```

## Checklist Selesai

```text
[x] ci.yml sudah dibuat dalam 1 file
[x] backend build berjalan di GitHub Actions
[x] frontend build berjalan di GitHub Actions
[x] Docker image backend dibuat
[x] Docker image frontend dibuat
[x] Docker image backend dipush ke GHCR
[x] Docker image frontend dipush ke GHCR
[x] GHCR_USERNAME dibuat di GitHub Secrets
[x] GHCR_TOKEN dibuat di GitHub Secrets
[x] VM memakai docker-compose.yml berbasis image GHCR
[x] self-hosted runner dipasang di VM
[x] self-hosted runner berjalan sebagai service
[x] deploy job memakai runs-on: self-hosted
[x] docker compose pull berjalan dari VM
[x] docker compose up -d berjalan dari VM
[x] CI/CD berhasil deploy ke VM
```

## Ringkasan Alur Final

```text
Developer push ke main
-> GitHub Actions menjalankan backend build
-> GitHub Actions menjalankan frontend build
-> GitHub Actions build Docker image backend
-> GitHub Actions build Docker image frontend
-> GitHub Actions push image ke GHCR
-> self-hosted runner di VM mengambil deploy job
-> VM login ke GHCR
-> VM pull image terbaru
-> VM restart container dengan docker compose up -d
```

## Catatan Beban VM

Self-hosted runner tidak dipakai untuk build berat.

Build berat tetap berjalan di GitHub-hosted runner:

```text
dotnet build
npm build
docker build
docker push
```

VM hanya melakukan deploy:

```text
docker login
docker compose pull
docker compose up -d
```

Karena itu beban VM relatif ringan.
