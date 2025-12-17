using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Authelp.Pages
{
    public class HelpModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public HelpModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public List<string> UploadedFiles { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public void OnGet()
        {
            LoadFiles();
        }

        private void LoadFiles()
        {
            string folder = Path.Combine(_env.WebRootPath, "helpdocs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            UploadedFiles = Directory
                .GetFiles(folder)
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .Cast<string>()   
                .ToList();
        }

        public IActionResult OnPostUpload()
        {
            if (UploadFile == null || UploadFile.Length == 0)
            {
                TempData["Error"] = "Please choose a file before uploading.";
                LoadFiles();
                return Page();
            }

            string folder = Path.Combine(_env.WebRootPath, "helpdocs");
            Directory.CreateDirectory(folder);

            string savePath = Path.Combine(folder, UploadFile.FileName);

            using (var fs = new FileStream(savePath, FileMode.Create))
            {
                UploadFile.CopyTo(fs);
            }

            TempData["Success"] = "File uploaded successfully!";
            return RedirectToPage();
        }

        public IActionResult OnGetDownload(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return BadRequest("Invalid file.");

            string folder = Path.Combine(_env.WebRootPath, "helpdocs");
            string fullPath = Path.Combine(folder, filename);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);

            
            Response.Headers.Append("Content-Disposition", $"attachment; filename={filename}");

            return File(fileBytes, "application/octet-stream");
        }
    }
}
