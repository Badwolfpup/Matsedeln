using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Matsedeln.Models
{
    public class Recipe : INotifyPropertyChanged
    {
        #region
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public Recipe(string name)
        {
            this.name = name;
            ingredientlist = new ObservableCollection<Ingredient>();
        }

        public Recipe()
        {
            ingredientlist = new ObservableCollection<Ingredient>();
        }

        private string name;
        private string imagePath = "pack://application:,,,/Images/dummybild.png";
        private int id;

        private ICollection<Ingredient> ingredientlist;
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

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public int? ParentRecipeId { get; set; }  // FK for self-ref
        public Recipe? ParentRecipe { get; set; }  // Nav prop
        public ICollection<Recipe> ChildRecipes { get; set; } = new List<Recipe>();  // Reverse nav

        [NotMapped]
        public ICollection<Ingredient> Ingredientlist
        {
            get { return ingredientlist; }
            set
            {
                if (ingredientlist != value)
                {
                    ingredientlist = value;
                    OnPropertyChanged(nameof(Ingredientlist));
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


    }
}
