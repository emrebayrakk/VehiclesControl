using Microsoft.AspNetCore.Identity;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;

namespace VehiclesControl.API.Startup;

/// <summary>
/// Realistic sample data for local development: several galleries in different cities, each with its
/// own team, stock, customers, six months of sales, expenses, reminders and notifications.
/// Generated with fixed random seeds, so every reset produces the same data.
/// GalleryId is set explicitly because there is no signed-in tenant while seeding.
/// </summary>
internal static class DemoData
{
    private sealed record GallerySeed(
        string Name, string City, string PlateCode, string Phone, string Address, bool IsActive,
        (string First, string Last, string Email) Owner,
        (string First, string Last, string Email)[] Staff,
        int VehicleCount, int CustomerCount);

    private sealed record ModelSeed(
        string Brand, string Model, string[] Trims, BodyType Body, FuelType[] Fuels, TransmissionType[] Transmissions,
        int Cc, int Hp, decimal NewPrice);

    public static readonly (string Label, string Email)[] Accounts =
    [
        ("Galeri sahibi · Demo Otomotiv", "demo@galeri.local"),
        ("Personel · Demo Otomotiv", "personel@galeri.local"),
        ("Galeri sahibi · Anadolu Oto", "anadolu@galeri.local"),
        ("Galeri sahibi · Ege Otomotiv", "ege@galeri.local"),
        ("Galeri sahibi · Marmara Motors", "marmara@galeri.local"),
    ];

    private static readonly GallerySeed[] Galleries =
    [
        new("Demo Otomotiv", "İstanbul", "34", "0212 555 01 01", "Maslak Mah. Oto Plaza No:12 Sarıyer/İstanbul", true,
            ("Emre", "Bayrak", "demo@galeri.local"),
            [("Ayşe", "Demir", "personel@galeri.local"), ("Murat", "Koç", "murat@demootomotiv.local")],
            VehicleCount: 28, CustomerCount: 30),
        new("Anadolu Oto", "Ankara", "06", "0312 444 06 06", "Ostim OSB 1234. Cad. No:8 Yenimahalle/Ankara", true,
            ("Can", "Öztürk", "anadolu@galeri.local"),
            [("Selin", "Aksoy", "selin@anadoluoto.local")],
            VehicleCount: 18, CustomerCount: 20),
        new("Ege Otomotiv", "İzmir", "35", "0232 333 35 35", "Bornova Oto Galericiler Sitesi B Blok No:4 Bornova/İzmir", true,
            ("Deniz", "Aydın", "ege@galeri.local"),
            [("Okan", "Yalçın", "okan@egeotomotiv.local")],
            VehicleCount: 22, CustomerCount: 24),
        new("Marmara Motors", "Bursa", "16", "0224 222 16 16", "Nilüfer Oto Center No:21 Nilüfer/Bursa", true,
            ("Burcu", "Erdem", "marmara@galeri.local"),
            [],
            VehicleCount: 14, CustomerCount: 15),
        new("Akdeniz Oto", "Antalya", "07", "0242 111 07 07", "Kepez Sanayi Sitesi 5. Blok No:3 Kepez/Antalya", false,
            ("Kerem", "Polat", "akdeniz@galeri.local"),
            [],
            VehicleCount: 8, CustomerCount: 8),
    ];

    private static readonly FuelType[] Gas = [FuelType.Gasoline];
    private static readonly FuelType[] Diesel = [FuelType.Diesel];
    private static readonly FuelType[] GasOrDiesel = [FuelType.Gasoline, FuelType.Diesel];
    private static readonly FuelType[] GasOrLpg = [FuelType.Gasoline, FuelType.Lpg];
    private static readonly TransmissionType[] Manual = [TransmissionType.Manual];
    private static readonly TransmissionType[] Auto = [TransmissionType.Automatic];
    private static readonly TransmissionType[] Any = [TransmissionType.Manual, TransmissionType.Automatic];

