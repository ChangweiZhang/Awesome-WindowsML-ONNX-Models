using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using WindowsMLDemos.Common;

// GoogLeNetPlacesModel

namespace GoogleNetPlaces
{
    public sealed class GoogLeNetPlacesInput : IMachineLearningInput
    {
        public ImageFeatureValue sceneImage { get; set; }
    }

    public sealed class GoogLeNetPlacesOutput : IMachineLearningOutput
    {
        public TensorString sceneLabel; // shape(-1,1)
        public IList<Dictionary<string, float>> sceneLabelProbs;
        public GoogLeNetPlacesOutput()
        {
        }
    }

    public sealed class GoogLeNetPlacesModel : IMachineLearningModel
    {
        public LearningModel LearningModel
        {
            get; set;
        }
        public LearningModelSession Session { get; set; }
        public LearningModelBinding Binding { get; set; }


        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as GoogLeNetPlacesInput;
            Binding.Bind("sceneImage", modelInput.sceneImage);
            var result = await Session.EvaluateAsync(Binding, "0");
            var output = new GoogLeNetPlacesOutput();
            output.sceneLabel = result.Outputs["sceneLabel"] as TensorString;
            output.sceneLabelProbs = result.Outputs["sceneLabelProbs"] as IList<Dictionary<string, float>>;
            return output;
        }
    }
}
