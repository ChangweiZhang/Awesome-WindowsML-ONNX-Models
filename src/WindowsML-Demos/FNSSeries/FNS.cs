using System;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using WindowsMLDemos.Common;

namespace FNSSeries
{

    public sealed class FNSInput:IMachineLearningInput
    {
        public ImageFeatureValue inputImage; // shape(-1,3,720,720)
    }
    
    public sealed class FNSOutput:IMachineLearningOutput
    {
        public TensorFloat16Bit outputImage; // shape(-1,3,720,720)
    }
    
    public sealed class FNSModel:IMachineLearningModel
    {
        public LearningModel LearningModel { get; set; }
        public LearningModelSession Session { get; set; }
        public LearningModelBinding Binding { get; set; }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as FNSInput;
            Binding.Bind("inputImage", modelInput.inputImage);
            var result = await Session.EvaluateAsync(Binding, "0");
            var output = new FNSOutput();
            output.outputImage = result.Outputs["outputImage"] as TensorFloat16Bit;
            return output;
        }
    }
}
