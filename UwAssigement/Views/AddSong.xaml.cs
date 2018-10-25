using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using UwAssigement.Entity;
using UwAssigement.Service;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UwAssigement.Views
{
    public sealed partial class AddSong : ContentDialog
    {
        private Song currentSong;

        public AddSong()
        {
            this.currentSong = new Song();
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void SaveSong(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file = await folder.GetFileAsync("token.txt");
            var tokenContent = await FileIO.ReadTextAsync(file);

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(tokenContent);

            // Lay thong tin ca nhan bang token.
            HttpClient client2 = new HttpClient();
            client2.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
            var resp = client2.GetAsync("http://2-dot-backup-server-002.appspot.com/_api/v2/members/information").Result;
            Debug.WriteLine(await resp.Content.ReadAsStringAsync());
            var userInfoContent = await resp.Content.ReadAsStringAsync();

            Member userInfoJson = JsonConvert.DeserializeObject<Member>(userInfoContent);

            // do validate first.
            this.currentSong.name = this.NameSong.Text;
            this.currentSong.description = this.Description.Text;
            this.currentSong.singer = this.Singer.Text;
            this.currentSong.author = this.Author.Text;
            this.currentSong.thumbnail = this.Thumbnail.Text;
            this.currentSong.link = this.Link.Text;
            //Debug.WriteLine("Action success.");
            //currentSong.memberId = userInfoJson.id;

            string content = await ApiHandle.Create_Song(this.currentSong);

            var dialog = new Windows.UI.Popups.MessageDialog("Upload thành công");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK") { Id = 1 });
            dialog.CancelCommandIndex = 1;
            await dialog.ShowAsync();
            //Reset form
            NameSong.Text = "";
            Description.Text = "";
            Singer.Text = "";
            Author.Text = "";
            Thumbnail.Text = "";
            Link.Text = "";
        }

        private void BtnReset(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
