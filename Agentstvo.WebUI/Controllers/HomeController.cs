using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using Agentstvo.WebUI.Models.DomainModel;
using Agentstvo.WebUI.Models.DomainModel.Images;

namespace Agentstvo.WebUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(ObjectsRepository.ObjectForSales);
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
            var image = new Bitmap(Server.MapPath(imageFile));
            if (id != null)
            {
                image = image.AddBluredRect(30);
            }
            if (width != null && height != null)
            {
                image = image.ResizeImage(width.Value, height.Value);
            }
            image.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "image/png");
        }
    }
}