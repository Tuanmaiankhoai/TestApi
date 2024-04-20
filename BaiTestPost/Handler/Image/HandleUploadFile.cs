namespace BaiTestPost.Handler.Image
{
    public class HandleUploadFile
    {
        public static async Task<string> WriteFile(IFormFile file)
        {
            string fileName = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                fileName = DateTime.Now.Ticks + extension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Files");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var exactpath = Path.Combine(filePath, fileName);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return fileName;
        }
    }
}
