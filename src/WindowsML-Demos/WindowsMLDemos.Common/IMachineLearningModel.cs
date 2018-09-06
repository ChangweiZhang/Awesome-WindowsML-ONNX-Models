using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace WindowsMLDemos.Common
{
    public interface IMachineLearningModel
    {
        LearningModel LearningModel
        {
            get; set;
        }
        LearningModelSession Session { get; set; }
        LearningModelBinding Binding { get; set; }

        Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input);
    }
}
