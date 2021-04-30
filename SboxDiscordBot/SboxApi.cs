using System;
using System.Collections.Generic;
using System.Linq;
using RSG;

namespace SboxDiscordBot
{
    public partial class SboxApi
    {
        public static SboxApi Instance { get; } = new();

        public Promise<List<Category>> GetIndex()
        {
            var promise = new Promise<List<Category>>();
            // Endpoint: https://apix.facepunch.com/api/sbox/menu/index
            Request.Fetch("https://apix.facepunch.com/api/sbox/menu/index").Then(response =>
            {
                var index = response.Json<List<Category>>();
                promise.Resolve(index);
            }).Catch(exception => { promise.Reject(exception); });

            return promise;
        }

        public Promise<Org> GetOrg(string ident)
        {
            var promise = new Promise<Org>();
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

            void ResolveFindResult(FetchResponse response)
            {
                var findResult = response.Json<FindResult>();
                AddAssets(findResult.Assets);
            }

            // Programmer challenge: take a shot every time you see the word 'Then'
            Request.Fetch("http://apix.facepunch.com/api/sbox/asset/find?type=map")
                .Then(ResolveFindResult)
                .Then(() =>
                {
                    Request.Fetch("http://apix.facepunch.com/api/sbox/asset/find?type=gamemode")
                        .Then(ResolveFindResult)
                        .Then(() =>
                        {
                            Instance.GetIndex().Then(index =>
                                {
                                    foreach (var category in index)
                                        AddAssets(category.Packages);
                                }
                            ).Then(() =>
                            {
                                // Step 2: search for our org
                                if (orgDictionary.ContainsKey(ident))
                                {
                                    // Step 3: get org's first asset
                                    Instance.GetPackage($"{ident}.{orgDictionary[ident].First()}")
                                    .Then(asset =>
                                        {
                                            // Step 4: return title & description
                                            asset.Package.Org.PackageIdents = orgDictionary[ident].ToArray();
                                            promise.Resolve(asset.Package.Org);
                                        });
                                }
                                else
                                {
                                    promise.Reject(new Exception("Org not found"));
                                }
                            }).Catch(exception => promise.Reject(exception));
                        }).Catch(exception => promise.Reject(exception));
                }).Catch(exception => promise.Reject(exception));
            return promise;
        }

        public Promise<Asset> GetPackage(string ident)
        {
            var promise = new Promise<Asset>();
            // Endpoint: http://apix.facepunch.com/api/sbox/asset/get?id=(name)
            Request.Fetch($"http://apix.facepunch.com/api/sbox/asset/get?id={ident}").Then(response =>
            {
                var index = response.Json<Asset>();
                promise.Resolve(index);
            }).Catch(exception => { promise.Reject(exception); });

            return promise;
        }
    }
}