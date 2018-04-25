using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace WindowML_Demos.Common
{
    public class AlertHelper
    {
        private static MessageDialog dialog = new MessageDialog(string.Empty);
        public async static Task ShowMessage(string message)
        {
            dialog.Content = message;
            await dialog.ShowAsync();
        }
    }
}
