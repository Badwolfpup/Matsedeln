using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Media.Animation;

namespace Matsedeln.Models
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

        private double quantity;

        [Required]
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


        [Required]
        public double Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    //GetQuantityInGram(quantity);
                    //ConvertToOtherUnits(QuantityInGram);
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public string ChosenUnit
        {
            get => chosenunit;
            set
            {
                if (chosenunit != value)
                {
                    chosenunit = value;
                    OnPropertyChanged(nameof(ChosenUnit));
                }
            }
        }

        private int quantityingram;
        private int quantityindl;
        private int quantityinst;
        private int quantityinmsk;
        private int quantityintsk;
        private int quantityinkrm;
        private string chosenunit = "g";
        

        [NotMapped]
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
        [NotMapped]
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

        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
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
        [NotMapped]
        public int QuantityInKrm
        {
            get => quantityinkrm;
            set
            {
                if (quantityinkrm != value)
                {
                    quantityinkrm = value;
                    OnPropertyChanged(nameof(QuantityInKrm));
                }
            }
        }
        

        public int Id { get; set; }

        [Required]
        public int GoodsId { get; set; }  // FK to Goods
        
        private Goods _good;

        [Required]
        public Goods Good
        {
            get => _good;
            set
            {
                _good = value;
                OnPropertyChanged(nameof(Good));
            }
        }

        [Required]
        public int RecipeId { get; set; }  // FK to Recipe
        [Required]
        public Recipe Recipe { get; set; }  // Nav prop


        [NotMapped]
        public  ObservableCollection<string> UnitOptions { get; set; }


        public Ingredient()
        {
            UnitOptions = new ObservableCollection<string>();
        }


        public Ingredient(Ingredient copy)
        {
            UnitOptions = new ObservableCollection<string>();
            this.Good = copy.Good;
            this.Unit = copy.Unit;
            this.Id = copy.Id;
            GetQuantityInGram(copy.Quantity);
            ConvertToOtherUnits();
            AddUnitOptions();
            this.Quantity = copy.Quantity;
            this.ChosenUnit = copy.ChosenUnit;

        }
        public void AddUnitOptions()
        {
            UnitOptions.Clear();
            UnitOptions.Add("g");
            if (Good.GramsPerDeciliter != 0)
            {
                UnitOptions.Add("dl");
                UnitOptions.Add("l");
                UnitOptions.Add("msk");
                UnitOptions.Add("tsk");
                UnitOptions.Add("krm");
            }
            if (Good.GramsPerStick != 0) UnitOptions.Add("st");

            //Remove of adds unintended value
            if (!UnitOptions.Contains(Unit)) Unit = "g";
        }

        public void ConvertToOtherUnits()
        {
            if (string.IsNullOrEmpty(Unit)) return;
            
            if (Good.GramsPerDeciliter != 0)
            {
                QuantityInDl = QuantityInGram / Good.GramsPerDeciliter + QuantityInGram % Good.GramsPerDeciliter > 0 ? 1 : 0;
                QuantityInMsk = QuantityInGram / (int)Math.Ceiling(((double)Good.GramsPerDeciliter / 20 * 3)) + (QuantityInGram % Good.GramsPerDeciliter > 0 ? 1 : 0);
                QuantityInTsk = QuantityInGram / (int)Math.Ceiling(((double)Good.GramsPerDeciliter / 20)) + (QuantityInGram % Good.GramsPerDeciliter > 0 ? 1 : 0);
                QuantityInKrm = QuantityInGram / (int)Math.Ceiling(((double)Good.GramsPerDeciliter / 100)) + (QuantityInGram % Good.GramsPerDeciliter > 0 ? 1 : 0);
            }
            if ((Good.GramsPerStick != 0)) QuantityInSt = QuantityInGram / Good.GramsPerStick + (QuantityInSt % Good.GramsPerStick > 0 ? 1 : 0);

            //if (Unit == "g")
            //{
            //    if (Good.GramsPerStick != 0) QuantityInSt = QuantityInGram / Good.GramsPerStick + (QuantityInSt % Good.GramsPerStick > 0 ? 1 : 0);
            //    if (Good.GramsPerDeciliter != 0) QuantityInDl = QuantityInGram / Good.GramsPerDeciliter + QuantityInGram % Good.GramsPerDeciliter > 0 ? 1 : 0;
            //}
            //else if (Unit == "dl")
            //{
            //    if (Good.GramsPerDeciliter != 0) QuantityInDl = QuantityInGram / Good.GramsPerDeciliter + (QuantityInDl % Good.GramsPerDeciliter > 0 ? 1 : 0);
            //}
            //else if (Unit == "st")
            //{
            //    if (Good.GramsPerStick != 0) QuantityInSt = QuantityInGram / Good.GramsPerStick + (QuantityInSt % Good.GramsPerStick > 0 ? 1 : 0);
            //}
            //else if (Unit == "kg") {
            //    if (Good.GramsPerStick != 0) QuantityInSt = QuantityInGram / Good.GramsPerStick + (QuantityInSt % Good.GramsPerStick > 0 ? 1 : 0);
            //    if (Good.GramsPerDeciliter != 0) QuantityInDl = QuantityInGram / Good.GramsPerDeciliter + (QuantityInDl % Good.GramsPerDeciliter > 0 ? 1 : 0);
            //}
            //else if (Unit == "msk")
            //{
            //    if (Good.GramsPerDeciliter != 0) QuantityInDl = QuantityInGram / (Good.GramsPerDeciliter / 20 * 3) + (QuantityInMsk % Good.GramsPerDeciliter > 0 ? 1 : 0);
            //}
            //else if (Unit == "tsk")
            //{
            //    if (Good.GramsPerDeciliter != 0) QuantityInDl = QuantityInGram / (Good.GramsPerDeciliter / 20) + (QuantityInTsk % Good.GramsPerDeciliter > 0 ? 1 : 0);

            //}
            //else if ((Unit == "krm")) 
            //{
            //    if (Good.GramsPerDeciliter != 0) QuantityInDl = QuantityInGram / (Good.GramsPerDeciliter / 100) + (QuantityInKrm % Good.GramsPerDeciliter > 0 ? 1 : 0);
            //}
        }

        public void GetQuantityInGram(double q)
        {
            if (Unit == "g") QuantityInGram = (int)q;
            else if (Unit == "dl") QuantityInGram = (int)(Good.GramsPerDeciliter * q);
            else if (Unit == "st") QuantityInGram = (int)(Good.GramsPerStick * q);
            else if (Unit == "kg") QuantityInGram = (int)q * 1000;
            else if (Unit == "msk") QuantityInGram = (int)((Good.GramsPerDeciliter / 20 * 3) * q);
            else if (Unit == "tsk") QuantityInGram = (int)((Good.GramsPerDeciliter / 20) * q);
            else if (Unit == "krm") QuantityInGram = (int)((Good.GramsPerDeciliter / 100) * q);
        }



        public override string ToString()
        {
            return $"{Quantity} {Unit} {Good.Name}";
        }

        public int GetQuantity(Ingredient ingredient)
        {
            switch (ingredient.Unit)
            {
                case "g":
                    return QuantityInGram;
                case "dl":
                    return QuantityInDl;
                case "st":
                    return QuantityInSt;
                case "msk":
                    return QuantityInMsk;
                case "tsk":
                    return QuantityInTsk;
                case "krm":
                    return QuantityInKrm;
                default:
                    return QuantityInGram;
            }
        }
    }
}
