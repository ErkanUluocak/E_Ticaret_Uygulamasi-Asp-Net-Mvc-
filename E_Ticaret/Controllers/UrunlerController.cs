﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Ticaret.Models;

namespace E_Ticaret.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UrunlerController : Controller
    {
        private E_TicaretEntities db = new E_TicaretEntities();

        // GET: Urunler
        public ActionResult Index()
        {
            var urunler = db.Urunler.Include(u => u.Kategori);
            return View(urunler.ToList());
        }

        // GET: Urunler/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunler.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // GET: Urunler/Create
        public ActionResult Create()
        {
            ViewBag.KategoriID = new SelectList(db.Kategori, "KategoriID", "KategoriAdi");
            return View();
        }

        // POST: Urunler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UrunID,UrunAdi,KategoriID,UrunAciklamasi,UrunFiyatı")] Urunler urunler, HttpPostedFileBase UrunResim)
        {
            if (ModelState.IsValid)
            {
                db.Urunler.Add(urunler);
                db.SaveChanges();
                if (UrunResim != null && UrunResim.ContentLength > 0)
                {
                    string dosya = Path.Combine(Server.MapPath("~/Resim"), urunler.UrunID + ".jpg");
                    UrunResim.SaveAs(dosya);
                }
                return RedirectToAction("Index");
            }

            ViewBag.KategoriID = new SelectList(db.Kategori, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }

        // GET: Urunler/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunler.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriID = new SelectList(db.Kategori, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }

        // POST: Urunler/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UrunID,UrunAdi,KategoriID,UrunAciklamasi,UrunFiyatı")] Urunler urunler, HttpPostedFileBase UrunResim)
        {
            if (ModelState.IsValid)
            {
                db.Entry(urunler).State = EntityState.Modified;
                db.SaveChanges();
                if (UrunResim != null && UrunResim.ContentLength > 0)
                {
                    string dosya = Path.Combine(Server.MapPath("~/Resim"), urunler.UrunID + ".jpg");
                    UrunResim.SaveAs(dosya);
                }
                return RedirectToAction("Index");
            }
            ViewBag.KategoriID = new SelectList(db.Kategori, "KategoriID", "KategoriAdi", urunler.KategoriID);
            return View(urunler);
        }

        // GET: Urunler/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunler.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // POST: Urunler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Urunler urunler = db.Urunler.Find(id);
            db.Urunler.Remove(urunler);
            db.SaveChanges();

            string dosya = Path.Combine(Server.MapPath("~/Resim"), urunler.UrunID + ".jpg");

            FileInfo fi = new FileInfo(dosya);
            if (fi.Exists)
            {
                fi.Delete();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
