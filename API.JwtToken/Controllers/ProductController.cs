using API.JwtToken.Helper;
using API.JwtToken.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace API.JwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly dbContext dbContext;

        public ProductController(IWebHostEnvironment environment, dbContext dbContext)
        {
            this.environment = environment;
            this.dbContext = dbContext;
        }

        //upload single img & store in server path
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productCode)
        {
            APIResponse response = new APIResponse();

            try
            {
                string FilePath = GetFilePath(productCode);
                if (!System.IO.Directory.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }
                string imagepath = FilePath + "\\" + productCode + ".png";

                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
            }
            catch (Exception ex)
            {

                response.ErrorMessage = ex.Message;
            }

            return Ok(response);
        }

        

        //upload multiple imgs & store in server path
        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0;
            int errorCount = 0;
            try
            {
                string FilePath = GetFilePath(productCode);
                if (!System.IO.Directory.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }

                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }

                foreach (var file in fileCollection)
                {
                    string imagepath = FilePath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passCount++;
                       
                    }
                }        
               
            }
            catch (Exception ex)
            {
                errorCount++;
                response.ErrorMessage = ex.Message;
            }

            response.ResponseCode = 200;
            response.Result = passCount + " Files uploaded & " +errorCount + " Files failed";

            return Ok(response);
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult>GetImage(string productCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    imageUrl = hostUrl + "Upload/product/" + productCode + "/" + productCode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return Ok(imageUrl);
        }

        //Get Multiple Images
        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string productCode)
        {
            List< string> imageUrlList = new List<string>();

            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);
                if (System.IO.Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos= directoryInfo.GetFiles(); //get Images Array
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string fileName = fileInfo.Name;
                        string imagePath = FilePath + "\\" + fileName;
                        if (System.IO.File.Exists(imagePath))
                        {
                            string _imageUrl = hostUrl + "Upload/product/" + productCode + "/" + fileName;
                            imageUrlList.Add(_imageUrl);
                        }

                    }
                }
                
                //else
                //{
                //    return NotFound();
                //}
            }
            catch (Exception ex)
            {

                throw;
            }

            return Ok(imageUrlList);
        }

        //dowload File
        [HttpGet("Download")]
        public async Task<IActionResult> Download(string productCode)
        {
            //string imageUrl = string.Empty;
            //string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productCode + ".png");
                    //imageUrl = hostUrl + "Upload/product/" + productCode + "/" + productCode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return NotFound();
            }

            //return Ok(imageUrl);
        }

        
        [HttpGet("Remove")]
        public async Task<IActionResult> Remove(string productCode)
        {
            //string imageUrl = string.Empty;
            //string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);
                string imagepath = FilePath + "\\" + productCode + ".png";
                if (System.IO.File.Exists(imagepath))
                {

                    System.IO.File.Delete(imagepath);
                    return Ok("pass!");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return NotFound();
            }

            //return Ok(imageUrl);
        }

        [HttpGet("MultiRemove")]
        public async Task<IActionResult> MultiRemove(string productCode)
        {

            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            try
            {
                string FilePath = GetFilePath(productCode);
                if (System.IO.Directory.Exists(FilePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles(); //get Images Array
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                    return Ok("pass!");
                }
                else
                {
                    return NotFound();
                }


            }
            catch (Exception ex)
            {

                return NotFound();
            }

            
        }

        [HttpPut("DBUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection fileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0;
            int errorCount = 0;
            try
            {
                

                foreach (var file in fileCollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        this.dbContext.TblProductImages.Add(new Repos.Models.TblProductImage()
                        {
                            ProductCode = productCode,
                            ProductImg = stream.ToArray()
                        });
                        await this.dbContext.SaveChangesAsync();
                        passCount++;
                    }
                }

            }
            catch (Exception ex)
            {
                errorCount++;
                response.ErrorMessage = ex.Message;
            }

            response.ResponseCode = 200;
            response.Result = passCount + " Files uploaded & " + errorCount + " Files failed";

            return Ok(response);
        }

        [HttpGet("DBGetMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string productCode)
        {
            List<string> imageUrlList = new List<string>();

            
            try
            {
                var _prodImages = dbContext.TblProductImages.Where(item => item.ProductCode == productCode);
                if (_prodImages!=null && _prodImages.Count()>0)
                {
                    foreach (var prodImage in _prodImages)
                    {
                        imageUrlList.Add(Convert.ToBase64String(prodImage.ProductImg));
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return Ok(imageUrlList);
        }
        
        [NonAction]
        private string GetFilePath(string productCode)
        {
            return this.environment.WebRootPath + "\\Upload\\product\\" + productCode;
        }


    }
}
