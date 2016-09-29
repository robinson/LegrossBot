using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudSpeechAPI.v1beta1;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Legross.ChatClient.Helper
{
   public class GoogleTranscribeAsync
    {
        // [START authenticating]
        static public CloudSpeechAPIService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudSpeechAPIService.Scope.CloudPlatform
                });
            }
            return new CloudSpeechAPIService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Google Cloud Platform Speech Sample",
            });
        }
        // [END authenticating]
        public static List<string> Transcribe(string audio_file_path)
        {
            var service = CreateAuthorizedClient();
            // [START construct_request]
            var request = new Google.Apis.CloudSpeechAPI.v1beta1.Data.AsyncRecognizeRequest()
            {
                Config = new Google.Apis.CloudSpeechAPI.v1beta1.Data.RecognitionConfig()
                {
                    Encoding = "LINEAR16",
                    SampleRate = 16000,
                    LanguageCode = "vi-VN"
                },
                Audio = new Google.Apis.CloudSpeechAPI.v1beta1.Data.RecognitionAudio()
                {
                    Content = Convert.ToBase64String(File.ReadAllBytes(audio_file_path))
                }
            };
            // [END construct_request]
            // [START send_request]
            var asyncResponse = service.Speech.Asyncrecognize(request).Execute();
            var name = asyncResponse.Name;
            Google.Apis.CloudSpeechAPI.v1beta1.Data.Operation op;
            do
            {
                //Console.WriteLine("Waiting for server processing...");
                Thread.Sleep(1000);
                op = service.Operations.Get(name).Execute();
            } while (!(op.Done.HasValue && op.Done.Value));
            dynamic results = op.Response["results"];
            List<string> outcome = new List<string>();
            foreach (var result in results)
            {
                foreach (var alternative in result.alternatives)
                    outcome.Add(alternative.transcript);
                    //Console.WriteLine(alternative.transcript);
            }
            // [END send_request]
            return outcome;
        }
    }
}
