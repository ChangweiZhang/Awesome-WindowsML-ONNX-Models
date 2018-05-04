using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;
using WindowsMLDemos.Common;
// ResNet50

namespace ResNet50
{
    public sealed class ResNet50ModelInput : IMachineLearningInput
    {
        public VideoFrame image { get; set; }
    }

    public sealed class ResNet50ModelOutput : IMachineLearningOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> classLabelProbs { get; set; }
        public ResNet50ModelOutput()
        {
            this.classLabel = new List<string>();
            this.classLabelProbs = new Dictionary<string, float>();
            for (var i = 0; i < 999; i++)
            {
                classLabelProbs[i.ToString()] = float.NaN;
            }
        }
    }

    public sealed class ResNet50Model : IMachineLearningModel
    {
        public LearningModelPreview LearningModel { get; set; }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as ResNet50ModelInput;
            ResNet50ModelOutput output = new ResNet50ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(LearningModel);
            binding.Bind("image", modelInput.image);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("classLabelProbs", output.classLabelProbs);
            LearningModelEvaluationResultPreview evalResult = await LearningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
