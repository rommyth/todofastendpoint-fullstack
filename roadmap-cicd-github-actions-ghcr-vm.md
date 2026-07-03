# Roadmap CI/CD GitHub Actions + GHCR + Ubuntu VM

Dokumen ini mencatat alur belajar CI/CD untuk project Todo FastEndpoints + React.

Target akhirnya:

```text
push code ke GitHub
-> GitHub Actions menjalankan CI
-> Docker image backend/frontend dibuat
-> image dipush ke GitHub Container Registry (GHCR)
-> GitHub Actions SSH ke Ubuntu VM
-> VM pull image terbaru
-> docker compose up -d
```

## Kondisi Project

Project ini berisi:

```text
client-todo-fastendpoints/       frontend React + Vite
net-todo-fastendpoints/          backend .NET FastEndpoints
docker-compose.yml               compose lokal / server
.github/workflows/ci.yml         workflow CI/CD
```

Backend:

```text
.NET 10
FastEndpoints
Redis cache
Dockerfile backend
```

Frontend:

```text
React
Vite
TypeScript
Dockerfile frontend
Nginx
```

## Yang Sudah Dilewati

### 1. Memahami Tujuan CI/CD

CI/CD bukan hanya auto deploy.

CI berarti Continuous Integration:

```text
mengecek apakah code masih bisa dibuild dan belum rusak
```

CD berarti Continuous Delivery atau Continuous Deployment:

```text
mengirim hasil build yang sudah aman ke server
```

Pemahaman penting:

```text
CI menjaga agar code rusak tidak langsung masuk server.
CD membuat deploy bisa otomatis tanpa login manual ke server setiap update.
```

### 2. Memakai GitHub-Hosted Runner

Workflow memakai:

```yaml
runs-on: ubuntu-latest
```

Artinya job GitHub Actions berjalan di mesin Ubuntu sementara milik GitHub.

Mesin ini:

```text
dibuat saat workflow mulai
menjalankan command di file YAML
dihapus setelah workflow selesai
```

Karena runner ini sementara, Docker image yang dibuat di runner akan hilang jika tidak dipush ke registry.

### 3. Membuat CI Dasar

CI dasar sudah berhasil hijau untuk:

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
npm run lint
npm run build
```

Tujuannya:

```text
memastikan backend compile
memastikan dependency frontend valid
memastikan frontend lolos lint
memastikan frontend bisa dibuat production build
```

### 4. Menambahkan Docker Build

Docker build ditambahkan agar CI juga mengecek:

```text
Dockerfile backend valid
Dockerfile frontend valid
image bisa dibuat
```

Ini penting karena code bisa saja build sukses, tetapi Dockerfile gagal karena:

```text
path COPY salah
base image tidak cocok
dependency gagal diinstall di container
nginx config bermasalah
```

### 5. Memilih GHCR Sebagai Registry

GHCR adalah GitHub Container Registry.

GHCR dipilih karena:

```text
terintegrasi dengan GitHub
bisa dipakai langsung dari GitHub Actions
cocok untuk repo public/private
image tampil sebagai package di GitHub
```

Alur GHCR:

```text
GitHub Actions build image
-> push image ke ghcr.io
-> VM pull image dari ghcr.io
```

## Workflow Saat Ini

File workflow:

```text
.github/workflows/ci.yml
```

Workflow dibuat dalam 1 file agar mudah dipahami.

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

Bagian penghubungnya:

```yaml
needs: [backend, frontend]
```

Artinya `docker-publish` hanya jalan kalau backend dan frontend sukses.

```yaml
needs: [docker-publish]
```

Artinya `deploy` hanya jalan kalau image berhasil dibuat dan dipush ke GHCR.

## Hal Yang Belum Selesai

Berikut sisa langkah sampai deploy otomatis ke VM benar-benar berjalan.

### 0. File `ci.yml` Lengkap Dalam 1 File

Status:

```text
sudah dibuat
```

File ini sudah berisi:

```text
CI backend
CI frontend
push image ke GHCR
deploy via SSH ke VM
```

Namun deploy belum bisa sukses jika secrets GitHub dan setup VM belum lengkap.

### 1. Push Image ke GHCR

Status:

```text
belum selesai sampai workflow berhasil dijalankan di GitHub
```

Bagian workflow yang bertanggung jawab:

```yaml
docker-publish:
  name: Build and Push Docker Images
