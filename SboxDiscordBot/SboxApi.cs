using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SboxDiscordBot
{
    public partial class SboxApi
    {
        public static SboxApi Instance { get; } = new();

        public Task<List<Category>> GetIndex()
        {
            var taskCompletionSource = new TaskCompletionSource<List<Category>>();
            // Endpoint: https://apix.facepunch.com/api/sbox/menu/index
            Request.Fetch("https://apix.facepunch.com/api/sbox/menu/index").ContinueWith(task =>
            {
                if (task.Exception != null)
                    taskCompletionSource.TrySetException(new Exception("Couldn't get index"));
                
                var response = task.Result;
                var index = response.Json<List<Category>>();
                taskCompletionSource.TrySetResult(index);
            });

            return taskCompletionSource.Task;
        }

        public Task<Org> GetOrg(string ident)
        {
            ident = ident.ToLower();

            var taskCompletionSource = new TaskCompletionSource<Org>();
            /* Right now, this is a big bodge / hack
             * Here's what we do:
             * 1. Visit several endpoints (/asset/find?type=map|gamemode, /menu/index) and collect a list of orgs with
             *    their gamemode idents
             * 2. Search that list for an org that matches the ident we're looking for
             * 3. Visit /asset/get?id=(org).(ident)
             * 4. Get the title & description for the org from this
             *
             * It's definitely not good and won't work for every org, but it's the best we have atm since there's no
             * API endpoint that lists orgs.
             */

            // Step 1: collect orgs
            var orgDictionary = new Dictionary<string, List<string>>();

            void AddAssets(Package[] packages)
            {
                foreach (var asset in packages)
                    if (orgDictionary.ContainsKey(asset.Org.Ident))
                    {
                        var assetIdents = orgDictionary[asset.Org.Ident];
                        if (!assetIdents.Contains(asset.Ident))
                            assetIdents.Add(asset.Ident);
                        
                        orgDictionary[asset.Org.Ident] = assetIdents;
                    }
                    else
                    {
                        orgDictionary.Add(asset.Org.Ident, new List<string> {asset.Ident});
                    }
            }

            void ResolveFindResult(Task<FetchResponse> task)
            {
                if (task.Exception != null)
                    taskCompletionSource.TrySetException(new Exception("Couldn't resolve find result"));
                
                var response = task.Result;
                var findResult = response.Json<FindResult>();
                AddAssets(findResult.Assets);
            }

            // Programmer challenge: take a shot every time you see the word 'Then'
            Request.Fetch("http://apix.facepunch.com/api/sbox/asset/find?type=map")
                .ContinueWith(ResolveFindResult)
                .ContinueWith(_ => 
                    Request.Fetch("http://apix.facepunch.com/api/sbox/asset/find?type=gamemode")
                        .ContinueWith(ResolveFindResult))
                .ContinueWith(_ => Instance.GetIndex())
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                        taskCompletionSource.TrySetException(new Exception("Couldn't resolve index"));
                    
                    var index = task.Result;
                    foreach (var category in index.Result)
                    {
                        AddAssets(category.Packages);
                    }
                })
                .ContinueWith(_ =>
                {
                    // Step 2: search for our org
                    if (orgDictionary.ContainsKey(ident))
                    {
                        // Step 3: get org's first asset
                        Instance.GetPackage($"{ident}.{orgDictionary[ident].First()}")
                            .ContinueWith(task =>
                            {
                                if (task.Exception != null)
                                    taskCompletionSource.TrySetException(new Exception("Org not found"));
                                
                                // Step 4: return title & description
                                var asset = task.Result;
                                asset.Package.Org.PackageIdents = orgDictionary[ident].ToArray();
                                taskCompletionSource.TrySetResult(asset.Package.Org);
                            });
                    }
                    else
                    {
                        taskCompletionSource.TrySetException(new Exception("Org not found"));
                    }
                });
            return taskCompletionSource.Task;
        }

        public Task<Asset> GetPackage(string ident)
        {
            var taskCompletionSource = new TaskCompletionSource<Asset>();
            // Endpoint: http://apix.facepunch.com/api/sbox/asset/get?id=(name)
            Request.Fetch($"http://apix.facepunch.com/api/sbox/asset/get?id={ident}").ContinueWith(task =>
            {
                if (task.Exception != null)
                    taskCompletionSource.TrySetException(new Exception("No such package exists."));
                
                var response = task.Result;
                var index = response.Json<Asset>();
                taskCompletionSource.TrySetResult(index);
            });

            return taskCompletionSource.Task;
        }
    }
}