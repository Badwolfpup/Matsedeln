using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MatsedelnShared.Models
{
    public partial class Goods : ObservableValidator
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
        [ObservableProperty]
        private string name = string.Empty;
        [ObservableProperty]
        private string? imagePath = "pack://application:,,,/Images/dummybild.png";
        [ObservableProperty]
        private int gramsPerDeciliter = 0;
        [ObservableProperty]
        private int gramsPerStick = 0;
    }
}