```

Bagian login GHCR:

```yaml
- name: Login to GHCR
  run: echo "${{ secrets.GITHUB_TOKEN }}" | docker login $REGISTRY -u ${{ github.actor }} --password-stdin
```

Bagian build image:

```yaml
docker build \
  -t $BACKEND_IMAGE:latest \
  -t $BACKEND_IMAGE:${{ github.sha }} \
  ./net-todo-fastendpoints
```

```yaml
docker build \
  -t $FRONTEND_IMAGE:latest \
  -t $FRONTEND_IMAGE:${{ github.sha }} \
  ./client-todo-fastendpoints
```

Bagian push:

```yaml
docker push --all-tags $BACKEND_IMAGE
docker push --all-tags $FRONTEND_IMAGE
```

Nama image otomatis:

```text
ghcr.io/username/nama-repo/backend:latest
ghcr.io/username/nama-repo/frontend:latest
```

Sumber nama image:

```yaml
BACKEND_IMAGE: ghcr.io/${{ github.repository }}/backend
FRONTEND_IMAGE: ghcr.io/${{ github.repository }}/frontend
```

`github.repository` otomatis berisi:

```text
owner/repository
```

### 2. Buat Token GHCR

Status:

```text
belum selesai
```

Token ini dibutuhkan agar VM bisa pull image private dari GHCR.

Jika package GHCR dibuat public, login di VM bisa saja tidak diperlukan. Namun untuk belajar deployment private yang lebih realistis, tetap siapkan token.

Cara membuat token:

```text
GitHub
-> klik profile picture
-> Settings
-> Developer settings
-> Personal access tokens
-> Tokens (classic)
-> Generate new token
```

Permission minimal untuk VM pull image:

```text
read:packages
```

Jika suatu saat token juga dipakai untuk push package, tambahkan:

```text
write:packages
```

Untuk alur saat ini:

```text
GitHub Actions push image memakai GITHUB_TOKEN
VM pull image memakai GHCR_TOKEN
```

Jadi GHCR token untuk VM cukup:

```text
read:packages
```

### 3. Setup GitHub Secrets

Status:

```text
belum selesai
```

Lokasi setup:

```text
GitHub repository
-> Settings
-> Secrets and variables
-> Actions
-> New repository secret
```

Secrets yang perlu dibuat:

```text
GHCR_USERNAME
GHCR_TOKEN
SSH_HOST
SSH_USER
SSH_KEY
SSH_PORT
```

#### `GHCR_USERNAME`

Isi:

```text
username GitHub yang punya akses ke package GHCR
```

Contoh:

```text
johndoe
```

Dipakai di workflow:

```bash
docker login ghcr.io -u "${{ secrets.GHCR_USERNAME }}"
```

Pengaruhnya:

```text
menentukan user yang dipakai VM untuk login ke GHCR
```

#### `GHCR_TOKEN`

Isi:

```text
Personal Access Token GitHub dengan permission read:packages
```

Dipakai di workflow:

```bash
echo "${{ secrets.GHCR_TOKEN }}" | docker login ghcr.io -u "${{ secrets.GHCR_USERNAME }}" --password-stdin
```

Pengaruhnya:

```text
memberi izin ke VM untuk pull image dari GHCR
```

Tanpa ini, jika package private, command ini akan gagal:

```bash
docker compose pull
```

#### `SSH_HOST`

Isi:

```text
IP public atau domain VM Ubuntu
```

Contoh:

```text
123.45.67.89
```

atau:

```text
api.example.com
```

Dipakai di workflow:

```yaml
host: ${{ secrets.SSH_HOST }}
```

Pengaruhnya:

```text
menentukan server mana yang akan diakses GitHub Actions lewat SSH
```

#### `SSH_USER`

Isi:

```text
username Linux di VM
```

Contoh:

```text
ubuntu
```

atau:

```text
deploy
```

Dipakai di workflow:

```yaml
username: ${{ secrets.SSH_USER }}
```

Pengaruhnya:

```text
menentukan user Linux yang menjalankan docker compose di server
```

Pastikan user ini punya akses menjalankan Docker.

Biasanya user perlu masuk group docker:

```bash
sudo usermod -aG docker ubuntu
```

Setelah itu logout dan login lagi.

#### `SSH_KEY`

Isi:

```text
private key SSH yang dipakai GitHub Actions untuk login ke VM
```

Contoh isi private key biasanya dimulai dengan:

```text
-----BEGIN OPENSSH PRIVATE KEY-----
...
-----END OPENSSH PRIVATE KEY-----
```

Dipakai di workflow:

```yaml
key: ${{ secrets.SSH_KEY }}
```

Pengaruhnya:

```text
memberi akses login SSH dari GitHub Actions ke VM tanpa password
```

Public key pasangannya harus ada di VM:

```text
~/.ssh/authorized_keys
```

#### `SSH_PORT`

Isi:

```text
port SSH server
```

Umumnya:

```text
22
```

Dipakai di workflow:

```yaml
port: ${{ secrets.SSH_PORT }}
```

Pengaruhnya:

```text
menentukan port yang dipakai GitHub Actions saat connect SSH ke VM
```

### 4. Ubah Compose Server Pakai Image GHCR

Status:

```text
belum selesai
```

Di server VM, compose sebaiknya tidak lagi memakai:

```yaml
build: ./net-todo-fastendpoints
```

Tapi memakai:

```yaml
image: ghcr.io/username/nama-repo/backend:latest
```

Contoh `docker-compose.yml` untuk server:

```yaml
services:
  backend-1:
    image: ghcr.io/username/nama-repo/backend:latest
    ports:
      - "5030:5030"

  backend-2:
    image: ghcr.io/username/nama-repo/backend:latest
    ports:
      - "5031:5030"

  frontend:
    image: ghcr.io/username/nama-repo/frontend:latest
    ports:
      - "8000:80"

  redis:
    image: redis:8-alpine
    ports:
      - "6379:6379"
