# Roadmap Belajar Deployment Todo App
## ASP.NET FastEndpoints + React + Docker + VM + PostgreSQL + JWT + Redis + Load Balancer

> Tujuan roadmap ini:
> - membangun **project kecil tapi end-to-end** dengan mental load serendah mungkin
> - belajar **deploy aplikasi fullstack** di VM Linux
> - memahami alur **backend → frontend → Docker → Compose → auth → cache → scaling**
>
> Studi kasus:
> **Todo App** dengan backend **ASP.NET + FastEndpoints**, frontend **React**, dan deployment di **VirtualBox Linux VM**.

---

# 1. Gambaran Akhir Project

Target akhir yang ingin dicapai:

```text
todo-app/
├─ backend/
│  └─ FirstFastEndpoints/
│     ├─ Dockerfile
│     ├─ Program.cs
│     ├─ Features/
│     ├─ Entities/
│     ├─ Infrastructure/
│     └─ ...
├─ frontend/
│  └─ todo-web/
│     ├─ Dockerfile
│     ├─ src/
│     └─ ...
├─ docker-compose.yml
├─ .env
└─ README.md
```

Di VM nanti akan berjalan beberapa service:

- **backend** → ASP.NET FastEndpoints
- **frontend** → React app
- **db** → PostgreSQL
- **redis** → Redis
- **nginx** → reverse proxy / load balancer (tahap akhir)

---

# 2. Prinsip Belajar

## Fokus roadmap ini
Roadmap ini sengaja disusun **bertahap**, supaya tidak langsung lompat ke arsitektur besar.

Urutan yang dipakai:

1. **Backend API dulu**
2. **Frontend CRUD**
3. **Docker untuk masing-masing app**
4. **Docker Compose**
5. **JWT auth**
6. **Redis**
7. **Load balancer**

## Aturan pengerjaan
- jangan tambah banyak fitur sekaligus
- tiap fase harus **bisa dijalankan dan dites**
- kalau ada error, perbaiki di fase itu dulu sebelum lanjut
- prioritaskan **service sederhana** dibanding arsitektur yang terlalu kompleks

---

# 3. Progress Saat Ini

## Sudah selesai
- [x] Buat project REST API **FirstFastEndpoint** tentang todo list
- [x] Tambahkan **Dockerfile** untuk backend ASP.NET
- [x] Install **VM Linux**
- [x] Connect **SSH** ke VM
- [x] Clone project ke VM
- [x] Install **Docker** di VM
- [x] Build image dari project ASP.NET **FirstFastEndpoints**
- [x] Jalankan container API endpoint
- [x] Test API via **REST Client / Postman**

---

# 4. Roadmap Belajar

# Phase 1 — Rapikan Backend Todo API
## Tujuan
Membuat backend benar-benar siap dipakai frontend dan siap di-docker-compose nanti.

## Outcome akhir phase ini
- backend punya CRUD todo yang rapi
- struktur project backend sudah cukup stabil
- endpoint siap dikonsumsi frontend

## Checklist
- [ ] Pastikan entity Todo sudah final minimal field:
  - `Id`
  - `Title`
  - `Description`
  - `IsDone`
  - `CreatedAt`
- [ ] Rapikan endpoint:
  - [ ] `GET /todos`
  - [ ] `GET /todos/{id}`
  - [ ] `POST /todos`
  - [ ] `PUT /todos/{id}`
  - [ ] `DELETE /todos/{id}`
- [ ] Tambahkan validasi request
- [ ] Rapikan response model
- [ ] Tambahkan Swagger / OpenAPI jika belum
- [ ] Pastikan backend bisa jalan **tanpa Docker** dan **dengan Docker**

## Hasil yang harus bisa dites
- CRUD todo berhasil dari Postman / REST Client
- container backend tetap bisa dijalankan

---

