using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// ResNet50

namespace ResNet50
{
    public sealed class ResNet50ModelInput
    {
        public VideoFrame image { get; set; }
    }

    public sealed class ResNet50ModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> classLabelProbs { get; set; }
        public ResNet50ModelOutput()
        {
            this.classLabel = new List<string>();
            this.classLabelProbs = new Dictionary<string, float>();
        }
    }

    public sealed class ResNet50Model
    {
        private LearningModelPreview learningModel;
        public static async Task<ResNet50Model> CreateResNet50Model(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            ResNet50Model model = new ResNet50Model();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<ResNet50ModelOutput> EvaluateAsync(ResNet50ModelInput input) {
            ResNet50ModelOutput output = new ResNet50ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("image", input.image);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("classLabelProbs", output.classLabelProbs);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