```

Ganti:

```text
username/nama-repo
```

dengan repository GitHub asli.

Contoh:

```text
ghcr.io/johndoe/todo-fastendpoints/backend:latest
ghcr.io/johndoe/todo-fastendpoints/frontend:latest
```

Pengaruhnya:

```text
server tidak build image sendiri
server hanya pull image yang sudah dibuat GitHub Actions
deploy lebih cepat dan lebih konsisten
```

### 5. Tambahkan dan Jalankan Deploy Job

Status:

```text
sudah ada di file ci.yml, tetapi belum bisa dianggap selesai sebelum secrets dan VM siap
```

Bagian workflow:

```yaml
deploy:
  name: Deploy to Ubuntu VM
  runs-on: ubuntu-latest
  needs: [docker-publish]
```

Artinya:

```text
deploy hanya jalan setelah docker-publish sukses
```

Bagian SSH:

```yaml
- name: Deploy with SSH
  uses: appleboy/ssh-action@v1
  with:
    host: ${{ secrets.SSH_HOST }}
    username: ${{ secrets.SSH_USER }}
    key: ${{ secrets.SSH_KEY }}
    port: ${{ secrets.SSH_PORT }}
    script: |
      cd ~/todo-fastendpoints
      echo "${{ secrets.GHCR_TOKEN }}" | docker login ghcr.io -u "${{ secrets.GHCR_USERNAME }}" --password-stdin
      docker compose pull
      docker compose up -d
