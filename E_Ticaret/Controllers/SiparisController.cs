﻿using E_Ticaret.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {

        E_TicaretEntities db = new E_TicaretEntities();

        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            return View(db.Siparis.Where(x => x.UserID == userID).ToList());
        }

        public ActionResult SiparisDetay(int id)
        {
            var siparisdetay = db.SiparisDetay.Where(x => x.SiparisID == id).ToList();
            return View(siparisdetay);
        }

        public ActionResult SiparisTamamla()
        {
            //ClientID : Bankadan alınan mağaza kodu
            //Amount bilgisi: sepettekilerin ürünlerin toplam tutar
            //Oid : Sipariş Id
            //OnayUrl : Ödeme başarılı olduğunda gelen verilerin gösterileceği url
            //Hata url : Ödeme sırasında hata olduysa gelen hatanın gösterileceği url 
            //RDN : Hash karlışaştırılması için kullanılan bilgi
            //StoreKey : Güvenlik anahtarı. Bankanın sanal pos sayfasından alınır.
            //TransactionType : Auth
            //Instalment : 
            //HashStr : HashSet oluşturulurken  bankanın istediği bilgiler birleştirilir.
            //Hash : Farklı değerler oluşturulup birleştirilir.


            string userID = User.Identity.GetUserId();

            List<Sepet> sepetUrunleri = db.Sepet.Where(x => x.UserID == userID).ToList();

            string ClientId = "1003001";//Bankanın verdiği magaza kodu
            string ToplamTutar = sepetUrunleri.Sum(x => x.ToplamTutar).ToString();

            string sipId = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);

            string onayURL = "https://localhost:44395/Siparis/Tamamlandi";

            string hataURL = "https://localhost:44395/Siparis/Hatali";

            string RDN = "asdf";
            string StoreKey = "123456";

            string TransActionType = "Auth";
            string Instalment = "";

            string HashStr = ClientId + sipId + ToplamTutar + onayURL + hataURL + TransActionType + Instalment + RDN + StoreKey;//Bankanın istediği bilgiler

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();

            byte[] HashBytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(HashStr);
            byte[] InputBytes = sha.ComputeHash(HashBytes);
            string Hash = Convert.ToBase64String(InputBytes);

            ViewBag.ClientId = ClientId;
            ViewBag.Oid = sipId;
            ViewBag.okUrl = onayURL;
            ViewBag.failUrl = hataURL;
            ViewBag.TransActionType = TransActionType;
            ViewBag.RDN = RDN;
            ViewBag.Hash = Hash;
            ViewBag.Amount = ToplamTutar;
            ViewBag.StoreType = "3d_pay_hosting"; // Ödeme modelimiz
            ViewBag.Description = "";
            ViewBag.XID = "";
            ViewBag.Lang = "tr";
            ViewBag.EMail = "erkanuluocak@gmail.com";
            ViewBag.UserID = "ErkanUluocak"; // bu id yi bankanın sanala pos ekranında biz oluşturuyoruz.
            ViewBag.PostURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate";
            return View();
        }


        public ActionResult Tamamlandi()
        {
            string userID = User.Identity.GetUserId();

            Siparis siparis = new Siparis()
            {
                Ad = Request.Form.Get("Ad"),
                Soyad = Request.Form.Get("Soyad"),
                Adres = Request.Form.Get("Adres"),
                Tarih = DateTime.Now,
                TcKimlikNo = Request.Form.Get("TcKimlikNo"),
                Telefon = Request.Form.Get("Telefon"),
                UserID = userID
            };

            List<Sepet> sepettekiurunler = db.Sepet.Where(x => x.UserID == userID).ToList();

            foreach (var item in sepettekiurunler)
            {
                SiparisDetay sd = new SiparisDetay()
                {
                    Adet = item.Adet,
                    ToplamTutar = item.ToplamTutar,
                    UrunID = item.UrunID
                };

                siparis.SiparisDetay.Add(sd);
                db.Sepet.Remove(item);
            }

            db.Siparis.Add(siparis);
            db.SaveChanges();

            return View();
        }

        public ActionResult Hatali()
        {
            ViewBag.hata = Request.Form;
            return View();
        }
    }
}