# Phase 2 — Tambahkan PostgreSQL ke Backend
## Tujuan
Memindahkan data todo dari in-memory / dummy / local store ke database PostgreSQL.

## Outcome akhir phase ini
- backend menggunakan PostgreSQL
- CRUD todo tersimpan permanen di database
- project siap digabung ke compose

## Checklist
- [ ] Tambahkan PostgreSQL ke backend
- [ ] Buat connection string via environment variable
- [ ] Buat migration / schema todo
- [ ] Pastikan tabel todo berhasil dibuat
- [ ] Test CRUD todo ke PostgreSQL
- [ ] Pastikan backend di Docker bisa connect ke PostgreSQL

## Catatan implementasi
Kalau memakai PostgreSQL, biasakan nama tabel **lowercase** untuk menghindari masalah quoted identifier.
Contoh:
- `todos`
- `users`

## Hasil yang harus bisa dites
- insert todo masuk ke PostgreSQL
- update dan delete juga tercermin di database

---

# Phase 3 — Buat Frontend React Todo App
## Tujuan
Membuat frontend sederhana yang benar-benar memakai API backend.

## Outcome akhir phase ini
- ada UI CRUD todo
- frontend terhubung ke backend API
- alur fullstack pertama selesai

## Checklist
- [ ] Buat project frontend menggunakan **React**
- [ ] Tentukan stack frontend sederhana
  - contoh: React + Vite + fetch/axios
- [ ] Buat layout halaman todo:
  - [ ] list todo
  - [ ] form tambah todo
  - [ ] edit todo
  - [ ] hapus todo
  - [ ] toggle done / not done
- [ ] Consume endpoint backend:
  - [ ] get all todos
  - [ ] create todo
  - [ ] update todo
  - [ ] delete todo
- [ ] Tangani loading dan error state sederhana

## Hasil yang harus bisa dites
- buka frontend di browser
- tambah / edit / hapus todo dari UI
- perubahan tersimpan ke backend + PostgreSQL

---

# Phase 4 — Dockerize Frontend
## Tujuan
Menjalankan frontend React sebagai container terpisah.

## Outcome akhir phase ini
- frontend punya Dockerfile sendiri
- frontend bisa dijalankan via Docker

## Checklist
- [ ] Tambahkan **Dockerfile** untuk frontend
- [ ] Tentukan cara serve frontend:
  - **opsi sederhana:** Vite preview / dev server
  - **opsi lebih rapi:** build static lalu serve via Nginx
- [ ] Build image frontend
- [ ] Run container frontend
- [ ] Test frontend di browser

## Hasil yang harus bisa dites
- frontend bisa diakses dari host / VM
- frontend berhasil memanggil backend API

---

# Phase 5 — Rapikan Repository & Struktur Folder
## Tujuan
Menyatukan backend dan frontend dalam satu root project supaya mudah dikelola dengan Docker Compose.

## Outcome akhir phase ini
- struktur folder final terbentuk
- siap membuat docker-compose

## Struktur target yang disarankan
```text
todo-app/
├─ backend/
│  └─ FirstFastEndpoints/
├─ frontend/
│  └─ todo-web/
├─ docker-compose.yml
├─ .env
└─ README.md
```

## Checklist
- [ ] Buat **GitHub repository** untuk project final
- [ ] Susun folder:
  - [ ] `backend/FirstFastEndpoints`
  - [ ] `frontend/todo-web`
- [ ] Pindahkan / rapikan file Dockerfile di masing-masing app
- [ ] Tambahkan `.gitignore`
- [ ] Tambahkan `README.md`
- [ ] Push project ke GitHub

## Hasil yang harus bisa dites
- backend dan frontend bisa dibuild dari root project yang rapi

---

# Phase 6 — Docker Compose (Backend + Frontend + PostgreSQL)
## Tujuan
Menjalankan seluruh stack dengan satu command.

## Outcome akhir phase ini
- backend, frontend, dan PostgreSQL berjalan bersama
- environment variable lebih rapi
- project siap dijalankan fullstack di VM

