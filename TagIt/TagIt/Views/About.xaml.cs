using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TagIt.Views
{
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();
            AboutText.Text = "TagIt# is an easy way to generate hashtags for your images and photos."+Environment.NewLine+
                "Just load an image/photo from your gallery  or take a new one with the \"New\" button."+ Environment.NewLine +
                "Now press the \"Analyze\" button and your hashtags will be created!" + Environment.NewLine +
                "Want to delete one or more hashtags, just tap it and it will be deleted." + Environment.NewLine +
                "Are you ready for share? Just tap the \"Share\" button and choose the app!" + Environment.NewLine +
                "** Due to limitations, the hashtags and image can't be automatic shared, instead" + Environment.NewLine +
                "TagIt will share the image and copy the hashtags to the clipboard, you only have to paste it **";
        }
    }
}
