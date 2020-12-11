using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.Models;
using PicoShelter_ApiServer.Requests;
using PicoShelter_ApiServer.Shared;
using PicoShelter_ApiServer.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        ApplicationContext db;
        public UploadController(ApplicationContext context)
        {
            db = context;
        }

        [HttpPost]
        [RequestSizeLimit(10000000)]
        public async Task<IActionResult> Upload([FromForm] UploadFormModel form)
        {
            var profile = db.GetProfile(User?.Identity);

            bool? IsPublic = form.IsPublic;
            if (profile == null)
            {
                if (form.Quality > 95)
                    ModelState.AddModelError("Quality", "Quality more than " + 95 + " not allowed for unauthorized users.");

                if (form.IsPublic == false)
                    ModelState.AddModelError("IsPublic", "Private upload not allowed for unauthorized users.");

                if (form.DeleteInHours == null)
                    ModelState.AddModelError("DeleteInHours", "Lifetime same not allowed for unauthorized users.");

                IsPublic ??= true;
            }
            else
            {
                IsPublic ??= false;
            }

            if (form.JoinToAlbums?.Length > 0)
            {
                foreach (var album in form.JoinToAlbums)
                {
                    if (!db.Albums.Any(c => c.Id == album))
                    {
                        ModelState.AddModelError("JoinToAlbum", "Group #" + album + " doesn't exists.");
                    }
                }
            }            

            ImageHandler imageHandler = null;
            try
            {
                var stream = form.File.OpenReadStream();
                imageHandler = new ImageHandler(stream);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", ex.Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var image = new Image()
            {
                Title = form.Title,
                Profile = profile,
                IsPublic = (bool)IsPublic,
                UploadedTime = DateTime.UtcNow
            };

            if (form.DeleteInHours != null)
                image.DeleteIn = image.UploadedTime + TimeSpan.FromHours((double)form.DeleteInHours);
            else
                image.DeleteIn = null;

            db.Images.Add(image);
            await db.SaveChangesAsync();
            image.ImageCode = NumberToCodeConventer.Convert(image.Id);

            if (form.Quality == 100)
            {
                image.Extension = imageHandler.Extension;
                imageHandler.Save(image.ImageFilePath);
            }
            else
            {
                image.Extension = "jpg";
                imageHandler.Save(image.ImageFilePath, form.Quality);
            }

            imageHandler.GetThumbnail().SaveWithDirectory(image.ImageThumbnail_128x, ImageFormat.Jpeg);

            db.Update(image);

            if (form.JoinToAlbums?.Length > 0)
            {
                var albumsImages = form.JoinToAlbums?.Select(t => new AlbumImage() { AlbumId = t, Image = image });
                db.AlbumImages.AddRange(albumsImages);
            }
            
            await db.SaveChangesAsync();

            return new JsonResult(image);
        }
    }
}
