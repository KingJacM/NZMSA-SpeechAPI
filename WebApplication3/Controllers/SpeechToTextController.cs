using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using System;
using Google.Apis.Auth.OAuth2;
using WebApplication3.Data;
using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Speech.V1P1Beta1;
using Google.LongRunning;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechToTextController : ControllerBase
    {

        public static GoogleCredential credential = GoogleCredential.FromFile("./Controllers/speech-to-text-359100-3dd6a60c06e6.json");
       // public static GoogleCredential speechCred = GoogleCredential.FromFile("./Controllers/oceanic-bindery-356922-db4ff8676115.json");
        //public static GoogleCredential credential = GoogleCredential.GetApplicationDefault();
        public static StorageClient storage = StorageClient.Create(credential);
        private readonly FileDBContext _context;

        public SpeechToTextController(FileDBContext context)
        {
            _context = context;

        }


        /// <summary> 
        /// Get all file names from the storage bucket
        /// </summary>
        [HttpGet]
        public string Get()
        {
            
            // Make an authenticated API request.
            string str = "";
            foreach (var obj in _context.SpeechFiles)
            {
                str += (obj.Name+"\n");
            }
            return str;
        }

        /// <summary> 
        /// Upload a file to the storage bucket to recieve transcribed text - mp3 only
        /// </summary>
        [DisableRequestSizeLimit]
        [HttpPost]
        public string UploadFile(IFormFile file)
        {
            var results = "";
            var ms = new MemoryStream();
            
            file.CopyTo(ms);
         
            // Make an authenticated API request.
            var buckets = storage.ListBuckets("speech-to-text-359100");
            
            var find = _context.SpeechFiles.FirstOrDefault(f => f.Name == file.FileName);
            var data = new SpeechFile
            {
                Name = file.FileName,
                isVIP = false,
                Type = "audio/mpeg3"
            };
            if (find == null)
            {
                var obj = storage.UploadObject("msa-speech-to-text-2", file.FileName, null, ms);
                _context.Add(data);
                _context.SaveChanges();
            }


            var speech = new SpeechClientBuilder
            {
                CredentialsPath = "./Controllers/speech-to-text-359100-3dd6a60c06e6.json"
            }.Build();
            var config = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Mp3,
                MaxAlternatives = 1,
                LanguageCode = LanguageCodes.English.UnitedStates,
                EnableWordTimeOffsets = true,
                SampleRateHertz=8000
            };
            var audio = RecognitionAudio.FromStorageUri("gs://msa-speech-to-text-2/" + file.FileName);


            var response =  speech.LongRunningRecognize(config, audio);
            response = response.PollUntilCompleted();
            //if (response.IsCompleted) { //LongRunningRecognizeResponse r = completedResponse.Result;
                //Console.WriteLine(response);
            Console.WriteLine(response.Result); 
            foreach (var result in response.Result.Results)
            {
                foreach (var alternative in result.Alternatives)
                    {
                    results += (alternative.Transcript + "\n");


                    }
                }
            
            
           






            return results;
        }

        /// <summary> 
        /// PUT method - change File's accessibility on membership
        /// </summary>
        [HttpPut("{n}")]
        public void Put(string n)
        {
            var obj = _context.SpeechFiles.Single(f => f.Name == n);
            obj.isVIP = !obj.isVIP;
            _context.Update(obj);
            _context.SaveChanges();

        }

        /// <summary> 
        /// Delete a file by name
        /// </summary>
        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            // Make an authenticated API request.
            var buckets = storage.ListBuckets("speech-to-text-359100");
            storage.DeleteObjectAsync("msa-speech-to-text-2", name);
            _context.Remove(_context.SpeechFiles.Single(f => f.Name==name));
            _context.SaveChanges();
        }
    
    }
}
