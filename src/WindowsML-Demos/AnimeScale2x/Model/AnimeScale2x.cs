using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// AnimeScale

namespace AnimeScale2x
{
    public sealed class AnimeScaleModelInput
    {
        public VideoFrame input { get; set; }
    }

    public sealed class AnimeScaleModelOutput
    {
        public VideoFrame conv7 { get; set; }
        public AnimeScaleModelOutput()
        {
            this.conv7 = VideoFrame.CreateWithSoftwareBitmap(new Windows.Graphics.Imaging.SoftwareBitmap(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, 128, 128));
        }
    }

    public sealed class AnimeScaleModel
    {
        private LearningModelPreview learningModel;
        public static async Task<AnimeScaleModel> CreateAnimeScaleModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            AnimeScaleModel model = new AnimeScaleModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<AnimeScaleModelOutput> EvaluateAsync(AnimeScaleModelInput input) {
            AnimeScaleModelOutput output = new AnimeScaleModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("input", input.input);
            binding.Bind("conv7", output.conv7);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