    // Approximate 2026 new-car prices (TL), used as the base for depreciation.
    private static readonly ModelSeed[] Catalog =
    [
        new("Renault", "Clio", ["1.0 TCe Touch", "1.0 TCe Joy", "1.5 dCi Icon"], BodyType.Hatchback, GasOrDiesel, Any, 999, 90, 1_250_000),
        new("Renault", "Megane", ["1.3 TCe Touch", "1.5 Blue dCi Icon"], BodyType.Sedan, GasOrDiesel, Auto, 1332, 140, 1_550_000),
        new("Fiat", "Egea", ["1.4 Fire Easy", "1.3 Multijet Urban", "1.6 Multijet Lounge"], BodyType.Sedan, [FuelType.Gasoline, FuelType.Diesel, FuelType.Lpg], Any, 1368, 95, 1_100_000),
        new("Fiat", "Egea Cross", ["1.4 Fire Street", "1.6 Multijet Lounge"], BodyType.Hatchback, GasOrDiesel, Any, 1598, 130, 1_300_000),
        new("Toyota", "Corolla", ["1.5 Dream", "1.8 Hybrid Flame", "1.5 Vision"], BodyType.Sedan, [FuelType.Gasoline, FuelType.Hybrid], Auto, 1490, 125, 1_650_000),
        new("Toyota", "C-HR", ["1.8 Hybrid Flame", "2.0 Hybrid Passion"], BodyType.Suv, [FuelType.Hybrid], Auto, 1798, 122, 2_150_000),
        new("Volkswagen", "Passat", ["1.5 TSI Business", "1.6 TDI Highline", "2.0 TDI Elegance"], BodyType.Sedan, GasOrDiesel, Auto, 1598, 120, 2_600_000),
        new("Volkswagen", "Golf", ["1.0 eTSI Life", "1.5 eTSI Style", "1.6 TDI Comfortline"], BodyType.Hatchback, GasOrDiesel, Auto, 1498, 150, 1_900_000),
        new("Volkswagen", "Polo", ["1.0 TSI Life", "1.0 Trendline"], BodyType.Hatchback, Gas, Any, 999, 95, 1_350_000),
        new("Hyundai", "i20", ["1.4 MPI Jump", "1.4 MPI Elite"], BodyType.Hatchback, Gas, Any, 1368, 100, 1_150_000),
        new("Hyundai", "Tucson", ["1.6 T-GDI Prime", "1.6 CRDi Elite"], BodyType.Suv, GasOrDiesel, Auto, 1598, 180, 2_350_000),
        new("Ford", "Focus", ["1.5 TDCi Trend X", "1.0 EcoBoost Titanium"], BodyType.Hatchback, GasOrDiesel, Any, 1499, 120, 1_500_000),
        new("Ford", "Kuga", ["1.5 EcoBlue Style", "2.5 FHEV ST-Line"], BodyType.Suv, [FuelType.Diesel, FuelType.Hybrid], Auto, 1499, 150, 2_400_000),
        new("Opel", "Astra", ["1.4 Turbo Edition", "1.2 Turbo Elegance"], BodyType.Hatchback, Gas, Auto, 1399, 150, 1_600_000),
        new("Opel", "Corsa", ["1.2 Edition", "1.2 Turbo GS Line"], BodyType.Hatchback, Gas, Any, 1199, 100, 1_200_000),
        new("Peugeot", "208", ["1.2 PureTech Active", "1.2 PureTech Allure"], BodyType.Hatchback, Gas, Any, 1199, 100, 1_250_000),
        new("Peugeot", "3008", ["1.5 BlueHDi Allure", "1.5 BlueHDi GT Line"], BodyType.Suv, Diesel, Auto, 1499, 130, 2_300_000),
        new("Dacia", "Duster", ["1.3 TCe Prestige", "1.5 Blue dCi Comfort"], BodyType.Suv, GasOrDiesel, Manual, 1332, 150, 1_350_000),
        new("Dacia", "Sandero Stepway", ["1.0 TCe Comfort", "1.0 ECO-G Expression"], BodyType.Hatchback, GasOrLpg, Manual, 999, 90, 1_050_000),
        new("Skoda", "Octavia", ["1.5 TSI Premium", "1.6 TDI Style"], BodyType.Sedan, GasOrDiesel, Auto, 1498, 150, 1_950_000),
        new("Skoda", "Superb", ["1.5 TSI Prestige", "2.0 TDI L&K"], BodyType.Sedan, GasOrDiesel, Auto, 1498, 150, 2_700_000),
        new("Honda", "Civic", ["1.5 VTEC Eco Elegance", "1.6 i-DTEC Executive"], BodyType.Sedan, [FuelType.Gasoline, FuelType.Lpg], Auto, 1498, 129, 1_800_000),
        new("BMW", "3 Serisi", ["320i ED Luxury Line", "320d M Sport", "318i First Edition"], BodyType.Sedan, GasOrDiesel, Auto, 1597, 170, 3_400_000),
        new("BMW", "5 Serisi", ["520i Special Edition", "520d Executive"], BodyType.Sedan, GasOrDiesel, Auto, 1995, 190, 4_600_000),
        new("Mercedes-Benz", "C Serisi", ["C 200 d AMG", "C 180 Exclusive", "C 200 Avantgarde"], BodyType.Sedan, GasOrDiesel, Auto, 1598, 136, 3_700_000),
        new("Mercedes-Benz", "A Serisi", ["A 180 Style", "A 200 AMG"], BodyType.Hatchback, Gas, Auto, 1332, 136, 2_450_000),
        new("Audi", "A3", ["35 TFSI Advanced", "30 TDI Dynamic"], BodyType.Sedan, GasOrDiesel, Auto, 1498, 150, 2_500_000),
        new("Audi", "A4", ["40 TDI quattro Design", "2.0 TFSI Sport"], BodyType.Sedan, GasOrDiesel, Auto, 1968, 190, 3_500_000),
        new("Citroën", "C4", ["1.2 PureTech Feel", "1.5 BlueHDi Shine"], BodyType.Hatchback, GasOrDiesel, Auto, 1199, 130, 1_550_000),
        new("Kia", "Sportage", ["1.6 T-GDI Prestige", "1.6 CRDi Elegance"], BodyType.Suv, GasOrDiesel, Auto, 1598, 180, 2_300_000),
        new("Nissan", "Qashqai", ["1.3 DIG-T Tekna", "1.5 dCi Sky Pack"], BodyType.Suv, GasOrDiesel, Auto, 1332, 158, 2_100_000),
        new("Togg", "T10X", ["V1 RWD Standart Menzil", "V2 RWD Uzun Menzil"], BodyType.Suv, [FuelType.Electric], Auto, 0, 218, 1_900_000),
        new("Ford", "Transit Custom", ["2.0 EcoBlue Trend"], BodyType.Van, Diesel, Manual, 1995, 130, 1_700_000),
        new("Fiat", "Doblo", ["1.6 Multijet Safeline", "1.3 Multijet Easy"], BodyType.Mpv, Diesel, Manual, 1598, 105, 1_050_000),
        new("Toyota", "Hilux", ["2.4 D-4D Adventure"], BodyType.Pickup, Diesel, Auto, 2393, 150, 2_500_000),
    ];

