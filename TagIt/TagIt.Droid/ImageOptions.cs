using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Xamarin.Forms;

namespace TagIt.Droid
{
    class ImageOptions : IPlatformSpecific
    {
        //Resize the image
        public byte[] ResizeImage(byte[] imageStream, int reqWidth, int reqHeight, int quality)
        {
            // Load the bitmap 
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageStream,0, imageStream.Length);
            if (originalImage.Height > reqHeight || originalImage.Width > reqWidth)
            {
                Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, reqWidth, reqHeight, false);
                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                    return ms.ToArray();
                }
            }
            return imageStream;
        }

        //Not implemented

        public Task<Stream> capturePhoto()
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> loadPhoto()
        {
            throw new NotImplementedException();
    }

        public void RegisterForShare()
        {
            throw new NotImplementedException();
        }

        public Task<Stream> ResizeImage(int reqWidth, int reqHeight, int quality)
        {
            throw new NotImplementedException();
        }
        
        public void setClipboardContent(string message)
        {
            throw new NotImplementedException();
        }

        public bool setShareOptions(string message)
        {
            throw new NotImplementedException();
        }

        public void showShare()
        {
            throw new NotImplementedException();
        }
    }
}