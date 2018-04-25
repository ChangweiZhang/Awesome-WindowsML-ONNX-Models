using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// FNSLaMuseModel

namespace FNS_La_Muse
{
    public sealed class FNSLaMuseModelModelInput
    {
        public VideoFrame inputImage { get; set; }
    }

    public sealed class FNSLaMuseModelModelOutput
    {
        public VideoFrame outputImage { get; set; }
        public FNSLaMuseModelModelOutput()
        {
            this.outputImage = VideoFrame.CreateWithSoftwareBitmap(new Windows.Graphics.Imaging.SoftwareBitmap(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, 720, 720));
        }
    }

    public sealed class FNSLaMuseModelModel
    {
        private LearningModelPreview learningModel;
        public static async Task<FNSLaMuseModelModel> CreateFNSLaMuseModelModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            FNSLaMuseModelModel model = new FNSLaMuseModelModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<FNSLaMuseModelModelOutput> EvaluateAsync(FNSLaMuseModelModelInput input) {
            FNSLaMuseModelModelOutput output = new FNSLaMuseModelModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("inputImage", input.inputImage);
            binding.Bind("outputImage", output.outputImage);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
