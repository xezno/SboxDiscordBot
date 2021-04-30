using Newtonsoft.Json;
using RSG;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/*
 * Fetch-style API based on the documentation located at https://github.github.io/fetch/
 */

namespace SboxDiscordBot
{
    public enum FetchBodyTypes
    {
        String,
        UrlSearchParams,
        FormData,
        Blob,
        ArrayBuffer,
        TypedArray,
        DataView
    };

    public class FetchOptions
    {
        // TODO: Implement all of below
        public string Method { get; set; } = "GET";
        public string Body { get; set; } = "";
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Credentials { get; set; } = "omit";
    }

    public class FetchResponse
    {
        public int Status { get; set; } = 100;
        public string StatusText { get; set; } = "";
        public bool Ok { get => Status >= 200 && Status <= 299; }
        public Dictionary<string, string> Headers { get; set; }
        public string Url { get; set; }

        private byte[] Data { get; }

        public string Text()
        {
            return Encoding.UTF8.GetString(Data);
        }

        public T Json<T>()
        {
            return JsonConvert.DeserializeObject<T>(Text());
        }

        public Dictionary<string, string> Json()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(Text());
        }

        public FetchResponse(byte[] data, int status, string statusText, string url)
        {
            Data = data;
            Status = status;
            StatusText = statusText;
            Url = url;
        }

        /*
         * Not implemented here:
         * public FetchBlob Blob() { }
         * public ArrayBuffer ArrayBuffer() { }
         * public FormData FormData() { }
         */
    }

    public class Request
    {
        private List<FetchRequest> fetchRequestsList = new List<FetchRequest>();

        private Promise<FetchResponse> InternalFetch(string url, FetchOptions options = null)
        {
            var newRequest = new FetchRequest();
            fetchRequestsList.Add(newRequest);
            
            newRequest.Fetch(url, options);

            return newRequest.Promise;
        }

        public static Request Instance { get; private set; } = new Request();

        public static Promise<FetchResponse> Fetch(string url, FetchOptions options = null)
        {
            return Instance.InternalFetch(url, options);
        }
    }

    public class FetchRequest
    {
        public WebClient WebClient { get; set; }
        public Promise<FetchResponse> Promise { get; set; }

        public FetchRequest()
        {
            WebClient = new WebClient();
            Promise = new Promise<FetchResponse>();
        }
        
        public void Fetch(string url, FetchOptions options = null)
        {
            options ??= new FetchOptions();
            WebClient.Headers = new WebHeaderCollection();

            foreach (var kvp in options.Headers)
                WebClient.Headers.Add(kvp.Key, kvp.Value);

            WebClient.UploadStringCompleted += (s, ev) => UploadDataCompleted(url, s, ev);
            WebClient.DownloadDataCompleted += (s, ev) => DownloadDataCompleted(url, s, ev);

            switch (options.Method)
            {
                case "POST":
                    WebClient.UploadStringAsync(new Uri(url), options.Body);
                    break;
                default:
                    WebClient.DownloadDataAsync(new Uri(url));
                    break;
            }
        }

        private void UploadDataCompleted(string url, object sender, EventArgs ev)
        {
            var uploadEv = (UploadStringCompletedEventArgs)ev;
            if (uploadEv.Error != null)
            {
                Logging.Log(uploadEv.Error.Message, Logging.Severity.Fatal);
                Promise.Reject(uploadEv.Error);
            }
            else
            {
                // TODO: Status codes
                var response = new FetchResponse(Encoding.UTF8.GetBytes(uploadEv.Result), 200, "OK", url);

                response.Headers = new Dictionary<string, string>();
                for (int i = 0; i < WebClient.ResponseHeaders?.Count; i++)
                    response.Headers.Add(WebClient.ResponseHeaders.GetKey(i), WebClient.ResponseHeaders.Get(i));

                Promise.Resolve(response);
            }
            
            WebClient.Dispose();
        }

        private void DownloadDataCompleted(string url, object sender, EventArgs ev)
        {
            var downloadEv = (DownloadDataCompletedEventArgs)ev;
            if (downloadEv.Error != null)
            {
                Logging.Log(downloadEv.Error.Message, Logging.Severity.Fatal);
                Promise.Reject(downloadEv.Error);
            }
            else
            {
                // TODO: Status codes
                var response = new FetchResponse(downloadEv.Result, 200, "OK", url);

                response.Headers = new Dictionary<string, string>();
                for (int i = 0; i < WebClient.ResponseHeaders?.Count; i++)
                    response.Headers.Add(WebClient.ResponseHeaders.GetKey(i), WebClient.ResponseHeaders.Get(i));

                Promise.Resolve(response);
            }
            
            WebClient.Dispose();
        }
    }
}