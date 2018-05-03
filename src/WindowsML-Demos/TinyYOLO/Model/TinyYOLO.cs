using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// TinyYOLOModel

namespace TinyYOLO
{
    public sealed class TinyYOLOModelModelInput
    {
        public VideoFrame image { get; set; }
    }

    public sealed class TinyYOLOModelModelOutput
    {
        public IList<float> grid { get; set; }
        public TinyYOLOModelModelOutput()
        {
            this.grid = new List<float>();
        }
    }

    public sealed class TinyYOLOModelModel
    {
        private LearningModelPreview learningModel;
        public static async Task<TinyYOLOModelModel> CreateTinyYOLOModelModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            TinyYOLOModelModel model = new TinyYOLOModelModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<TinyYOLOModelModelOutput> EvaluateAsync(TinyYOLOModelModelInput input) {
            TinyYOLOModelModelOutput output = new TinyYOLOModelModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("image", input.image);
            binding.Bind("grid", output.grid);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
