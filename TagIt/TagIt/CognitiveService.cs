using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;

namespace TagIt
{
    class CognitiveService
    {
        public async Task<AnalysisResult> GetImageDescription(Stream imageStream)
        {
            //Log("Calling VisionServiceClient.AnalyzeImageAsync()...");

            VisualFeature[] features = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Description };
            //Replace xxx with your service cliente code
            VisionServiceClient visionClient = new VisionServiceClient("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            //VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };
            AnalysisResult analysisResult = await visionClient.AnalyzeImageAsync(imageStream, features);

            return analysisResult;
        }
    }
}
