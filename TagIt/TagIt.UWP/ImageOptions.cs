using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;

namespace TagIt.UWP
{
    public class ImageOptions : IPlatformSpecific
    {
        private string messageShare;
        private StorageFile imageSource;

        //public async Task<byte[]> ResizeImage(byte[] imageData, int reqWidth, int reqHeight, int quality)
        public async Task<Stream> ResizeImage(int reqWidth, int reqHeight, int quality)
        {
            IRandomAccessStream imageStream = await this.imageSource.OpenAsync(FileAccessMode.Read);
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            if (decoder.PixelHeight > reqHeight || decoder.PixelWidth > reqWidth)
            {
                using (imageStream)
                {
                    var resizedStream = new InMemoryRandomAccessStream();

                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    double widthRatio = (double)reqWidth / decoder.PixelWidth;
                    double heightRatio = (double)reqHeight / decoder.PixelHeight;

                    double scaleRatio = Math.Min(widthRatio, heightRatio);

                    if (reqWidth == 0)
                        scaleRatio = heightRatio;

                    if (reqHeight == 0)
                        scaleRatio = widthRatio;

                    uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                    uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

                    encoder.BitmapTransform.ScaledHeight = aspectHeight;
                    encoder.BitmapTransform.ScaledWidth = aspectWidth;

                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    var outBuffer = new byte[resizedStream.Size];
                    await resizedStream.ReadAsync(outBuffer.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);
                    //return outBuffer;
                    return new MemoryStream(outBuffer);
                }
            }
            return await imageSource.OpenStreamForReadAsync();
        }

        //Register event share listener
        public void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareImageHandler);
        }

        private async void ShareImageHandler(DataTransferManager sender, DataRequestedEventArgs e)
        //private async void ShareImageHandler(string image_path, string message)
        {

            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = "TagIt";
            requestData.Properties.Description = this.messageShare;
            requestData.SetText(this.messageShare);
            requestData.Properties.ContentSourceApplicationLink = GetApplicationLink(GetType().Name);
            // Because we are making async calls in the DataRequested event handler,
            //  we need to get the deferral first.
            DataRequestDeferral deferral = e.Request.GetDeferral();
            
            // Make sure we always call Complete on the deferral.
            try
            {
                // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
                // since the target app may only support one or the other.
                List<IStorageItem> imageItems = new List<IStorageItem>();
                imageItems.Add(this.imageSource);
                requestData.SetStorageItems(imageItems);

                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(this.imageSource);
                requestData.Properties.Thumbnail = imageStreamRef;
                requestData.SetBitmap(imageStreamRef);
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                deferral.Complete();
            }
        }

        //Show share UI
        public void showShare()
        {
            DataTransferManager.ShowShareUI();
        }

        //Open file chooser
        public async Task<Stream> loadPhoto()
        {
            FileOpenPicker imagePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".png", ".bmp"}
            };

            StorageFile pickedImage = await imagePicker.PickSingleFileAsync();

            if (pickedImage != null)
            {
                this.imageSource = pickedImage;
                
                // Get image randomaccessstream
                IRandomAccessStream displayStream = await pickedImage.OpenAsync(FileAccessMode.Read);
                //Convert to byte[]
                var reader = new DataReader(displayStream.GetInputStreamAt(0));
                var bytes = new byte[displayStream.Size];
                await reader.LoadAsync((uint)displayStream.Size);
                reader.ReadBytes(bytes);
                //Return stream
                return new MemoryStream(bytes);
            }
            else
            {
                this.imageSource = null;
            }
            return null;
        }
                
        //Capture photo
        public async Task<Stream> capturePhoto()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            //captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (photo == null)
            {
                // User cancelled photo capture
                return null;
            }
            this.imageSource = photo;
            IRandomAccessStream displayStream = await photo.OpenAsync(FileAccessMode.Read);
            /*BitmapDecoder decoder = await BitmapDecoder.CreateAsync(displayStream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
        BitmapPixelFormat.Bgra8,
        BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            imageControl.Source = bitmapSource;*/

            //Convert to byte[]
            var reader = new DataReader(displayStream.GetInputStreamAt(0));
            var bytes = new byte[displayStream.Size];
            await reader.LoadAsync((uint)displayStream.Size);
            reader.ReadBytes(bytes);
            //Return stream
            return new MemoryStream(bytes);
        }

        //Set share options
        public bool setShareOptions(string message)
        {
            bool flagResult;
            try
            {
                /*StorageFile image = await StorageFile.GetFileFromPathAsync(imagePath);
                this.imageSource = image;*/
                this.messageShare = message;
                flagResult = true;
            }
            catch (Exception ex)
            {
                flagResult = false;
            }

            return flagResult;
        }

        public void setClipboardContent(string message)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(message);
            Clipboard.SetContent(dataPackage);
        }

        //Test
        protected static Uri GetApplicationLink(string sharePageName)
        {
            return new Uri("debroydev-tagit:navigate?page=" + sharePageName);
        }

        //Not implemented
        public byte[] ResizeImage(byte[] imageStream,int reqWidth, int reqHeight, int quality)
        {
            throw new NotImplementedException();
        }
    }
}
