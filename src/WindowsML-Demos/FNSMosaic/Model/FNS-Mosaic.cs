using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// FNSMosaic

namespace FNSMosaic
{
    public sealed class FNSMosaicModelInput
    {
        public VideoFrame inputImage { get; set; }
    }

    public sealed class FNSMosaicModelOutput
    {
        public VideoFrame outputImage { get; set; }
        public FNSMosaicModelOutput()
        {
            this.outputImage = VideoFrame.CreateWithSoftwareBitmap(new Windows.Graphics.Imaging.SoftwareBitmap(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, 720, 720));
        }
    }

    public sealed class FNSMosaicModel
    {
        private LearningModelPreview learningModel;
        public static async Task<FNSMosaicModel> CreateFNSMosaicModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            FNSMosaicModel model = new FNSMosaicModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<FNSMosaicModelOutput> EvaluateAsync(FNSMosaicModelInput input) {
            FNSMosaicModelOutput output = new FNSMosaicModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("inputImage", input.inputImage);
            binding.Bind("outputImage", output.outputImage);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
