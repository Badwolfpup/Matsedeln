using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Windows.Media.Animation;

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

        public Recipe(Recipe recipe)
        {
            this.name = recipe.Name;
            this.Id = recipe.Id;
            this.imagePath = recipe.ImagePath;
            this.ingredientlist = new ObservableCollection<Ingredient>(recipe.Ingredientlist);
            this.ParentRecipes = recipe.ParentRecipes;
            this.ChildRecipes = recipe.ChildRecipes;
        }

        private string name;
        private string imagePath = "pack://application:,,,/Images/dummybild.png";

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

        [Key]
        public int Id { get; set; }



        public ICollection<RecipeHierarchy> ChildRecipes { get; set; } = new ObservableCollection<RecipeHierarchy>();

        public ICollection<RecipeHierarchy> ParentRecipes { get; set; } = new ObservableCollection<RecipeHierarchy>();


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
