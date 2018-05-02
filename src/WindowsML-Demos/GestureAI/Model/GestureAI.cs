using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// GestureAI

namespace GestureAI
{
    public sealed class GestureAIModelInput
    {
        public IList<float> input1 { get; set; }
    }

    public sealed class GestureAIModelOutput
    {
        public IList<float> gru_1_h_out { get; set; }
        public IList<float> output1 { get; set; }
        public GestureAIModelOutput()
        {
            this.gru_1_h_out = new List<float>();
            this.output1 = new List<float>();
        }
    }

    public sealed class GestureAIModel
    {
        private LearningModelPreview learningModel;
        public static async Task<GestureAIModel> CreateGestureAIModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            GestureAIModel model = new GestureAIModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<GestureAIModelOutput> EvaluateAsync(GestureAIModelInput input) {
            GestureAIModelOutput output = new GestureAIModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("input1", input.input1);
            binding.Bind("gru_1_h_out", output.gru_1_h_out);
            binding.Bind("output1", output.output1);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
