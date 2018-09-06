using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.AI.MachineLearning;
namespace LocationNet
{
    
    public sealed class RN1015k500Input
    {
        public TensorFloat16Bit data; // shape(-1,3,224,224)
    }
    
    public sealed class RN1015k500Output
    {
        public TensorString classLabel; // shape(-1,1)
        public IList<Dictionary<string,float>> softmax_output;
    }
    
    public sealed class RN1015k500Model
    {
        private LearningModel model;
        private LearningModelSession session;
        private LearningModelBinding binding;
        public static async Task<RN1015k500Model> CreateFromStreamAsync(IRandomAccessStreamReference stream)
        {
            RN1015k500Model learningModel = new RN1015k500Model();
            learningModel.model = await LearningModel.LoadFromStreamAsync(stream);
            learningModel.session = new LearningModelSession(learningModel.model);
            learningModel.binding = new LearningModelBinding(learningModel.session);
            return learningModel;
        }
        public async Task<RN1015k500Output> EvaluateAsync(RN1015k500Input input)
        {
            binding.Bind("data", input.data);
            var result = await session.EvaluateAsync(binding, "0");
            var output = new RN1015k500Output();
            output.classLabel = result.Outputs["classLabel"] as TensorString;
            output.softmax_output = result.Outputs["softmax_output"] as IList<Dictionary<string,float>>;
            return output;
        }
    }
}
