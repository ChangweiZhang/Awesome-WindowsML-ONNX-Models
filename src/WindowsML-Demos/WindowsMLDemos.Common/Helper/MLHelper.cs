using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Storage;

namespace WindowsMLDemos.Common.Helper
{
    public class MLHelper
    {
        /// <summary>
        /// init a ML model
        /// </summary>
        /// <param name="file"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async static Task CreateModelAsync(StorageFile file, IMachineLearningModel model)
        {
            var learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            model.LearningModel = learningModel;
        }
        /// <summary>
        /// evaluate a model
        /// </summary>
        /// <param name="input"></param>
        /// <param name="learningModel"></param>
        /// <returns></returns>
        public async static Task<IMachineLearningOutput> EvaluateAsync(IMachineLearningInput input, IMachineLearningModel learningModel)
        {
            var output = await learningModel.EvaluateAsync(input);
            return output;
        }
    }
}