    private static readonly string[] Colors = ["Beyaz", "Beyaz", "Siyah", "Gri", "Gri", "Gümüş", "Füme", "Kırmızı", "Mavi", "Lacivert", "Kahverengi", "Bej", "Turuncu", "Yeşil", "Bordo"];

    private static readonly string[] FirstNames =
    [
        "Mehmet", "Ahmet", "Mustafa", "Ali", "Hüseyin", "Hasan", "İbrahim", "Yusuf", "Ömer", "Murat", "Emre", "Burak", "Serkan", "Volkan",
        "Kemal", "Tolga", "Onur", "Barış", "Cem", "Uğur", "Zeynep", "Elif", "Ayşe", "Fatma", "Emine", "Merve", "Esra", "Büşra", "Derya",
        "Gamze", "Seda", "Ebru", "Pınar", "Gizem", "Tuğba", "Özge", "Sibel", "Hande", "Cansu", "Melis",
    ];

    private static readonly string[] LastNames =
    [
        "Yılmaz", "Kaya", "Demir", "Şahin", "Çelik", "Yıldız", "Yıldırım", "Öztürk", "Aydın", "Özdemir", "Arslan", "Doğan", "Kılıç", "Aslan",
        "Çetin", "Kara", "Koç", "Kurt", "Özkan", "Şimşek", "Polat", "Korkmaz", "Erdoğan", "Güneş", "Aktaş", "Bulut", "Tekin", "Ünal", "Yavuz", "Acar",
    ];

