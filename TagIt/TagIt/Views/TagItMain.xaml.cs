using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using Plugin.Media;
using Xamarin.Forms;

namespace TagIt.Views
{
    public partial class TagItMain : ContentPage
    {
        private Plugin.Media.Abstractions.MediaFile photo;
        private List<string> outTags;

        public TagItMain()
        {
            InitializeComponent();
            //Register share event (UWP)
            if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
            {
                App.PlatformSpecific.RegisterForShare();
            }

        }

        async void onNewImageClicked(object sender, EventArgs e)
        {
            try
            {

                if (Device.OS == TargetPlatform.Android)
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        //Directory = "Camera Roll",
                        //Name = "test.jpg",

                        SaveToAlbum = true
                    });

                    if (photo == null)
                    {
                        imgContainer.Source = null;
                        return;
                    }

                    imgContainer.Source = ImageSource.FromStream(() =>
                    {
                        //var stream = file.GetStream();
                        var stream = photo.GetStream();
                        //file.Dispose();
                        //photo.Dispose();
                        return stream;
                    });
                }

                else if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
                {
                    System.IO.Stream photoSource = await App.PlatformSpecific.capturePhoto();
                    if (photoSource != null)
                    {
                        imgContainer.Source = ImageSource.FromStream(() =>
                        {
                            var stream = photoSource;
                            return stream;
                        });
                    }
                }

                lblContainer.Children.Clear();

            }
            catch (Exception ex)
            {
                return;
            }
        }

        async void onLoadImageClicked(object sender, EventArgs e)
        {
            // Select a photo. 
            try
            {
                aiLoading.IsRunning = true;
                aiLoading.IsVisible = true;
                if (Device.OS == TargetPlatform.Android)
                {
                    if (CrossMedia.Current.IsPickPhotoSupported)
                    {
                        photo = await CrossMedia.Current.PickPhotoAsync();
                        if (photo != null)
                        {
                            imgContainer.Source = ImageSource.FromStream(() =>
                            {
                                var stream = photo.GetStream();
                                return stream;
                            });
                        }
                        else
                        {
                            imgContainer.Source = null;
                        }
                    }
                }
                else if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
                {
                    System.IO.Stream photoSource = await App.PlatformSpecific.loadPhoto();
                    if (photoSource != null)
                    {
                        imgContainer.Source = ImageSource.FromStream(() =>
                        {
                            var stream = photoSource;
                            return stream;
                        });
                    }
                }
                lblContainer.Children.Clear();
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                aiLoading.IsRunning = false;
                aiLoading.IsVisible = false;
            }
        }

        async void onAnalyzeClicked(object sender, EventArgs e)
        {
            try
            {
                aiLoading.IsRunning = true;
                aiLoading.IsVisible = true;
                if (Device.OS == TargetPlatform.Android)
                {
                    if (photo == null)
                    {
                        return;
                    }
                    var newImage = App.PlatformSpecific.ResizeImage(streamToByte(photo.GetStream()), 1000, 1000, 1);
                    imgContainer.Source = ImageSource.FromStream(()=> { return new MemoryStream(newImage); });
                    Stream img = new MemoryStream(newImage);
                    CognitiveService service = new CognitiveService();
                    AnalysisResult response = await service.GetImageDescription(img);
                    outTags = new List<string> { };

                    foreach (var item in response.Description.Tags)
                    {
                        outTags.Add("#" + item.ToString());
                        Button btnItem = new Button
                        {
                            Text = "#" + item.ToString(),
                            TextColor = Xamarin.Forms.Color.Blue
                        };
                        btnItem.Clicked += btnTagRemove;
                        lblContainer.Children.Add(btnItem);
                    }
                }
                else if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.WinPhone)
                {
                    var newImage = await App.PlatformSpecific.ResizeImage(1000, 1000, 1);


                    CognitiveService service = new CognitiveService();
                    AnalysisResult response = await service.GetImageDescription(newImage);
                    outTags = new List<string> { };

                    foreach (var item in response.Description.Tags)
                    {
                        outTags.Add("#" + item.ToString());
                        Button btnItem = new Button
                        {
                            Text = "#" + item.ToString(),
                            TextColor = Xamarin.Forms.Color.Blue
                        };
                        btnItem.Clicked += btnTagRemove;
                        lblContainer.Children.Add(btnItem);
                    }
                }
            }
            catch(Exception ex)
            {
                return;
            }
            finally
            {
                aiLoading.IsRunning = false;
                aiLoading.IsVisible = false;
            }
        }

        void onShare(object sender, EventArgs e)
        {
            try
            {
                if (outTags.Count >= 1)
                {
                    bool result = App.PlatformSpecific.setShareOptions(string.Join(" ",outTags.ToArray()));
                    if (result)
                    {
                        App.PlatformSpecific.setClipboardContent(string.Join(" ", outTags.ToArray()));
                        App.PlatformSpecific.showShare();
                    }
                }                
            }
            catch (Exception ex)
            {
                return;
            }
        }
        
        void btnTagRemove(object sender, EventArgs e)
        {
            var element = (Button)sender;
            outTags.Remove(element.Text);
            lblContainer.Children.Remove(element);
        }                  

        //Utilities
        byte[] streamToByte(Stream InputStream)
        {
            byte[] result;
            using (var streamReader = new MemoryStream())
            {
                InputStream.CopyTo(streamReader);
                result = streamReader.ToArray();
                return result;
            }

        }
    }
}
