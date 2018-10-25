﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using UwAssigement.Entity;
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
    public sealed partial class Login : ContentDialog
    {
        private Member currentMember;
        private static StorageFile file;
        private static string UploadUrl;
        private static string API_LOGIN = "http://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
        public Login()
        {
            this.InitializeComponent();
        }
       
        private void btnMoveRegister(object sender, RoutedEventArgs e)
        {
            Frame rootFram = Window.Current.Content as Frame;
            rootFram.Navigate(typeof(Views.Register));
            this.Hide();
        }

        private void Login_Handle(object sender, TappedRoutedEventArgs e)
        {
            var email = Email.Text;
            var password = Password.Password.ToString();

            if (email == "")
            {
                Email_Message.Text = "Vui lòng nhập Email.";
                Email_Message.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                Email_Message.Text = "";
            }
            if (password == "")
            {
                Password_Message.Text = "Vui lòng nhập mật khẩu.";
                Password_Message.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                Password_Message.Text = "";
            }
            if (email != "" && password != "")
            {
                this.Hide();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<String, String> LoginInfor = new Dictionary<string, string>();
            LoginInfor.Add("email", this.Email.Text);
            LoginInfor.Add("password", this.Password.Password);

            // Lay token
            HttpClient httpClient = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(LoginInfor), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(API_LOGIN, content).Result;
            var responseContent = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(response.StatusCode);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                // save file...
                Debug.WriteLine(responseContent);
                // Doc token
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Luu token
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, responseContent);

            }
            else
            {
                // Xu ly loi.
                ErrorResponse errorObject = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                if (errorObject != null && errorObject.error.Count > 0)
                {
                    foreach (var key in errorObject.error.Keys)
                    {
                        var textMessage = this.FindName(key);
                        if (textMessage == null)
                        {
                            continue;
                        }
                        TextBlock textBlock = textMessage as TextBlock;
                        textBlock.Text = errorObject.error[key];
                        textBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
