# Vehicles Control — Galeri Yönetim Sistemi

Birden fazla oto galerisinin aynı sistemi kendi verileriyle kullandığı, çok kiracılı (multi-tenant) bir galeri yönetim paneli.
Backend **.NET 10 / ASP.NET Core / EF Core 10**, arayüz **Blazor (Interactive Server) + MudBlazor 9**.

## Neler var?

| Modül | Özellikler |
| --- | --- |
| **Araç stoku** | Marka/model/paket, yıl, km, yakıt, vites, kasa, renk, motor; plaka ve şasi no; tramer, boyalı ve değişen parçalar; alış tarihi, alış fiyatı ve satıcı; satış fiyatı; durum (stokta / rezerve / satıldı); fotoğraf galerisi ve kapak fotoğrafı |
| **Müşteriler** | Müşteri kartı, alım sayısı ve toplam harcama |
| **Satışlar** | Araçtan satış kaydı (kapora, ödeme şekli), araç bazlı maliyet ve kâr, tarih aralığına göre ciro ve kâr toplamları, satış iptali |
| **Masraflar** | Araca bağlı masraflar (aracın maliyetine eklenir) ve genel giderler; kategori ve tarih filtresi |
| **Hatırlatıcılar** | Muayene, trafik sigortası, kasko ve bakım tarihleri; son 14 gün kala uygulama içi bildirim (Hangfire) |
| **Panel** | Stok değeri ve maliyeti, aylık satış/ciro/kâr, son 6 ay grafiği, yaklaşan işler, son satışlar, ortalama stokta kalma süresi |
| **Ekip** | Galeri sahibi personel ekler veya pasif yapar |
| **Süper admin** | Galerileri ve sahiplerini oluşturur, düzenler, pasif yapar; sistem geneli sayılar |

### Roller

| Rol | Yetki |
| --- | --- |
| Süper admin | Yalnızca galeri yönetimi. Galerilerin iş verisini görmez. |
| Galeri sahibi | Kendi galerisinde her şey: ekip, galeri ayarları, araç silme, satış iptali dahil. |
| Personel | Araç, müşteri, satış, masraf ve hatırlatıcı işlemleri. Ekip, ayarlar, silme ve iptal yetkisi yoktur. |

Galeriler dışarıdan kayıt olamaz; hesapları süper admin açar.

## Çok kiracılık nasıl çalışıyor?

- Tek veritabanı vardır; galeriye ait her tabloda `GalleryId` sütunu bulunur (`ITenantEntity`).
- `GalleryId` JWT'deki `gallery_id` claim'inden okunur (`HttpTenantContext`).
- `VehiclesControlContext` her kiracı tablosuna **global query filter** uygular. Bir galeri başka galerinin kaydına ID ile ulaşmaya çalışırsa 404 alır.
- Yeni kayıtlarda `GalleryId` otomatik yazılır ve sonradan değiştirilemez.
- Galeri pasif yapılırsa kullanıcıları giriş yapamaz.
- Testler bu izolasyonu ayrıca doğrular (`VehiclesControl.Tests/Tenancy`).

## Proje yapısı

```
VehiclesControl.Domain          Entity'ler, enum'lar, istek/yanıt DTO'ları (Web ile paylaşılır)
VehiclesControl.Application     Servisler, FluentValidation kuralları, JWT, IAppDbContext/ITenantContext soyutlamaları
VehiclesControl.Data            EF Core DbContext (kiracı filtreleri), migration'lar
VehiclesControl.Infrastructure  DI kurulumları, JWT ve yetki politikaları, kiracı bağlamı, dosya deposu
VehiclesControl.API             REST API, ProblemDetails, OpenAPI/Scalar, sağlık ve metrik uçları, geliştirme seed'i
VehiclesControl.Web             Blazor arayüzü
VehiclesControl.Hangfire        Hatırlatıcı bildirim işi (her gün 08:00)
VehiclesControl.Tests           xUnit testleri (EF InMemory)
```

## Çalıştırma

Gereksinimler: .NET 10 SDK ve SQL Server (varsayılan bağlantı `Server=.`, veritabanı `VehiclesControlGallery`).

```bash
dotnet run --project VehiclesControl.API --launch-profile https
```

```bash
dotnet run --project VehiclesControl.Web --launch-profile https
```

