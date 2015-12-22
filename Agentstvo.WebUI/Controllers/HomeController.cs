using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Agentstvo.WebUI.Models.DomainModel;
using Agentstvo.WebUI.Models.DomainModel.Images;

namespace Agentstvo.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private int _pageSize = 3;

        public HomeController()
        {
            ObjectsRepository.CheckVersion();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Image(string imageFile, int? width, int? height, int? id)
        {
            var memoryStream = new MemoryStream();
            var image = new Bitmap(ObjectsRepository.CatalogFolder + imageFile.Replace("___", "/"));
            if (width != null && height != null)
            {
                image = image.Clip10X15();
                image = image.ResizeImage(width.Value, height.Value);
            }
            if (id != null)
            {
                image = image.AddBluredRect(30);
                var htmlText = ObjectsRepository.GetObjectDescription(id.Value);
                if (!string.IsNullOrEmpty(htmlText))
                {
                    image = image.AddText(30, 0, htmlText);
                }
            }
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "image/jpeg");
        }

        [HttpPost]
        public ActionResult ObjectsList(int? page)
        {
            page = page ?? 1;
            _pageSize = 3;
            return Json(new {
                data = ObjectsRepository.ObjectForSales
                    .Skip((page.Value - 1) * _pageSize)
                    .Take(_pageSize),
                total = ObjectsRepository.ObjectForSales. Count
            });
        }
    }
}