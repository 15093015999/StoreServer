using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace ProfileServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        [HttpPost("upload")]
        public string Upload([FromForm] IFormCollection formCollection)
        {
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            string filename = "";
            foreach (IFormFile file in fileCollection)
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                string content = reader.ReadToEnd();

                //分割名字和后缀
                string[] array = file.FileName.Split('.');
                string suffix = array[array.Length - 1];
                string realname = "";
                for (int i = 0; i < array.Length - 1; i++)
                {
                    realname += array[i];
                }
                string name = Guid.NewGuid() + DateTime.UtcNow.ToString() + realname;


                //md5数字摘要取哈希
                string hashName = "";
                using (var md5 = MD5.Create())
                {
                    var result = md5.ComputeHash(Encoding.ASCII.GetBytes(name));
                    var strResult = BitConverter.ToString(result);
                    hashName = strResult.Replace("-", "");
                }


                //哈希名加后缀
                filename = hashName + "." + suffix;
                string path = @"./StaticFiles/Avatars/" + filename;
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                using (FileStream fs = System.IO.File.Create(path))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return filename;
        }
    }
}