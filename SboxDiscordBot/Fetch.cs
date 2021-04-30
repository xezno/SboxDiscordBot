using Newtonsoft.Json;
using RSG;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
        public static Promise<FetchResponse> Fetch(string url, FetchOptions options = null)
        {
            var promise = new Promise<FetchResponse>();
            options ??= new FetchOptions();

            using (var client = new WebClient())
            {
                client.Headers = new WebHeaderCollection();

                foreach (var kvp in options.Headers)
                    client.Headers.Add(kvp.Key, kvp.Value);

                client.UploadStringCompleted += (s, ev) => UploadDataCompleted(url, promise, s, ev, client);
                client.DownloadDataCompleted += (s, ev) => DownloadDataCompleted(url, promise, s, ev, client);

                switch (options.Method)
                {
                    case "POST":
                        client.UploadStringAsync(new Uri(url), options.Body);
                        break;
                    default:
                        client.DownloadDataAsync(new Uri(url));
                        break;
                }
            }

            return promise;
        }

        private static void UploadDataCompleted(string url, Promise<FetchResponse> promise, object sender, EventArgs _ev, WebClient client)
        {
            var ev = (UploadStringCompletedEventArgs)_ev;
            if (ev.Error != null)
            {
                Logging.Log(ev.Error.Message, Logging.Severity.Fatal);
                promise.Reject(ev.Error);
            }
            else
            {
                // TODO: Status codes
                var response = new FetchResponse(Encoding.UTF8.GetBytes(ev.Result), 200, "OK", url);

                response.Headers = new Dictionary<string, string>();
                for (int i = 0; i < client.ResponseHeaders.Count; i++)
                    response.Headers.Add(client.ResponseHeaders.GetKey(i), client.ResponseHeaders.Get(i));

                promise.Resolve(response);
            }
        }

        private static void DownloadDataCompleted(string url, Promise<FetchResponse> promise, object sender, EventArgs _ev, WebClient client)
        {
            var ev = (DownloadDataCompletedEventArgs)_ev;
            if (ev.Error != null)
            {
                Logging.Log(ev.Error.Message, Logging.Severity.Fatal);
                promise.Reject(ev.Error);
            }
            else
            {
                // TODO: Status codes
                var response = new FetchResponse(ev.Result, 200, "OK", url);

                response.Headers = new Dictionary<string, string>();
                for (int i = 0; i < client.ResponseHeaders.Count; i++)
                    response.Headers.Add(client.ResponseHeaders.GetKey(i), client.ResponseHeaders.Get(i));

                promise.Resolve(response);
            }
        }
    }
}