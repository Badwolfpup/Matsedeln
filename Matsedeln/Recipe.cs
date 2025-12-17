using Matsedeln;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Matsedeln
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

        }

        private string name;
        private string imagepath;
        private int id;

        private ObservableCollection<Ingredient> ingredientlist;
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

        public ObservableCollection<Ingredient> Ingredientlist
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

        public string Imagepath
        {
            get { return imagepath; }
            set
            {
                if (imagepath != value)
                {
                    imagepath = value;
                    OnPropertyChanged(nameof(Imagepath));
                }
            }
        }


    }
}
