# TinyHouse Veritabanı Yapısı

Bu repoda, bir kiralık konut platformu olan **TinyHouse** projesi için tasarlanmış NoSQL veritabanı yapısı yer almaktadır. Uygulama MongoDB ile çalışacak şekilde kurgulanmıştır.

## 🏠 Amaç

* Kiralık ilan, kullanıcı ve rezervasyon verilerini düzenli bir yapıda tutmak
* Airbnb tarzı platformlar için ölçeklenebilir NoSQL çözümler sağlamak

##  İçerik

* Kullanıcılar: Ev sahibi ve kiracı profilleri
* İlanlar: Lokasyon, fotoğraflar, açıklama ve özellikler
* Rezervasyonlar: Tarihler, toplam tutar, işlem bilgileri

##  Dosyalar

* `users.json` – Kullanıcı veri örneği
* `listings.json` – İlanlar veri yapısı
* `bookings.json` – Rezervasyon veri yapısı

##  Teknolojiler

* MongoDB
* JSON veri modelleme
* (Opsiyonel) Mongoose şemaları (Node.js üzerinden)

---

Bu veritabanı modeli, TinyHouse full-stack projesinin arka planını oluşturur. Gerçek dünya senaryolarıyla uyumlu olacak şekilde tasarlanmıştır.