## Service minimal di compose
- `backend`
- `frontend`
- `db`

## Checklist
- [ ] Buat file `docker-compose.yml`
- [ ] Tambahkan service **backend**
- [ ] Tambahkan service **frontend**
- [ ] Tambahkan service **PostgreSQL**
- [ ] Tambahkan **volume** untuk PostgreSQL
- [ ] Tambahkan **network** default compose
- [ ] Gunakan `.env` untuk:
  - port backend
  - port frontend
  - db name
  - db user
  - db password
- [ ] Pastikan backend memakai host database dari nama service compose, misalnya:
  - `Host=db`
- [ ] Build compose
- [ ] Jalankan compose di VM
- [ ] Test frontend → backend → database

## Contoh struktur compose target
```yaml
services:
  backend:
    build: ./backend/FirstFastEndpoints
    ports:
      - "5030:8080"

  frontend:
    build: ./frontend/todo-web
    ports:
      - "3000:3000"

  db:
    image: postgres:16
    ports:
      - "5432:5432"
```

## Hasil yang harus bisa dites
- `docker compose up --build` menjalankan seluruh stack
- frontend dapat membaca dan mengubah todo melalui backend
- backend tersambung ke PostgreSQL

---

# Phase 7 — Tambahkan JWT Bearer Authentication
## Tujuan
Belajar auth sederhana untuk API dan frontend.

## Outcome akhir phase ini
- user bisa login
- token JWT dipakai untuk mengakses endpoint tertentu
- frontend bisa menyimpan token dan mengirim Authorization header

## Scope pembelajaran
Tidak perlu langsung role/permission yang kompleks. Cukup auth dasar.

## Checklist
- [ ] Tambahkan entity / tabel user sederhana
- [ ] Buat endpoint register
- [ ] Buat endpoint login
- [ ] Generate JWT token saat login berhasil
- [ ] Tambahkan JWT bearer auth ke backend
- [ ] Lindungi endpoint tertentu dengan `[Authorize]`
- [ ] Uji endpoint tanpa token → harus gagal
- [ ] Uji endpoint dengan token → harus berhasil
- [ ] Integrasikan login di frontend
- [ ] Simpan token di frontend
- [ ] Kirim token saat mengakses endpoint protected

## Hasil yang harus bisa dites
- login berhasil dan mendapat token
- request ke endpoint protected berhasil saat token valid

---

# Phase 8 — Tambahkan Redis
## Tujuan
Belajar fungsi Redis pada aplikasi nyata, bukan sekadar install service.

## Rekomendasi use case pembelajaran
Agar tetap fokus, pilih **2 use case** berikut:

## Use Case A — Cache list todo
Cocok sebagai latihan pertama karena paling mudah dipahami.

### Ide implementasi
- saat `GET /todos`, cek cache Redis dulu
- kalau ada cache → return dari Redis
- kalau tidak ada → ambil dari DB lalu simpan ke Redis
- saat create/update/delete todo → hapus / refresh cache terkait

### Checklist
- [ ] Tambahkan service Redis di compose
- [ ] Integrasikan Redis client di backend
- [ ] Implementasikan cache untuk `GET /todos`
- [ ] Invalidate cache saat create/update/delete

---

## Use Case B — Token blacklist / session sederhana
Cocok untuk menghubungkan Redis dengan JWT.

### Ide implementasi
- saat logout, simpan token ke blacklist Redis
- middleware / endpoint check token blacklist sebelum menerima request

### Checklist
- [ ] Tambahkan mekanisme logout
- [ ] Simpan token blacklist di Redis
- [ ] Cek blacklist saat request masuk

## Hasil yang harus bisa dites
- `GET /todos` bisa diambil dari cache
- perubahan todo membuat cache diperbarui / dibersihkan
- token yang di-blacklist tidak bisa dipakai lagi

---

