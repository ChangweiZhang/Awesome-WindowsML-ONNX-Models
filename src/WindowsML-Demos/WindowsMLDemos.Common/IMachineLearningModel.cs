using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;

namespace WindowsMLDemos.Common
{
    public interface IMachineLearningModel
    {
        LearningModelPreview LearningModel { get; set; }

        Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input);
    }
}
