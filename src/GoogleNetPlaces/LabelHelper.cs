using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoogleNetPlaces
{
    public class LabelHelper
    {
        public static List<string> Labels = new List<string>();
        public static async Task LoadLabelAsync()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Model/IndoorOutdoor_places205.csv"));
            if (file != null)
            {
                Labels.Clear();
                using (var fs = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (var sr = new StreamReader(fs.AsStreamForRead()))
                    {
                        var txt = await sr.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(txt))
                        {
                            var lines = txt.Split('\n');
                            foreach(var line in lines)
                            {
                                var labelLine = line.Replace("\'","").Split(',')[0].Split('/').Last();
                                if (!string.IsNullOrEmpty(labelLine))
                                {
                                    Labels.Add(labelLine);
                                }
                            }
                        }

                    }
                }
            }
        }

    }
}