# Phase 9 — Tambahkan Load Balancer
## Tujuan
Belajar scaling sederhana dan konsep reverse proxy.

## Outcome akhir phase ini
- ada beberapa instance backend
- request masuk lewat load balancer
- memahami konsep routing antar container

## Pendekatan yang disarankan
Gunakan **Nginx** sebagai reverse proxy / load balancer.

## Skema target
- `backend-1`
- `backend-2`
- `nginx`

## Checklist
- [ ] Tambahkan Nginx ke project
- [ ] Buat konfigurasi upstream ke beberapa backend container
- [ ] Jalankan 2 instance backend
- [ ] Route request dari Nginx ke backend
- [ ] Test pembagian request

## Hasil yang harus bisa dites
- request ke endpoint masuk melalui Nginx
- request bisa diarahkan ke lebih dari satu backend instance

---

# 5. Urutan Belajar yang Paling Disarankan

## Tahap A — Fullstack dasar dulu
1. Rapikan backend CRUD
2. Hubungkan backend ke PostgreSQL
3. Buat frontend React CRUD
4. Dockerize frontend
5. Satukan struktur repo

## Tahap B — Jalankan fullstack di VM
6. Buat docker-compose
7. Jalankan backend + frontend + db di VM
8. Test dari Windows / browser

## Tahap C — Tambah security
9. Implement JWT bearer auth
10. Tambahkan login frontend

## Tahap D — Tambah infrastruktur
11. Tambahkan Redis untuk caching / token blacklist
12. Tambahkan load balancer dengan Nginx

---

# 6. Milestone Belajar

## Milestone 1 — Backend siap
**Target:** API CRUD todo stabil dan terhubung ke PostgreSQL

## Milestone 2 — Fullstack lokal siap
**Target:** React CRUD berjalan dan terhubung ke backend

## Milestone 3 — Docker fullstack siap
**Target:** backend + frontend + db jalan dengan Docker / Compose

## Milestone 4 — Auth siap
**Target:** login + JWT + protected endpoint berjalan

## Milestone 5 — Infrastruktur lanjutan siap
**Target:** Redis dan load balancer sudah dipahami dalam konteks project yang sama

---

# 7. Checklist Ringkas Eksekusi

## Sekarang kerjakan ini dulu
### Fokus sprint berikutnya:
- [ ] Tambahkan PostgreSQL ke backend
- [ ] Rapikan CRUD todo backend
- [ ] Buat frontend React CRUD
- [ ] Connect frontend ke backend
- [ ] Dockerize frontend

## Setelah itu lanjut:
- [ ] Rapikan struktur root project
- [ ] Buat GitHub repository
- [ ] Tambahkan docker-compose

## Setelah compose stabil:
- [ ] JWT auth
- [ ] Redis
- [ ] Load balancer

---

# 8. Catatan Penting

## Jangan langsung load balancer
Load balancer masuk **belakangan**, setelah:
- backend stabil
- frontend stabil
- database stabil
- compose stabil

## Redis juga jangan terlalu cepat
Redis paling enak dipelajari **setelah JWT** atau setelah fullstack + compose sudah jalan.

## Compose dibuat setelah struktur project jelas
Karena kamu tadi bilang belum yakin dengan struktur folder, itu keputusan yang benar:
- selesaikan backend + frontend dulu
- lalu tetapkan root project
- baru buat `docker-compose.yml`

---

# 9. Target Akhir yang Bagus untuk Portofolio
Kalau semua phase selesai, hasil project kamu bisa menjadi:

## **Todo App Fullstack on VM**
Dengan fitur:
- ASP.NET FastEndpoints backend
- PostgreSQL database
- React frontend
- Dockerized backend & frontend
- Docker Compose orchestration
- JWT authentication
- Redis caching / token blacklist
- Nginx load balancing

Ini sudah sangat bagus sebagai **project pembelajaran deployment + backend + infra dasar**.
