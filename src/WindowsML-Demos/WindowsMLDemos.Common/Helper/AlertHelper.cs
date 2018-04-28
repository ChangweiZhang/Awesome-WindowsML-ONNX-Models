using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace WindowsMLDemos.Common.Helper
{
    public class AlertHelper
    {
        public static async Task ShowMessageAsync(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}
