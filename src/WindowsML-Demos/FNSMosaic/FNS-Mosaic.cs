using System;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Media;
using WindowsMLDemos.Common;
// FNSMosaic

namespace FNSMosaic
{
    public sealed class FNSMosaicModelInput : IMachineLearningInput
    {
        public VideoFrame inputImage { get; set; }
    }

    public sealed class FNSMosaicModelOutput : IMachineLearningOutput
    {
        public VideoFrame outputImage { get; set; }
        public FNSMosaicModelOutput()
        {
            this.outputImage = VideoFrame.CreateWithSoftwareBitmap(new Windows.Graphics.Imaging.SoftwareBitmap(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, 720, 720));
        }
    }

    public sealed class FNSMosaicModel : IMachineLearningModel
    {
        public LearningModelPreview LearningModel { get; set; }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as FNSMosaicModelInput;
            FNSMosaicModelOutput output = new FNSMosaicModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(LearningModel);
            binding.Bind("inputImage", modelInput.inputImage);
            binding.Bind("outputImage", output.outputImage);
            LearningModelEvaluationResultPreview evalResult = await LearningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
