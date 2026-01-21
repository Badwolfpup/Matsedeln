using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Text;

namespace MatsedelnShared.Models
{
    public partial class Ingredient : ObservableValidator
    {
        [Key]
        public int Id { get; set; } = 0;
        [Required]
        [ObservableProperty]
        private string unit = string.Empty;

        [Required]
        [ObservableProperty]
        private double quantity;


        [property: NotMapped]
        [ObservableProperty]
        private int quantityInGram;
        [property: NotMapped]
        [ObservableProperty]
        private int quantityInDl;
        [property: NotMapped]
        [ObservableProperty]
        private int quantityInSt;
        [property: NotMapped]
        [ObservableProperty]
        private int quantityInMsk;
        [property: NotMapped]
        [ObservableProperty]
        private int quantityInTsk;
        [property: NotMapped]
        [ObservableProperty]
        private int quantityInKrm;

        [Required]
        public int GoodsId { get; set; }  // FK to Goods
        [Required]
        [ObservableProperty]
        private Goods _good;

        
        [Required]
        public int RecipeId { get; set; }  // FK to Recipe
        [Required]
        [ForeignKey(nameof(RecipeId))]
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

           
        }

        public void Initialize()
        {
            GetQuantityInGram(this.Quantity);
            ConvertToOtherUnits();
            AddUnitOptions();
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