```bash
dotnet run --project VehiclesControl.Hangfire
```

- API ilk açılışta migration'ları uygular. Development ortamında veritabanı boşsa örnek veri yükler.
- Arayüz: https://localhost:7015. API dokümanı: https://localhost:7264/scalar. Hangfire paneli: `/hangfire` (dev: admin / 123456).
- Tarayıcı sertifika uyarısı verirse geliştirme sertifikasına bir kez güvenin:

```bash
dotnet dev-certs https --trust
```

### Geliştirme hesapları ve örnek veri

Development ortamında veritabanı boşsa API beş galeri oluşturur. Her galerinin kendi ekibi, araç stoku (fotoğrafsız), müşterileri, son 6 aya yayılmış satışları, masrafları, hatırlatıcıları ve bildirimleri vardır. Veri sabit tohumla üretildiği için her sıfırlamada aynı çıkar. Hesaplar login sayfasında da listelenir, tıklayınca form dolar.

| Rol | E-posta | Şifre |
| --- | --- | --- |
| Süper admin | admin@vehiclescontrol.local | Admin123! |
| Galeri sahibi · Demo Otomotiv (İstanbul, 28 araç) | demo@galeri.local | Demo1234! |
| Personel · Demo Otomotiv | personel@galeri.local | Demo1234! |
| Galeri sahibi · Anadolu Oto (Ankara, 18 araç) | anadolu@galeri.local | Demo1234! |
| Galeri sahibi · Ege Otomotiv (İzmir, 22 araç) | ege@galeri.local | Demo1234! |
| Galeri sahibi · Marmara Motors (Bursa, 14 araç) | marmara@galeri.local | Demo1234! |
| Galeri sahibi · Akdeniz Oto (Antalya, **pasif**) | akdeniz@galeri.local | Demo1234! — pasif galeri olduğu için giriş reddedilir |

Örnek veriyi baştan üretmek için veritabanını silip API'yi yeniden başlatın:

```bash
dotnet ef database drop --force --project VehiclesControl.Data --startup-project VehiclesControl.Data
```

### Production ayarları

JWT anahtarı ve bağlantı bilgisi dosyada tutulmaz; ortam değişkeni veya user-secrets ile verilir:

```bash
dotnet user-secrets set "Jwt:SecretKey" "<en az 32 karakter>" --project VehiclesControl.API
```

- Seed'i kapatmak için `Seed:DemoData=false` yapın ve `Seed:SuperAdminEmail` / `Seed:SuperAdminPassword` değerlerini ortam değişkeniyle verin.
- Araç fotoğrafları varsayılan olarak `VehiclesControl.API/wwwroot/uploads` altına yazılır (`FileStorage:RootPath`). Rastgele adlarla saklanır ve Web uygulaması `/media` üzerinden proxy'ler. Bulut depolama için `IFileStorage`'ın başka bir uygulamasını yazmak yeterli.

## Migration

```bash
dotnet tool restore
```

```bash
dotnet ef migrations add <Ad> --project VehiclesControl.Data --startup-project VehiclesControl.Data
```

## API (özet)

| Yol | Açıklama |
| --- | --- |
| `POST /api/v1/auth/login`, `GET /api/v1/auth/me`, `PUT /api/v1/auth/profile` | Oturum ve profil |
| `/api/v1/admin/summary`, `/api/v1/admin/galleries` | Süper admin: galeriler |
| `/api/v1/gallery`, `/api/v1/gallery/team` | Kendi galerisi ve ekibi |
| `/api/v1/vehicles` (+ `/options`, `/brands`, `/{id}/status`, `/{id}/photos`) | Araç stoku |
| `/api/v1/customers`, `/api/v1/sales`, `/api/v1/expenses`, `/api/v1/reminders` | İş modülleri (sayfalı ve filtreli) |
| `/api/v1/dashboard`, `/api/v1/notifications` | Panel ve bildirimler |
| `/health`, `/metrics` | Sağlık kontrolü ve Prometheus metrikleri |

Başarılı yanıtlar `{ status, code, message, data }` zarfıyla döner; `code` HTTP durum koduyla aynıdır.
Doğrulama ve sunucu hataları ProblemDetails formatındadır.

## Testler

```bash
dotnet test
```
