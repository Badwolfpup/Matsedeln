using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Matsedeln
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
            this.imagepath = imagepath;
        }

        public Goods()
        {
        }

        private string name;

        private string imagepath;

        private int gperdl;
        private int gperst;
        private int id;

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

        public int GperDL
        {
            get => gperdl;
            set
            {
                if (gperdl != value)
                {
                    gperdl = value;
                    OnPropertyChanged(nameof(GperDL));
                }

            }
        }

        public int GperST
        {
            get => gperst;
            set
            {
                if (gperst != value)
                {
                    gperst = value;
                    OnPropertyChanged(nameof(GperST));
                }
            }
        }
    }
}
