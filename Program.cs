using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;
using Figgle;
using Newtonsoft.Json.Linq;

// Ana sınıf (BaseClass) tanımlanıyor
public abstract class BaseClass
{
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string TC { get; set; }
    public string KullaniciAdi { get; set; }
    public string Sifre { get; set; }
    public string Email { get; set; }  // E-posta adresi eklendi

    public BaseClass(string ad, string soyad, string tc, string kullaniciAdi, string sifre, string email)
    {
        Ad = ad;
        Soyad = soyad;
        TC = tc;
        KullaniciAdi = kullaniciAdi;
        Sifre = sifre;
        Email = email;  // E-posta adresi kaydedildi
    }

    public abstract void Yazdir();
}

// Kullanıcı sınıfı, BaseClass'tan miras alır
public class Kullanici : BaseClass
{
    public Kullanici(string ad, string soyad, string tc, string kullaniciAdi, string sifre, string email)
        : base(ad, soyad, tc, kullaniciAdi, sifre, email) { }

    public override void Yazdir()
    {
        Console.WriteLine($"Kullanıcı Adı: {KullaniciAdi}, Ad: {Ad}, Soyad: {Soyad}, TC: {TC}, E-posta: {Email}");
    }
}

// Banka işlemleri sınıfı
public class BankaIslemleri
{
    public double Bakiye { get; set; }

    public BankaIslemleri()
    {
        Bakiye = 0; // Başlangıçta bakiye sıfır.
    }

    public void ParaYatir(double miktar, string email)
    {
        if (miktar > 0)
        {
            Bakiye += miktar;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{miktar} TL yatırıldı. Güncel bakiye: {Bakiye} TL");
            Console.ResetColor();

            // Para yatırma işlemi tamamlandı, e-posta gönderme
            EmailServisi.SendParaYatirDekontu(email, miktar, Bakiye);
        }
        else
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Geçersiz miktar.");
            Console.ResetColor();
        }
    }

    public void ParaCek(double miktar, string email)
    {
        if (miktar > 0 && miktar <= Bakiye)
        {
            Bakiye -= miktar;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{miktar} TL çekildi. Güncel bakiye: {Bakiye} TL");
            Console.ResetColor();

            // Para çekme dekontu gönderme
            EmailServisi.SendParaCekmeDekontu(email, miktar, Bakiye);
        }
        else
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Geçersiz miktar veya yetersiz bakiye.");
            Console.ResetColor();
        }
    }

    public void BakiyeGoruntule()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Güncel bakiyeniz: {Bakiye} TL");
        Console.ResetColor();
    }
}

// E-posta hizmeti
public class EmailServisi
{
    public static void SendPasswordResetEmail(string email, string yeniSifre)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("bankaciliksistem2@gmail.com", "ldzh sowl wcmr lgac\r\n"), // E-posta bilgilerinizi buraya yazın
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("bankaciliksistem2@gmail.com"),
                Subject = "Şifre Sıfırlama",
                Body = $"Şifreniz sıfırlandı. Yeni şifreniz: {yeniSifre}",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Şifre sıfırlama e-postası gönderildi.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"E-posta gönderilemedi: {ex.Message}");
            Console.ResetColor();
        }
    }

    // Para çekme dekontu gönderme metodu
    public static void SendParaCekmeDekontu(string email, double miktar, double bakiye)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("bankaciliksistem2@gmail.com", "ldzh sowl wcmr lgac\r\n"), // E-posta bilgilerinizi buraya yazın
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("bankaciliksistem2@gmail.com"),
                Subject = "Para Çekme Dekontu",
                Body = $"Merhaba,\n\n{miktar} TL tutarında bir para çekme işlemi gerçekleştirdiniz. Güncel bakiyeniz: {bakiye} TL'dir.\n\nİyi günler dileriz.",
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Para çekme dekontu e-posta ile gönderildi.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"E-posta gönderilemedi: {ex.Message}");
            Console.ResetColor();
        }
    }
    public static void SendParaYatirDekontu(string email, double miktar, double bakiye)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("bankaciliksistem2@gmail.com", "ldzh sowl wcmr lgac\r\n"), 
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("bankaciliksistem2@gmail.com"),
                Subject = "Para Yatırma Dekontu",
                Body = $"Merhaba,\n\n{miktar} TL tutarında bir para yatırma işlemi gerçekleştirdiniz. Güncel bakiyeniz: {bakiye} TL'dir.\n\nİyi günler dileriz.",
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Para yatırma dekontu e-posta ile gönderildi.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"E-posta gönderilemedi: {ex.Message}");
            Console.ResetColor();
        }
    }

}