    private static readonly string[] Parts = ["Kaput", "Tavan", "Bagaj kapağı", "Sol ön çamurluk", "Sağ ön çamurluk", "Sol ön kapı", "Sağ ön kapı", "Sol arka kapı", "Sağ arka kapı", "Sol arka çamurluk", "Sağ arka çamurluk", "Ön tampon", "Arka tampon"];

    private static readonly (ExpenseCategory Category, decimal Min, decimal Max, string[] Descriptions)[] VehicleExpenses =
    [
        (ExpenseCategory.Expertise, 2_000, 3_500, ["Tam ekspertiz", "Kaporta-boya ekspertizi", "Motor + yürüyen ekspertizi"]),
        (ExpenseCategory.Maintenance, 4_500, 22_000, ["Periyodik bakım", "Triger seti değişimi", "Fren balata + disk", "Debriyaj seti", "Klima bakımı"]),
        (ExpenseCategory.BodyPaint, 6_000, 28_000, ["Çamurluk lokal boya", "Tampon boya", "Kapı göçük düzeltme (PDR)"]),
        (ExpenseCategory.Tires, 9_000, 26_000, ["4 adet yaz lastiği", "4 adet kış lastiği", "2 adet ön lastik"]),
        (ExpenseCategory.Cleaning, 900, 2_500, ["Detaylı iç-dış temizlik", "Pasta cila", "Motor temizliği"]),
        (ExpenseCategory.Transport, 2_500, 9_000, ["Çekici ile nakliye", "Şehirlerarası taşıma"]),
        (ExpenseCategory.Notary, 2_800, 3_600, ["Noter satış masrafı"]),
    ];

    public static async Task SeedAsync(VehiclesControlContext db, IPasswordHasher<User> hasher, string password)
    {
        for (var i = 0; i < Galleries.Length; i++)
            await SeedGalleryAsync(db, hasher, password, Galleries[i], new Random(1000 + i));
    }

