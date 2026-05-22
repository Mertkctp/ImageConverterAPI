# Multi-Format Image & Document Converter Station 🚀

Bu proje, .NET 8 Web API mimarisi kullanılarak geliştirilmiş, yüksek performanslı ve asenkron bir görsel format dönüştürme ve optimize etme istasyonudur. Kullanıcılar tekli veya toplu (bulk) şekilde görsellerini yükleyebilir, kalite oranlarını (sıkıştırma) belirleyebilir ve pürüzsüz bir sürükle-bırak arayüzü üzerinden çıktılarını (görsel veya birleştirilmiş PDF) alabilirler.

## 🌟 Öne Çıkan Özellikler
- **Çoklu Format Desteği:** PNG, JPEG, WebP formatları arasında anlık kayıpsız/kayıplı dönüşüm.
- **Toplu İşlem & ZIP Entegrasyonu:** Birden fazla görseli aynı anda işleyip sunucu diskini kirletmeden RAM üzerinde (`MemoryStream`) ZIP'leyerek teslim etme.
- **Görselden PDF'e (Belge Birleştirme):** Yüklenen farklı formattaki görselleri otomatik olarak standart bir PDF dökümanının sayfaları haline getirme (`PdfSharpCore` entegrasyonu).
- **Dinamik Kalite Optimizasyonu (Sıkıştırma):** Encoder seviyesinde kalite çubuğu (slider) ile dosya boyutlarını optimize edebilme.
- **Modern Ön Yüz (SPA):** Backend API ile CORS üzerinden konuşan, sürükle-bırak (Drag and Drop) destekli, dinamik geri bildirimli saf HTML5/CSS3/JavaScript arayüzü.

## 🛠️ Kullanılan Teknolojiler & Mimari
- **Backend:** C#, .NET 8 Web API, Asenkron Programlama (`async-await`)
- **Görsel İşleme:** SixLabors.ImageSharp (Sürüm 2.1.9 - Kararlı/Ücretsiz)
- **Doküman Yönetimi:** PdfSharpCore
- **Frontend:** HTML5, CSS3 (Modern UI/UX UI), Vanilla JavaScript (Fetch API & Blob Management)
- **Performans:** Tüm dosya akışları sunucu diskine yazılmadan tamamen RAM üzerinde (`MemoryStream` ve `byte[]`) işlenmiştir.

 ## 📱 Uygulama Görselleri
| Yükleme | Ana Ekran |
| :---: | :---: |
| <img src="https://github.com/user-attachments/assets/e3dd88a4-b693-4d43-9c0b-610da9c9c971" width="100%" /> | <img src="https://github.com/user-attachments/assets/6b1ec78c-8acc-4920-9649-c1bc93fa6042" width="100%" /> |
| **Dönüştürme İşlemi** | **Format Seçimi** |
| <img src="https://github.com/user-attachments/assets/0625a3fe-f574-4dfe-8c30-2d839214b716" width="100%" /> | <img src="https://github.com/user-attachments/assets/023b1fba-35c9-42c6-97f7-6c9c1533b5a0" width="90%" /> |
## ⚙️ Kurulum ve Çalıştırma

1. Projeyi bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/Mewrtkctp/ImageConverterAPI.git](https://github.com/Mertkctpe/ImageConverterAPI.git)

   NOT:"Backend'in çalışması için local'de dotnet run yapılmalıdır"
