﻿using Avalonia.Controls;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS.Services
{
    public static class NotificationService
    {
        public static async void NetworkIssueMessage()
        {
            var alertBox = MessageBoxManager.GetMessageBoxStandard(
                new MessageBoxStandardParams
                {
                    ContentTitle = "Connection Issue",
                    ContentMessage = "We’re having a little trouble connecting right now. Please check your internet connection and try again. Thanks for your patience!",
                    ButtonDefinitions = ButtonEnum.Ok,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Width = 400,
                    ShowInCenter = true
                });
            Debug.WriteLine("Imong Api Goy Taronga!");

            await alertBox.ShowAsync();
        }
    }
}
