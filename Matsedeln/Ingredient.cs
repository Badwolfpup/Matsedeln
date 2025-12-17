using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Media.Animation;

namespace Matsedeln
{
    public class Ingredient : INotifyPropertyChanged
    {
        #region
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string unit;

        private int quantity;

        public string Unit
        {
            get => unit;
            set
            {
                if (unit != value)
                {
                    unit = value;
                    
                    OnPropertyChanged(nameof(Unit));
                }
            }
        }



        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        private int quantityingram;
        private int quantityindl;
        private int quantityinst;
        private int quantityinmsk;
        private int quantityintsk;
        public int QuantityInGram
        {
            get => quantityingram;
            set
            {
                if (quantityingram != value)
                {
                    quantityingram = value;
                    OnPropertyChanged(nameof(QuantityInGram));
                }
            }
        }

        public int QuantityInDl
        {
            get => quantityindl;
            set
            {
                if (quantityindl != value)
                {
                    quantityindl = value;
                    OnPropertyChanged(nameof(QuantityInDl));
                }
            }
        }


        public int QuantityInSt
        {
            get => quantityinst;
            set
            {
                if (quantityinst != value)
                {
                    quantityinst = value;
                    OnPropertyChanged(nameof(QuantityInSt));
                }
            }
        }

        public int QuantityInMsk
        {
            get => quantityinmsk;
            set
            {
                if (quantityinmsk != value)
                {
                    quantityinmsk = value;
                    OnPropertyChanged(nameof(QuantityInMsk));
                }
            }
        }

        public int QuantityInTsk
        {
            get => quantityintsk;
            set
            {
                if (quantityintsk != value)
                {
                    quantityintsk = value;
                    OnPropertyChanged(nameof(QuantityInTsk));
                }
            }
        }


        public  ObservableCollection<string> UnitOptions { get; set; }

        public Goods Good { get; set; }

        public Ingredient(int quantity, string unit, Goods good)
        {
            UnitOptions = new ObservableCollection<string>();
            this.Unit = unit;
            this.Good = good;
            this.Quantity = quantity;
            GetQuantityInGram(Quantity);
            ConvertToOtherUnits(QuantityInGram);
        }



        public Ingredient(Ingredient copy)
        {
            UnitOptions = new ObservableCollection<string>();
            this.Good = copy.Good;
            this.Unit = copy.Unit;
            GetQuantityInGram(copy.Quantity);
            ConvertToOtherUnits(this.QuantityInGram);
            AddUnitOptions();
            this.Quantity = copy.QuantityInGram;
            this.Unit = "g";

        }
        private void AddUnitOptions()
        {
            UnitOptions.Clear();
            UnitOptions.Add("g");
            if (Good.GperDL != 0) UnitOptions.Add("dl");
            if (Good.GperST != 0) UnitOptions.Add("st");
            if (Unit == "msk") UnitOptions.Add("msk");
            if (Unit == "tsk") UnitOptions.Add("tsk");
        }

        private void ConvertToOtherUnits(int q)
        {
            if (string.IsNullOrEmpty(Unit)) return;

            if (Unit == "g")
            {
                if (Good.GperST != 0) QuantityInSt = QuantityInGram / Good.GperST + (QuantityInSt % Good.GperST > 0 ? 1 : 0);
                if (Good.GperDL != 0) QuantityInDl = QuantityInGram / Good.GperDL + QuantityInGram % Good.GperDL > 0 ? 1 : 0;
            }
            else if (Unit == "dl")
            {
                if (Good.GperDL != 0) QuantityInDl = QuantityInGram / Good.GperDL + (QuantityInDl % Good.GperDL > 0 ? 1 : 0);
            }
            else if (Unit == "st")
            {
                if (Good.GperST != 0) QuantityInSt = QuantityInGram / Good.GperST + (QuantityInSt % Good.GperST > 0 ? 1 : 0);
            }
            else if (Unit == "kg") {
                if (Good.GperST != 0) QuantityInSt = QuantityInGram / Good.GperST + (QuantityInSt % Good.GperST > 0 ? 1 : 0);
                if (Good.GperDL != 0) QuantityInDl = QuantityInGram / Good.GperDL + (QuantityInDl % Good.GperDL > 0 ? 1 : 0);
            }
            else if (Unit == "msk")
            {
                QuantityInMsk = QuantityInGram / (Good.GperDL / 20 * 3);
            }
            else if (Unit == "tsk")
            {
                QuantityInTsk = QuantityInGram / (Good.GperDL / 20);

            }
        }

        private void GetQuantityInGram(int q)
        {
            if (Unit == "g") QuantityInGram = q;
            else if (Unit == "dl") QuantityInGram = Good.GperDL * q;
            else if (Unit == "st") QuantityInGram = Good.GperST * q;
            else if (Unit == "kg") QuantityInGram = q * 1000;
            else if (Unit == "msk") QuantityInGram = (Good.GperDL / 20 * 3) * q;
            else if (Unit == "tsk") QuantityInGram = (Good.GperDL / 20) * q;
        }

        public Ingredient()
        {
            UnitOptions = new ObservableCollection<string>();
        }

        public override string ToString()
        {
            return $"{Quantity} {Unit} {Good.Name}";
        }

    }
}