```

Penjelasan:

```text
appleboy/ssh-action membuka koneksi SSH dari GitHub runner ke VM.
Semua command di dalam script dijalankan di VM, bukan di GitHub runner.
```

Command:

```bash
cd ~/todo-fastendpoints
```

Masuk ke folder di VM yang berisi `docker-compose.yml`.

Command:

```bash
echo "${{ secrets.GHCR_TOKEN }}" | docker login ghcr.io -u "${{ secrets.GHCR_USERNAME }}" --password-stdin
```

Login VM ke GHCR agar bisa pull image private.

Command:

```bash
docker compose pull
```

VM mengambil image terbaru dari GHCR berdasarkan `image:` di `docker-compose.yml`.

Command:

```bash
docker compose up -d
```

VM menjalankan container terbaru di background.

## Setup SSH Di VM

### 1. Buat SSH Key Untuk Deployment

Bisa dibuat di laptop/local machine:

```bash
ssh-keygen -t ed25519 -C "github-actions-deploy" -f github-actions-deploy
```

Hasilnya:

```text
github-actions-deploy        private key
github-actions-deploy.pub    public key
```

Private key:

```text
dimasukkan ke GitHub secret SSH_KEY
```

Public key:

```text
dimasukkan ke VM di ~/.ssh/authorized_keys
```

### 2. Pasang Public Key Di VM

Login ke VM manual sekali:

```bash
ssh ubuntu@IP_SERVER
```

Buat folder `.ssh` jika belum ada:

```bash
mkdir -p ~/.ssh
chmod 700 ~/.ssh
```

Tambahkan isi file `github-actions-deploy.pub` ke:

```bash
~/.ssh/authorized_keys
```

Lalu set permission:

```bash
chmod 600 ~/.ssh/authorized_keys
```

### 3. Pastikan User Bisa Menjalankan Docker

Cek:

```bash
docker ps
```

Jika butuh sudo, tambahkan user ke group docker:

```bash
sudo usermod -aG docker ubuntu
```

Logout lalu login lagi.

### 4. Siapkan Folder Deploy Di VM

Workflow saat ini memakai:

```bash
cd ~/todo-fastendpoints
```

Jadi di VM perlu ada folder:

```text
~/todo-fastendpoints
```

Di dalam folder itu perlu ada:

```text
docker-compose.yml
```

Untuk flow GHCR, server tidak wajib menyimpan seluruh source code. Yang penting ada file compose untuk menjalankan image.

## Checklist Akhir

Sebelum auto deploy dianggap selesai, pastikan:

```text
[ ] ci.yml sudah dipush ke GitHub
[ ] GitHub Actions backend job hijau
[ ] GitHub Actions frontend job hijau
[ ] docker-publish job hijau
[ ] package backend muncul di GHCR
[ ] package frontend muncul di GHCR
[ ] GHCR_USERNAME sudah dibuat di GitHub Secrets
[ ] GHCR_TOKEN sudah dibuat di GitHub Secrets
[ ] SSH_HOST sudah dibuat di GitHub Secrets
[ ] SSH_USER sudah dibuat di GitHub Secrets
[ ] SSH_KEY sudah dibuat di GitHub Secrets
[ ] SSH_PORT sudah dibuat di GitHub Secrets
[ ] docker-compose.yml di VM memakai image GHCR
[ ] user SSH di VM bisa menjalankan docker compose
[ ] deploy job berhasil
[ ] aplikasi bisa diakses dari browser
```

## Catatan Penting

Jika package GHCR private, VM harus login ke GHCR.

Jika package GHCR public, VM mungkin bisa pull tanpa login.

Namun untuk latihan CI/CD yang lebih realistis, tetap gunakan:

```text
GHCR_USERNAME
GHCR_TOKEN
```

Jika deploy gagal, baca job:

```text
Deploy to Ubuntu VM
```

Lihat error apakah berasal dari:

```text
SSH gagal connect
folder ~/todo-fastendpoints tidak ada
docker login ghcr.io gagal
docker compose pull gagal
container gagal start
```

## Ringkasan Alur Final

```text
Developer push ke main
-> GitHub Actions menjalankan backend build
-> GitHub Actions menjalankan frontend build
-> GitHub Actions build Docker image
-> GitHub Actions push image ke GHCR
-> GitHub Actions SSH ke VM
-> VM login GHCR
-> VM pull image terbaru
-> VM restart container dengan docker compose up -d
```
