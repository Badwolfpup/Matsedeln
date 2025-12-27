using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Matsedeln.Models
{
    public class Goods : INotifyPropertyChanged
    {
        #region
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Goods(string name, string imagepath)
        {
            this.name = name;
            this.ImagePath = imagepath;
        }

        public Goods()
        {
        }

        private string name;

        private string imagePath = "pack://application:,,,/Images/dummybild.png";

        private int gramsperdeciliter;
        private int gramsperstick;

        [Required]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }


        public string? ImagePath
        {
            get { return imagePath; }
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }
        [Key]
        public int Id { get; set; }


        public int GramsPerDeciliter
        {
            get => gramsperdeciliter;
            set
            {
                if (gramsperdeciliter != value)
                {
                    gramsperdeciliter = value;
                    OnPropertyChanged(nameof(GramsPerDeciliter));
                }

            }
        }

        public int GramsPerStick
        {
            get => gramsperstick;
            set
            {
                if (gramsperstick != value)
                {
                    gramsperstick = value;
                    OnPropertyChanged(nameof(GramsPerStick));
                }
            }
        }
    }
}
