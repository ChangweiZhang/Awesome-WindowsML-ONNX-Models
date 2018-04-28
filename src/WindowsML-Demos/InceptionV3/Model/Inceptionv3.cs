using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// Inceptionv3

namespace InceptionV3
{
    public sealed class Inceptionv3ModelInput
    {
        public VideoFrame image { get; set; }
    }

    public sealed class Inceptionv3ModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> classLabelProbs { get; set; }
        public Inceptionv3ModelOutput()
        {
            this.classLabel = new List<string>();
            this.classLabelProbs = new Dictionary<string, float>();
        }
    }

    public sealed class Inceptionv3Model
    {
        private LearningModelPreview learningModel;
        public static async Task<Inceptionv3Model> CreateInceptionv3Model(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            Inceptionv3Model model = new Inceptionv3Model();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<Inceptionv3ModelOutput> EvaluateAsync(Inceptionv3ModelInput input) {
            Inceptionv3ModelOutput output = new Inceptionv3ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("image", input.image);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("classLabelProbs", output.classLabelProbs);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
