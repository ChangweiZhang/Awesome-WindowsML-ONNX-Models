using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using WindowsMLDemos.Common;

namespace ResNet50
{

    public sealed class Resnet50Input : IMachineLearningInput
    {
        public ImageFeatureValue image; // shape(-1,3,224,224)
    }

    public sealed class Resnet50Output : IMachineLearningOutput
    {
        public TensorString classLabel; // shape(-1,1)
        public IList<Dictionary<string, float>> classLabelProbs;
    }

    public sealed class Resnet50Model : IMachineLearningModel
    {
        public LearningModel LearningModel { get; set; }
        public LearningModelSession Session { get; set; }
        public LearningModelBinding Binding { get; set; }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as Resnet50Input;
            Binding.Bind("image", modelInput.image);
            var result = await Session.EvaluateAsync(Binding, "0");
            var output = new Resnet50Output();
            output.classLabel = result.Outputs["classLabel"] as TensorString;
            output.classLabelProbs = result.Outputs["classLabelProbs"] as IList<Dictionary<string, float>>;
            return output;
        }
    }
}