    private static async Task SeedGalleryAsync(VehiclesControlContext db, IPasswordHasher<User> hasher, string password, GallerySeed seed, Random rnd)
    {
        var today = DateTime.Today;

        var gallery = new Gallery
        {
            Name = seed.Name,
            City = seed.City,
            Phone = seed.Phone,
            Address = seed.Address,
            Email = $"info@{Slug(seed.Name)}.local",
            TaxNumber = rnd.NextInt64(1_000_000_000, 9_999_999_999).ToString(),
            IsActive = seed.IsActive,
        };
        db.Galleries.Add(gallery);
        await db.SaveChangesAsync();

        var team = new List<User> { NewUser(seed.Owner, UserRole.GalleryOwner) };
        team.AddRange(seed.Staff.Select(s => NewUser(s, UserRole.GalleryStaff)));
        db.Users.AddRange(team);

        // ----- Customers -----
        var customers = Enumerable.Range(0, seed.CustomerCount).Select(_ =>
        {
            var first = Pick(rnd, FirstNames);
            var last = Pick(rnd, LastNames);
            return new Customer
            {
                GalleryId = gallery.Id,
                FirstName = first,
                LastName = last,
                Phone = Phone(rnd),
                Email = rnd.NextDouble() < .6 ? $"{Ascii(first)}.{Ascii(last)}{rnd.Next(1, 99)}@example.com" : null,
                City = rnd.NextDouble() < .75 ? seed.City : Pick(rnd, ["İstanbul", "Ankara", "İzmir", "Bursa", "Kocaeli", "Antalya", "Konya", "Eskişehir"]),
                Notes = rnd.NextDouble() < .15 ? Pick(rnd, ["Takas düşünüyor", "SUV arıyor, bütçe 1,5 milyon", "Kredi ile alım yapacak", "Referansla geldi"]) : null,
            };
        }).ToList();
        db.Customers.AddRange(customers);

        // ----- Vehicles -----
        var usedVins = new HashSet<string>();
        var vehicles = new List<Vehicle>();
        for (var i = 0; i < seed.VehicleCount; i++)
        {
            var model = Pick(rnd, Catalog);
            var age = rnd.Next(0, 10);
            var year = today.Year - age;
            var km = Math.Max(age == 0 ? rnd.Next(5, 40) * 100 : 0, (int)Math.Round(age * rnd.Next(11_000, 24_000) / 500.0) * 500);
            var fuel = Pick(rnd, model.Fuels);

            var value = model.NewPrice * (decimal)Math.Pow(0.89, age) * (1 - Math.Min(km, 250_000) / 1_000_000m);
            var list = RoundTo(value * (decimal)(0.96 + rnd.NextDouble() * 0.08), 5_000);
            var buy = RoundTo(list * (decimal)(0.86 + rnd.NextDouble() * 0.06), 5_000);

            var damaged = age > 1 && rnd.NextDouble() < .45;
            var painted = damaged ? Pick(rnd, Parts, rnd.Next(1, 3)) : null;
            var replaced = damaged && rnd.NextDouble() < .3 ? Pick(rnd, Parts, 1) : null;

            vehicles.Add(new Vehicle
            {
                GalleryId = gallery.Id,
                Brand = model.Brand,
                Model = model.Model,
                Trim = Pick(rnd, model.Trims),
                Year = year,
                Kilometers = km,
                FuelType = fuel,
                Transmission = Pick(rnd, model.Transmissions),
                BodyType = model.Body,
                Color = Pick(rnd, Colors),
                EngineVolume = model.Cc > 0 ? model.Cc : null,
                EnginePower = model.Hp,
                Plate = Plate(rnd, seed.PlateCode),
                Vin = rnd.NextDouble() < .8 ? Vin(rnd, usedVins) : null,
                DamageRecordAmount = damaged ? RoundTo((decimal)(rnd.NextDouble() * 85_000 + 4_000), 250) : null,
                PaintedParts = painted,
                ReplacedParts = replaced,
                PurchasePrice = buy,
                ListPrice = list,
                // Unsold stock was bought within the last 4 months; sold cars get dates assigned below.
                PurchaseDate = today.AddDays(-rnd.Next(2, 120)),
                SellerName = rnd.NextDouble() < .7 ? $"{Pick(rnd, FirstNames)} {Pick(rnd, LastNames)}" : Pick(rnd, ["Bayi takası", "Filo kiralama şirketi", "Oto ihale"]),
                SellerPhone = rnd.NextDouble() < .6 ? Phone(rnd) : null,
                Status = VehicleStatus.InStock,
                Notes = rnd.NextDouble() < .25 ? Pick(rnd, ["Tek elden, bakımları yetkili serviste.", "Garaj arabası, sigara içilmemiş.", "Yedek anahtar mevcut.", "Kış lastikleri ile birlikte.", "Garantisi devam ediyor."]) : null,
            });
        }
        db.Vehicles.AddRange(vehicles);
        await db.SaveChangesAsync();

        // ----- Sales: about 40% sold, spread over the last six months (a few this month) -----
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var soldCount = (int)Math.Round(vehicles.Count * 0.4);
        var sold = vehicles.OrderBy(_ => rnd.Next()).Take(soldCount).ToList();
        for (var i = 0; i < sold.Count; i++)
        {
            var vehicle = sold[i];
            var saleDate = i < Math.Max(2, sold.Count / 5)
                ? monthStart.AddDays(rnd.Next(0, Math.Max(1, today.Day)))
                : today.AddDays(-rnd.Next(1, 180));
            vehicle.PurchaseDate = saleDate.AddDays(-rnd.Next(12, 75));
            vehicle.Status = VehicleStatus.Sold;

            var price = RoundTo(vehicle.ListPrice * (decimal)(0.95 + rnd.NextDouble() * 0.05), 5_000);
            db.Sales.Add(new Sale
            {
                GalleryId = gallery.Id,
                VehicleId = vehicle.Id,
                CustomerId = Pick(rnd, customers).Id,
                SaleDate = saleDate,
                SalePrice = price,
                Deposit = rnd.NextDouble() < .45 ? RoundTo(price * (decimal)(0.05 + rnd.NextDouble() * 0.1), 5_000) : 0,
                PaymentMethod = Pick(rnd, [PaymentMethod.BankTransfer, PaymentMethod.BankTransfer, PaymentMethod.Cash, PaymentMethod.Credit, PaymentMethod.Mixed]),
                SoldByUserId = Pick(rnd, team).Id,
                Notes = rnd.NextDouble() < .2 ? Pick(rnd, ["Takaslı satış", "Noter randevusu galeride yapıldı", "Kasko satışla birlikte yapıldı"]) : null,
            });
        }

        // A couple of unsold cars are reserved.
        foreach (var vehicle in vehicles.Where(v => v.Status == VehicleStatus.InStock).OrderBy(_ => rnd.Next()).Take(Math.Max(1, vehicles.Count / 10)))
            vehicle.Status = VehicleStatus.Reserved;

        // ----- Expenses -----
        foreach (var vehicle in vehicles)
        {
            var count = rnd.Next(1, 4);
            foreach (var (category, min, max, descriptions) in VehicleExpenses.Where(e => e.Category != ExpenseCategory.Notary).OrderBy(_ => rnd.Next()).Take(count))
            {
                db.Expenses.Add(new Expense
                {
                    GalleryId = gallery.Id,
                    VehicleId = vehicle.Id,
                    Category = category,
                    Amount = RoundTo(min + (decimal)rnd.NextDouble() * (max - min), 50),
                    Date = Min(today, vehicle.PurchaseDate.AddDays(rnd.Next(1, 10))),
                    Description = Pick(rnd, descriptions),
                });
            }
        }

        foreach (var sale in db.ChangeTracker.Entries<Sale>().Select(e => e.Entity).Where(s => s.GalleryId == gallery.Id).ToList())
        {
            db.Expenses.Add(new Expense
            {
                GalleryId = gallery.Id, VehicleId = sale.VehicleId, Category = ExpenseCategory.Notary,
                Amount = RoundTo(2_800 + (decimal)rnd.NextDouble() * 800, 50), Date = sale.SaleDate, Description = "Noter satış masrafı",
            });
        }

        // General gallery costs for each of the last six months.
        for (var m = 0; m < 6; m++)
        {
            var month = monthStart.AddMonths(-m);
            db.Expenses.Add(new Expense { GalleryId = gallery.Id, Category = ExpenseCategory.Other, Amount = RoundTo(25_000 + (decimal)rnd.NextDouble() * 20_000, 500), Date = month, Description = "Showroom kirası" });
            db.Expenses.Add(new Expense { GalleryId = gallery.Id, Category = ExpenseCategory.Advertising, Amount = RoundTo(3_500 + (decimal)rnd.NextDouble() * 4_000, 100), Date = month.AddDays(4), Description = "İlan sitesi paket ücreti" });
        }

        // ----- Reminders for cars still in the gallery -----
        var reminderTypes = new[] { ReminderType.Inspection, ReminderType.TrafficInsurance, ReminderType.Casco, ReminderType.Maintenance };
        foreach (var vehicle in vehicles.Where(v => v.Status != VehicleStatus.Sold))
        {
            foreach (var type in reminderTypes.OrderBy(_ => rnd.Next()).Take(rnd.Next(1, 3)))
            {
                db.Reminders.Add(new Reminder
                {
                    GalleryId = gallery.Id,
                    VehicleId = vehicle.Id,
                    Type = type,
                    DueDate = today.AddDays(rnd.Next(-8, 75)),
                    Notes = rnd.NextDouble() < .3 ? Pick(rnd, ["TÜVTÜRK randevusu alınacak", "Teklif istendi", "Yetkili serviste yapılacak"]) : null,
                });
            }
        }

        // Some reminders were already handled.
        foreach (var vehicle in vehicles.Where(v => v.Status == VehicleStatus.Sold).Take(3))
        {
            db.Reminders.Add(new Reminder
            {
                GalleryId = gallery.Id, VehicleId = vehicle.Id, Type = ReminderType.Inspection,
                DueDate = vehicle.PurchaseDate.AddDays(10), IsCompleted = true, CompletedDate = vehicle.PurchaseDate.AddDays(8),
            });
        }

        // A few notifications as if the daily job had already run.
        foreach (var reminder in db.ChangeTracker.Entries<Reminder>().Select(e => e.Entity)
                     .Where(r => r.GalleryId == gallery.Id && !r.IsCompleted && r.DueDate <= today.AddDays(5))
                     .OrderBy(r => r.DueDate).Take(3).ToList())
        {
            var vehicle = vehicles.First(v => v.Id == reminder.VehicleId);
            var days = (reminder.DueDate - today).Days;
            db.Notifications.Add(new Notification
            {
                GalleryId = gallery.Id,
                Title = $"{Label(reminder.Type)} yaklaşıyor",
                Message = $"{vehicle.Year} {vehicle.Brand} {vehicle.Model} ({vehicle.Plate}) — {(days < 0 ? $"{-days} gün gecikti" : days == 0 ? "bugün" : $"{days} gün sonra")}.",
                Link = $"/vehicles/{vehicle.Id}",
            });
            reminder.NotifiedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        User NewUser((string First, string Last, string Email) person, UserRole role)
        {
            var user = new User
            {
                GalleryId = gallery.Id,
                FirstName = person.First,
                LastName = person.Last,
                Email = person.Email,
                Phone = Phone(rnd),
                Role = role,
                LastLoginAt = role == UserRole.GalleryOwner ? DateTime.UtcNow.AddHours(-rnd.Next(1, 72)) : null,
            };
            user.PasswordHash = hasher.HashPassword(user, password);
            return user;
        }
    }

    private static T Pick<T>(Random rnd, IReadOnlyList<T> items) => items[rnd.Next(items.Count)];

    private static string Pick(Random rnd, string[] items, int count) =>
        string.Join(", ", items.OrderBy(_ => rnd.Next()).Take(count));

    private static decimal RoundTo(decimal value, decimal step) => Math.Round(value / step) * step;

    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

    private static string Phone(Random rnd) =>
        $"05{rnd.Next(30, 56)} {rnd.Next(100, 999)} {rnd.Next(10, 99)} {rnd.Next(10, 99)}";

    private static string Plate(Random rnd, string code)
    {
        const string letters = "ABCDEFGHJKLMNPRSTUVYZ";
        var letterCount = rnd.Next(1, 4);
        var chars = new string(Enumerable.Range(0, letterCount).Select(_ => letters[rnd.Next(letters.Length)]).ToArray());
        var digits = letterCount switch { 1 => rnd.Next(1000, 9999), 2 => rnd.Next(100, 9999), _ => rnd.Next(10, 999) };
        return $"{code} {chars} {digits}";
    }

    private static string Vin(Random rnd, HashSet<string> used)
    {
        const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
        string vin;
        do
        {
            vin = new string(Enumerable.Range(0, 17).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        }
        while (!used.Add(vin));
        return vin;
    }

    private static string Ascii(string text) => text.ToLowerInvariant()
        .Replace("i̇", "i").Replace('ı', 'i').Replace('ğ', 'g').Replace('ü', 'u').Replace('ş', 's').Replace('ö', 'o').Replace('ç', 'c');

    private static string Slug(string text) => Ascii(text).Replace(" ", "");

    private static string Label(ReminderType type) => type switch
    {
        ReminderType.Inspection => "Muayene",
        ReminderType.TrafficInsurance => "Trafik sigortası",
        ReminderType.Casco => "Kasko",
        ReminderType.Maintenance => "Bakım",
        _ => "Hatırlatıcı",
    };
}
