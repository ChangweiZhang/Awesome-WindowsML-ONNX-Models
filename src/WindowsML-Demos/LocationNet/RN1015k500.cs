using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using WindowsMLDemos.Common;

namespace LocationNet
{

    public sealed class RN1015k500Input:IMachineLearningInput
    {
        public ImageFeatureValue data; // shape(-1,3,224,224)
    }
    
    public sealed class RN1015k500Output:IMachineLearningOutput
    {
        public TensorString classLabel; // shape(-1,1)
        public IList<Dictionary<string,float>> softmax_output;
    }
    
    public sealed class RN1015k500Model:IMachineLearningModel
    {
       
        public LearningModel LearningModel { get; set; }
        public LearningModelSession Session { get; set; }
        public LearningModelBinding Binding { get; set; }

        public async Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input)
        {
            var modelInput = input as RN1015k500Input;
            Binding.Bind("data", modelInput.data);
            var result = await Session.EvaluateAsync(Binding, "0");
            var output = new RN1015k500Output();
            output.classLabel = result.Outputs["classLabel"] as TensorString;
            output.softmax_output = result.Outputs["softmax_output"] as IList<Dictionary<string, float>>;
            return output;
        }
    }
}
