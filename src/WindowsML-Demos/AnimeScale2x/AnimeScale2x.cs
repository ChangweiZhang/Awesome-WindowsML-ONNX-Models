using System;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Media;
using Windows.Storage;
using WindowsMLDemos.Common;

// AnimeScale

namespace AnimeScale2x
{
    public sealed class AnimeScaleModelInput : IMachineLearningInput
    {
        public VideoFrame input { get; set; }
    }

    public sealed class AnimeScaleModelOutput : IMachineLearningOutput
    {
        public VideoFrame conv7 { get; set; }
        public AnimeScaleModelOutput()
        {
            this.conv7 = VideoFrame.CreateWithSoftwareBitmap(new Windows.Graphics.Imaging.SoftwareBitmap(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, 128, 128));
        }
    }

    public sealed class AnimeScaleModel : IMachineLearningModel
    {

        public LearningModelPreview LearningModel { get; set; }

        public static async Task<AnimeScaleModel> CreateAnimeScaleModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            AnimeScaleModel model = new AnimeScaleModel();
            model.LearningModel = learningModel;
            return model;
        }
        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as AnimeScaleModelInput;
            AnimeScaleModelOutput output = new AnimeScaleModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(LearningModel);
            binding.Bind("input", modelInput.input);
            binding.Bind("conv7", output.conv7);
            LearningModelEvaluationResultPreview evalResult = await LearningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
