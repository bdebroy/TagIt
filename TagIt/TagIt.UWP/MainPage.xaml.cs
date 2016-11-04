using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TagIt.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                TagIt.App.Init(new ImageOptions());
                LoadApplication(new TagIt.App());
            }
            catch (Exception ex)
            {
                TextBlock exceptionText = new TextBlock();
                exceptionText.Text = ex.ToString();
                mainGrid.Children.Add(exceptionText);
            }
        }
    }
}
