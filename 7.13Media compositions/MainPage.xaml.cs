using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.Transcoding;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _7._13Media_compositions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaComposition composition;

        public MainPage()
        {
            this.InitializeComponent();
            composition = new MediaComposition();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".wmv");
            Windows.Storage.StorageFile pickedFile = await picker.PickSingleFileAsync();
            if (pickedFile == null)
            {
                return;
            }

            // These files could be picked from a location that we won't have access to later
            var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            storageItemAccessList.Add(pickedFile);

            var clip = await MediaClip.CreateFromFileAsync(pickedFile);
            composition.Clips.Add(clip);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".wmv");
            Windows.Storage.StorageFile pickedFile = await picker.PickSingleFileAsync();
            if (pickedFile == null)
            {
                return;
            }

            // These files could be picked from a location that we won't have access to later
            var storageItemAccessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            storageItemAccessList.Add(pickedFile);

            var clip = await MediaClip.CreateFromFileAsync(pickedFile);
            composition.Clips.Add(clip);
        }


        private MediaStreamSource mediaStreamSource;
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mediaStreamSource = composition.GeneratePreviewMediaStreamSource(
                                                (int)mediaElement.ActualWidth,
                                                (int)mediaElement.ActualHeight);
            mediaElement.SetMediaStreamSource(mediaStreamSource);

            await RenderCompositionToFile();
        }

        private async Task RenderCompositionToFile()
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeChoices.Add("MP4 files", new List<string>() { ".mp4" });
            picker.SuggestedFileName = "RenderedComposition.mp4";

            Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                // Call RenderToFileAsync
                var saveOperation = composition.RenderToFileAsync(file, MediaTrimmingPreference.Precise);

                saveOperation.Progress = new AsyncOperationProgressHandler<TranscodeFailureReason, double>(async (info, progress) =>
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                    {
                        Debug.WriteLine("保存进度:{0}",progress);
                    }));
                });
                saveOperation.Completed = new AsyncOperationWithProgressCompletedHandler<TranscodeFailureReason, double>(async (info, status) =>
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                    {
                        try
                        {
                            var results = info.GetResults();
                            if (results != TranscodeFailureReason.None || status != AsyncStatus.Completed)
                            {
                                Debug.WriteLine("保存失败");
                            }
                            else
                            {
                                Debug.WriteLine("保存成功");
                            }
                        }
                        finally
                        {
                            // Update UI whether the operation succeeded or not
                        }

                    }));
                });
            }
            else
            {
                Debug.WriteLine("用户取消保存");
            }
        }
    }
}
