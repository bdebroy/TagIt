using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TagIt.Views
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            sldrAccuracy.Value = 45;
            App.TagAccuracy = sldrAccuracy.Value;
        }

        public void onSliderChange(object sender, ValueChangedEventArgs e)
        {
            //App.TagAccuracy = sldrAccuracy.Value;
        }
    }
}
