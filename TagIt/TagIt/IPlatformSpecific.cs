using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagIt
{
    public interface IPlatformSpecific
    {
        Task<Stream> ResizeImage(int reqWidth, int reqHeight, int quality);
        byte[] ResizeImage(byte[] imageStream,int reqWidth, int reqHeight, int quality);
        void showShare();
        void RegisterForShare();
        bool setShareOptions(string message);
        Task<Stream> loadPhoto();
        Task<Stream> capturePhoto();
        void setClipboardContent(string message);
    }
}