// Döviz kuru bilgisi alma
public class DövizKuru
{
    private static readonly string ApiKey = "ebab55e4bbf09a492bd6084e"; // API anahtarınızı buraya yazın
    private static readonly string ApiUrl = "https://v6.exchangerate-api.com/v6/{0}/latest/{1}"; // URL'deki {0} API_KEY, {1} BaseCurrency olacak

    public static async Task DövizKuruSorgula(string baseCurrency = "USD")
    {
        string url = string.Format(ApiUrl, ApiKey, baseCurrency);
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();  // Hata durumunda bir exception fırlatır

                // Yanıtı alıp JSON'a dönüştürme
                string responseBody = await response.Content.ReadAsStringAsync();

                // API Yanıtını konsola yazdır
                Console.WriteLine("API Yanıtı: ");
                Console.WriteLine(responseBody);

                // JSON'u deserialize et
                var dövizKuruYanıt = JsonConvert.DeserializeObject<DövizKuruYanıt>(responseBody);

                // JSON verisini kontrol et
                if (dövizKuruYanıt == null)
                {
                    Console.Beep();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Döviz kuru yanıtı boş.");
                    Console.ResetColor();
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.Beep();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hata: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
public class DövizKuruYanıt
{
    public bool Success { get; set; }
    public string BaseCurrency { get; set; }
    public Dictionary<string, double> Rates { get; set; }
}

// Ana Program
class Program
{
    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Blue;

        // "BANKA" kelimesini Figgle kütüphanesi ile büyük ASCII harflerle yazdır
        string asciiArt = Figgle.FiggleFonts.Standard.Render("BANKA");

        // Konsol ekranının ortasına yazıyı yerleştirmek için satır başına boşluk ekle
        int screenWidth = Console.WindowWidth;
        int textWidth = asciiArt.Split('\n')[0].Length; // İlk satırın uzunluğuna bakarak genişlik belirleyelim
        int spacesToAdd = (screenWidth - textWidth) / 2; // Yazıyı ortalamak için ekleyeceğimiz boşluk

        // Ortalanmış şekilde yazdır
        foreach (var line in asciiArt.Split('\n'))
        {
            Console.WriteLine(new string(' ', spacesToAdd) + line);
        }

        // Rengi varsayılan hale getir
        Console.ResetColor();
        string dosyaYolu = "kullaniciVerileri.json"; // Kullanıcı verilerinin kaydedileceği dosya yolu

        bool girisYapildi = false;
        Kullanici mevcutKullanici = null;

        while (!girisYapildi)
        {
            // Menu metni
            string menu = "\n1. Kaydol\n2. Giriş yap\n3. Şifremi unuttum";

            // Konsol ekranının genişliğini al
            int ScreenWidth = Console.WindowWidth;

            // Menü metnini satırlara ayır
            string[] menuLines = menu.Split('\n');

            // Menü metninin etrafını * ile süsle
            string borderLine = new string('*', ScreenWidth); // Ekran genişliğine göre bir sınır çiz

            // Üst sınır
            Console.WriteLine(borderLine);

            // Menü metnini her satıra hizalı olarak yazdır
            foreach (var line in menuLines)
            {
                int SpacesToAdd = (ScreenWidth - line.Length - 2) / 2; // Yazıyı ortalamak için ekleyeceğimiz boşluk
                string centeredLine = new string(' ', SpacesToAdd) + line + new string(' ', SpacesToAdd);

                // Sağdaki boşluk farkını dengele (eğer ekranın genişliği tek sayıda ise)
                if (centeredLine.Length < ScreenWidth - 1)
                {
                    centeredLine += " ";
                }

                // Ortalanmış satırı * işaretleriyle süsle
                Console.WriteLine("*" + centeredLine + "*");
            }

            // Alt sınır
            Console.WriteLine(borderLine);
            string secim = Console.ReadLine();

            try
            {
                switch (secim)
                {
                    case "1":
                        mevcutKullanici = Kaydol(dosyaYolu);
                        girisYapildi = true;
                        break;
                    case "2":
                        mevcutKullanici = GirisYap(dosyaYolu);
                        if (mevcutKullanici != null) girisYapildi = true;
                        break;
                    case "3":
                        SifreUnut(dosyaYolu);
                        break;
                    default:
                        Console.Beep();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Geçersiz seçim.");
                        Console.ResetColor();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Beep();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bir hata oluştu: " + ex.Message);
                Console.ResetColor();
            }
        }

        if (mevcutKullanici != null)
        {
            await KullaniciIslemleri(mevcutKullanici);
            girisYapildi = false;  // İşlemler bittikten sonra ana menüye dönüş
        }
    }
    /*
    static async Task DovizKuruOgren()
    {
        Console.WriteLine("Döviz kuru öğrenmek istediğiniz para birimini girin (örn: USD, EUR):");
        string fromCurrency = Console.ReadLine().ToUpper();
        Console.WriteLine("Hangi para birimine çevirmek istersiniz (örn: TRY, EUR):");
        string toCurrency = Console.ReadLine().ToUpper();

        double dovizKuru = await DovizKuru.GetDovizKuru(fromCurrency, toCurrency);

        if (dovizKuru != -1)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{fromCurrency} -> {toCurrency} kuru: {dovizKuru}");
            Console.ResetColor();
        }
    }
    */

    static Kullanici Kaydol(string dosyaYolu)
    {
        try
        {
            Console.WriteLine("Adınızı girin:");
            string ad = Console.ReadLine();
            Console.WriteLine("Soyadınızı girin:");
            string soyad = Console.ReadLine();

            string tc = "";

            // TC Kimlik numarasını alın ve geçerliliğini kontrol edin
            while (true)
            {
                Console.WriteLine("TC kimlik numaranızı girin:");

                tc = Console.ReadLine();

                // TC kimlik numarasının 11 haneli olup olmadığını kontrol et
                if (tc.Length == 11 && long.TryParse(tc, out _))
                {
                    if (TcKimlikNoDogrulama(tc))
                    {
                        Console.WriteLine("Geçerli TC Kimlik numarası.");
                        break; // Geçerli ise döngüden çık
                    }
                    else
                    {
                        Console.Beep();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Geçersiz TC Kimlik numarası! Lütfen tekrar deneyin.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.Beep();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("TC Kimlik numarası 11 haneli ve rakamlardan oluşmalıdır.");
                    Console.ResetColor();
                }
            }

            string kullaniciAdi = $"{ad.ToLower()} {soyad.ToLower()}";

            string sifre;

            while (true)
            {
                Console.WriteLine("Lütfen 6 haneli bir şifre girin (Sadece sayılar):");
                sifre = Console.ReadLine();

                // Şifrenin sadece sayılardan oluşup oluşmadığını kontrol et
                if (sifre.Length == 6 && long.TryParse(sifre, out _))
                {
                    Console.WriteLine("Şifreniz başarıyla kabul edildi.");
                    break; // Geçerli şifre girildiği için döngüden çık
                }
                else
                {
                    Console.Beep();  // Geçersiz giriş uyarısı
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Geçersiz şifre! Şifre sadece sayılardan oluşmalı ve 6 haneli olmalıdır.");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("E-posta adresinizi girin:");
            string email = Console.ReadLine(); // E-posta alınacak

            Kullanici yeniKullanici = new Kullanici(ad, soyad, tc, kullaniciAdi, sifre, email);  // E-posta adresi de kaydedilecek

            string jsonVerisi = File.Exists(dosyaYolu) ? File.ReadAllText(dosyaYolu) : "[]";
            var kullanicilar = JsonConvert.DeserializeObject<List<Kullanici>>(jsonVerisi) ?? new List<Kullanici>();
            kullanicilar.Add(yeniKullanici);

            File.WriteAllText(dosyaYolu, JsonConvert.SerializeObject(kullanicilar, Formatting.Indented));

            Console.WriteLine($"Kullanıcı kaydedildi. Hoş geldiniz {kullaniciAdi}!");
            return yeniKullanici;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kayit sırasında bir hata oluştu: {ex.Message}");
            return null;
        }
    }

    static bool TcKimlikNoDogrulama(string tc)
    {
        // TC kimlik numarasının 11 haneli ve rakamlardan oluştuğu kontrolü burada yapılıyor
        if (tc.Length != 11)
            return false;

        // Son hanenin geçerli olup olmadığını kontrol et
        int toplam = 0;
        for (int i = 0; i < 10; i++)
        {
            toplam += int.Parse(tc[i].ToString());
        }

        int ilk9Toplam = toplam % 10;

        // TC'nin algoritmasına göre, geçerli olabilmesi için bu şartları sağlamalı
        int sonBasamak = int.Parse(tc[10].ToString());

        return (ilk9Toplam == sonBasamak);
    }

    static Kullanici GirisYap(string dosyaYolu)
    {
        Console.WriteLine("Kullanıcı adı girin (Ad Soyad biçiminde):");
        string kullaniciAdi = Console.ReadLine();
        Console.WriteLine("Şifre girin:");
        string sifre = Console.ReadLine();

        string jsonVerisi = File.Exists(dosyaYolu) ? File.ReadAllText(dosyaYolu) : "[]";
        var kullanicilar = JsonConvert.DeserializeObject<List<Kullanici>>(jsonVerisi);

        foreach (var kullanici in kullanicilar)
        {
            if (kullanici.KullaniciAdi == kullaniciAdi && kullanici.Sifre == sifre)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Giriş başarılı.");
                Console.ResetColor();
                return kullanici;
            }
        }
        Console.Beep();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Kullanıcı adı veya şifre yanlış.");
        Console.ResetColor();
        return null;
    }

    static void SifreUnut(string dosyaYolu)
    {
        Console.WriteLine("E-posta adresinizi girin:");
        string email = Console.ReadLine();

        string jsonVerisi = File.Exists(dosyaYolu) ? File.ReadAllText(dosyaYolu) : "[]";
        var kullanicilar = JsonConvert.DeserializeObject<List<Kullanici>>(jsonVerisi);

        var kullanici = kullanicilar.Find(k => k.Email == email);

        if (kullanici != null)
        {
            string yeniSifre = GenerateRandomPassword();
            kullanici.Sifre = yeniSifre;

            File.WriteAllText(dosyaYolu, JsonConvert.SerializeObject(kullanicilar, Formatting.Indented));
            EmailServisi.SendPasswordResetEmail(email, yeniSifre);  // E-posta gönderildi
        }
        else
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Bu e-posta adresine ait kullanıcı bulunamadı.");
            Console.ResetColor();
        }
    }

    static string GenerateRandomPassword()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();  // 6 haneli rastgele şifre
    }

    static async Task KullaniciIslemleri(Kullanici mevcutKullanici)
    {
        var bankaIslemleri = new BankaIslemleri();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Hoş geldiniz, {mevcutKullanici.KullaniciAdi}!");

        bool cikis = false;
        while (!cikis)
        {
            Console.WriteLine("\n1. Para Yatır\n2. Para Çek\n3. Bakiye Görüntüle\n4. Döviz Kuru \n5. Çıkış");
            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    Console.WriteLine("Yatırmak istediğiniz miktarı girin:");
                    double miktarYatir = Convert.ToDouble(Console.ReadLine());
                    bankaIslemleri.ParaYatir(miktarYatir, mevcutKullanici.Email);
                    break;
                case "2":
                    Console.WriteLine("Çekmek istediğiniz miktarı girin:");
                    double miktarCek = Convert.ToDouble(Console.ReadLine());
                    bankaIslemleri.ParaCek(miktarCek, mevcutKullanici.Email); 
                    break;
                case "3":
                    bankaIslemleri.BakiyeGoruntule();
                    break;
                case "4":
                    Console.WriteLine("Döviz kuru cinsini girin (örneğin: EUR, TRY, USD): ");
                    string paraBirimi = Console.ReadLine().ToUpper();
                    await DövizKuru.DövizKuruSorgula(paraBirimi);
                    break;
                case "5":
                    cikis = true;
                    break;
                default:
                    Console.Beep();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Geçersiz seçim.");
                    Console.ResetColor();
                    break;
            }
        }
    }
}
