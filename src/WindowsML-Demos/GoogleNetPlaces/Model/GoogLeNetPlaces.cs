using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// GoogLeNetPlacesModel

namespace GoogleNetPlaces
{
    public sealed class GoogLeNetPlacesModelModelInput
    {
        public VideoFrame sceneImage { get; set; }
    }

    public sealed class GoogLeNetPlacesModelModelOutput
    {
        public IList<string> sceneLabel { get; set; }
        public IDictionary<string, float> sceneLabelProbs { get; set; }
        public GoogLeNetPlacesModelModelOutput()
        {
            this.sceneLabel = new List<string>();
            this.sceneLabelProbs = new Dictionary<string, float>();
        }
    }

    public sealed class GoogLeNetPlacesModelModel
    {
        private LearningModelPreview learningModel;
        public static async Task<GoogLeNetPlacesModelModel> CreateGoogLeNetPlacesModelModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            GoogLeNetPlacesModelModel model = new GoogLeNetPlacesModelModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<GoogLeNetPlacesModelModelOutput> EvaluateAsync(GoogLeNetPlacesModelModelInput input) {
            GoogLeNetPlacesModelModelOutput output = new GoogLeNetPlacesModelModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("sceneImage", input.sceneImage);
            binding.Bind("sceneLabel", output.sceneLabel);
            binding.Bind("sceneLabelProbs", output.sceneLabelProbs);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
