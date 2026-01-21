using CommunityToolkit.Mvvm.Messaging;
using Matsedeln.Usercontrols;
using Matsedeln.ViewModel;
using MatsedelnShared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Matsedeln.Pages
{
    /// <summary>
    /// Interaction logic for RecipePage.xaml
    /// </summary>
    public partial class RecipePage : Page
    {


        public RecipePage()
        {
            InitializeComponent();
            DataContext = new RecipePageViewModel();
        }
    }
}
