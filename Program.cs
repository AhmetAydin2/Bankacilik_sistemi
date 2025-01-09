using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;
using Figgle;

// Ana sınıf (BaseClass) tanımlanıyor
public abstract class BaseClass
{
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string TC { get; set; }
    public string KullaniciAdi { get; set; }
    public string Sifre { get; set; }

    public BaseClass(string ad, string soyad, string tc, string kullaniciAdi, string sifre)
    {
        Ad = ad;
        Soyad = soyad;
        TC = tc;
        KullaniciAdi = kullaniciAdi;
        Sifre = sifre;
    }

    public abstract void Yazdir();
}

// Kullanıcı sınıfı, BaseClass'tan miras alır
public class Kullanici : BaseClass
{
    public Kullanici(string ad, string soyad, string tc, string kullaniciAdi, string sifre)
        : base(ad, soyad, tc, kullaniciAdi, sifre) { }

    public override void Yazdir()
    {
        Console.WriteLine($"Kullanıcı Adı: {KullaniciAdi}, Ad: {Ad}, Soyad: {Soyad}, TC: {TC}");
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

    public void ParaYatir(double miktar)
    {
        if (miktar > 0)
        {
            Bakiye += miktar;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{miktar} TL yatırıldı. Güncel bakiye: {Bakiye} TL");
            Console.ResetColor();
        }
        else
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Geçersiz miktar.");
            Console.ResetColor();
        }
    }

    public void ParaCek(double miktar)
    {
        if (miktar > 0 && miktar <= Bakiye)
        {
            Bakiye -= miktar;
            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine($"{miktar} TL çekildi. Güncel bakiye: {Bakiye} TL");
            Console.ResetColor();   
        }
        else
        {
            Console.Beep();
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("Geçersiz miktar veya yetersiz bakiye.");
            Console.ResetColor();   
        }
    }

    public void BakiyeGoruntule()
    {
        Console.ForegroundColor=ConsoleColor.Blue;
        Console.WriteLine($"Güncel bakiyeniz: {Bakiye} TL");
        Console.ResetColor();
    }
}


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

// API'den gelen döviz kuru yanıtını temsil eden model
public class DövizKuruYanıt
{
    public bool Success { get; set; }
    public string BaseCurrency { get; set; }
    public Dictionary<string, double> Rates { get; set; }
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
            Console.ForegroundColor= ConsoleColor.Green;
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

            Kullanici yeniKullanici = new Kullanici(ad, soyad, tc, kullaniciAdi, sifre);

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
        Console.ForegroundColor= ConsoleColor.Red;    
        Console.WriteLine("Kullanıcı adı veya şifre yanlış.");
        Console.ResetColor();
        return null;
        Console.ReadKey();
    }

    static void SifreUnut(string dosyaYolu)
    {
        Console.WriteLine("Kullanıcı adı girin (Ad Soyad biçiminde):");
        string kullaniciAdi = Console.ReadLine();
        Console.WriteLine("E-posta adresinizi girin:");
        string email = Console.ReadLine();

        // Burada e-posta doğrulaması yapılabilir
        Random rastgele = new Random();
        string yeniSifre = $"{rastgele.Next(100000, 999999)}";  // 6 haneli yeni şifre

        // Kullanıcının JSON dosyasındaki verilerini oku
        string jsonVerisi = File.Exists(dosyaYolu) ? File.ReadAllText(dosyaYolu) : "[]";
        var kullanicilar = JsonConvert.DeserializeObject<List<Kullanici>>(jsonVerisi);

        // Kullanıcıyı bul ve şifresini güncelle
        var mevcutKullanici = kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);
        if (mevcutKullanici != null)
        {
            mevcutKullanici.Sifre = yeniSifre;  // Yeni şifreyi ata
            File.WriteAllText(dosyaYolu, JsonConvert.SerializeObject(kullanicilar, Formatting.Indented));  // Güncellenmiş veriyi dosyaya yaz

            // E-posta gönderme
            EmailServisi.SendPasswordResetEmail(email, yeniSifre);
        }
        else
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Kullanıcı adı bulunamadı.");
            Console.ResetColor();
        }
    }

    static async Task KullaniciIslemleri(Kullanici kullanici)
    {
        BankaIslemleri banka = new BankaIslemleri();

        while (true)
        {
            //Console.WriteLine("\n1. Para yatır\n2. Para çek\n3. Bakiye görüntüle\n4. Döviz kuru sorgula\n5. Çıkış");
            string menu = "\n1. Para yatır\n2. Para çek\n3. Bakiye görüntüle\n4. Döviz kuru sorgula\n5. Çıkış";

            // Konsol ekranının genişliğini al
            int screenWidth = Console.WindowWidth;

            // Menü metnini satırlara ayır
            string[] menuLines = menu.Split('\n');

            // Menü metninin etrafını * ile süsle
            string borderLine = new string('*', screenWidth); // Ekran genişliğine göre bir sınır çiz

            // Üst sınır
            Console.WriteLine(borderLine);

            // Menü metnini her satıra hizalı olarak yazdır
            foreach (var line in menuLines)
            {
                int spacesToAdd = (screenWidth - line.Length - 2) / 2; // Yazıyı ortalamak için ekleyeceğimiz boşluk
                string centeredLine = new string(' ', spacesToAdd) + line + new string(' ', spacesToAdd);

                // Sağdaki boşluk farkını dengele (eğer ekranın genişliği tek sayıda ise)
                if (centeredLine.Length < screenWidth - 1)
                {
                    centeredLine += " ";
                }

                // Ortalanmış satırı * işaretleriyle süsle
                Console.WriteLine("*" + centeredLine + "*");
            }

            // Alt sınır
            Console.WriteLine(borderLine);
            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    Console.WriteLine("Yatırmak istediğiniz tutarı girin:");
                    double yatirTutar = Convert.ToDouble(Console.ReadLine());
                    banka.ParaYatir(yatirTutar);
                    break;
                case "2":
                    Console.WriteLine("Çekmek istediğiniz tutarı girin:");
                    double cekTutar = Convert.ToDouble(Console.ReadLine());
                    banka.ParaCek(cekTutar);
                    break;
                case "3":
                    banka.BakiyeGoruntule();
                    break;
                case "4":
                    Console.WriteLine("Döviz kuru cinsini girin (örneğin: EUR, TRY, USD): ");
                    string paraBirimi = Console.ReadLine().ToUpper();
                    await DövizKuru.DövizKuruSorgula(paraBirimi);
                    break;
                case "5":
                    Console.WriteLine("Çıkılıyor...");
                    return;
                default:
                    Console.WriteLine("Geçersiz seçim.");
                    break;
            }
        }
    }
}

