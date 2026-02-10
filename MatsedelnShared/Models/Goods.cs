using System.ComponentModel.DataAnnotations;

namespace MatsedelnShared.Models
{
    // Plain POCO - UI change notification is handled by GoodsWrapper in the WPF client.
    // This keeps the shared library free of UI framework dependencies.
    public class Goods
    {
        public Goods(string name, string imagepath)
        {
            this.Name = name;
            this.ImagePath = imagepath;
        }

        public Goods()
        {
        }

        [Key]
        public int Id { get; set; } = 0;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? ImagePath { get; set; } = "pack://application:,,,/Images/dummybild.png";

        public int GramsPerDeciliter { get; set; } = 0;

        public int GramsPerStick { get; set; } = 0;
    }
}
