using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranNgocTan_Tuan4.Models;

namespace TranNgocTan_Tuan4.Controllers
{
    public class GioHangController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if(lstGiohang == null)
            {
                lstGiohang = new List<GioHang>();
                Session["GioHang"] = lstGiohang;

            }
            return lstGiohang;
        }
        // GET: GioHang
        public ActionResult ThemGioHang(int id, string strURL)
            
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.Find(n => n.masach == id);
            if(sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
            
        }
        private int TongSoLuong()
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if(lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.iSoluong);
            }
            return tsl;
        }
        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }
        private double TongTien()
        {
            double tt = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.dThanhtien);
            }
            return tt;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }

        public ActionResult GioHangPartial()
        {
            
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }

        public ActionResult XoaGioHang(int id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.masach == id);
            if(sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.masach == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhatGioHang(int id, FormCollection collection)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.masach == id);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(collection["txtSoLg"].ToString());
                
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCaGioHang(int id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Fashion");
            }
            List<GioHang> gioHangs = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(gioHangs);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dONDATHANG = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            Sach s = new Sach();
            List<GioHang> gioHangs = Laygiohang();
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);

            dONDATHANG.makh = kh.makh;
            dONDATHANG.ngaydat = DateTime.Now;
            dONDATHANG.ngaygiao = DateTime.Parse(ngaygiao);
            dONDATHANG.giaohang = false;
            dONDATHANG.thanhtoan = false;

            data.DonHangs.InsertOnSubmit(dONDATHANG);
            data.SubmitChanges();
            foreach (var item in gioHangs)
            {
                ChiTietDonHang CT = new ChiTietDonHang();

                CT.madon = dONDATHANG.madon;
                CT.masach = item.masach;
                CT.soluong = item.iSoluong;
                CT.gia = (decimal)item.giaban;
                s = data.Saches.Single(n => n.masach == item.masach);
                s.soluongton -= CT.soluong;
                data.SubmitChanges();
                data.ChiTietDonHangs.InsertOnSubmit(CT);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("XacNhanDonHang", "Giohang");

        }

        public ActionResult XacnhanDonhang()
        {
            return View();
        }




    }
}