using System;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using Ditat.Logic.Models;
using Ditat.Logic.Interfaces;
using System.IO;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Training;
using System.Linq;

namespace Ditat.Logic.Providers
{
    public class ClassifierProvider : IClassifierProvider
    {
        public ClassifierResponse ClassifyImage(string  path)
        {
            var projectId = GetProjectId();
            var endpoint = GetPredictionEndpoint();

            var memoryStream = new MemoryStream(File.ReadAllBytes(path));

            var prediction = endpoint.PredictImage(projectId, memoryStream);

            var topPrediction = prediction.Predictions.First();

            var classifierResponse = new ClassifierResponse();

            classifierResponse.Category = topPrediction.Tag;
            classifierResponse.CategoryPorpability = topPrediction.Probability;

            return classifierResponse;
        }

        private static Guid GetProjectId()
        {
            var trainingApi = new TrainingApi();
            
            trainingApi.ApiKey = Properties.Settings.Default.Classifier_TrainingApiKey; 

            var projectName = Properties.Settings.Default.Classifier_ProjectName; 
            var project = trainingApi.GetProjects().Single(p => p.Name == projectName);

            return project.Id;
        }

        private static PredictionEndpoint GetPredictionEndpoint()
        {
            var predictionEndpoint = new PredictionEndpoint();
            predictionEndpoint.ApiKey = Properties.Settings.Default.Classifier_PredictionApiKey;
            return predictionEndpoint;
        }

        public ClassifierResponse ClassifyImage(FileInfo imageFileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
