using System.ComponentModel.DataAnnotations;

namespace DocumentReader.MVC.Models
{
    public class DocumentInfo
    {
        [Required]
        [Display(Name = "Folder Path")]
        public string FolderPath { get; set; } = String.Empty;
        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = String.Empty;

        public DocumentInfo(string folderPath, string fileName)
        {
            FolderPath = folderPath;
            FileName = fileName;
        }
    }
}