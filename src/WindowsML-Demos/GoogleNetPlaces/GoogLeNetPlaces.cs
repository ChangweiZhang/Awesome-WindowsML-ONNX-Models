using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;
using WindowsMLDemos.Common;

// GoogLeNetPlacesModel

namespace GoogleNetPlaces
{
    public sealed class GoogLeNetPlacesModelModelInput : IMachineLearningInput
    {
        public VideoFrame sceneImage { get; set; }
    }

    public sealed class GoogLeNetPlacesModelModelOutput : IMachineLearningOutput
    {
        public IList<string> sceneLabel { get; set; }
        public IDictionary<string, float> sceneLabelProbs { get; set; }
        public GoogLeNetPlacesModelModelOutput()
        {
            this.sceneLabel = new List<string>();
            this.sceneLabelProbs = new Dictionary<string, float>();
            for(var i = 0; i < 205; i++)
            {
                sceneLabelProbs[i.ToString()] = float.NaN;
            }
        }
    }

    public sealed class GoogLeNetPlacesModelModel : IMachineLearningModel
    {
        public LearningModelPreview LearningModel
        {
            get; set;
        }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as GoogLeNetPlacesModelModelInput;
            GoogLeNetPlacesModelModelOutput output = new GoogLeNetPlacesModelModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(LearningModel);
            binding.Bind("sceneImage", modelInput.sceneImage);
            binding.Bind("sceneLabel", output.sceneLabel);
            binding.Bind("sceneLabelProbs", output.sceneLabelProbs);
            LearningModelEvaluationResultPreview evalResult = await LearningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
