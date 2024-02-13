using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Recipaldinio.Code
{


    public class StorageHandler
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly string _storageKey = "Key";

        public StorageHandler(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage ?? throw new ArgumentNullException(nameof(protectedLocalStorage));
        }

        public async Task StoreValueAsync(List<Recipe> recipes)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                string b64str = recipes[i].Image64;
                recipes[i].Image64 = "";
                string serializedRecipe = JsonConvert.SerializeObject(recipes[i]);

                // Store the JSON string in protected storage
                await _protectedLocalStorage.SetAsync(_storageKey + "-" + i, serializedRecipe);

                List<string> b64strlist = SplitString(b64str, 5000).ToList();
                for (int j = 0; j < b64strlist.Count; j++)
                {
                    await _protectedLocalStorage.SetAsync(_storageKey + "-" + i + "_b64-" + j, b64strlist[j]);
                }
            }
        }

        static IEnumerable<string> SplitString(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }


        public async Task<List<Recipe>> RetrieveValueAsync()
        {
            string? deserializedstringtocheckfornull = "";
            List<Recipe> reclist = new();
            int somei = 0;
            while (deserializedstringtocheckfornull != null)
            {
                try
                {
                    // Retrieve the JSON string from protected storage
                    string serializedRecipes =
                        (await _protectedLocalStorage.GetAsync<string>(_storageKey + "-" + somei)).Value;

                    if (string.IsNullOrEmpty(serializedRecipes))
                    {
                        // Return an empty list if no data is found in storage
                        deserializedstringtocheckfornull = null;
                    }

                    //// Deserialize the JSON string back to a list of recipes
                    Recipe rec = JsonConvert.DeserializeObject<Recipe>(serializedRecipes);
                    Console.WriteLine(serializedRecipes);

                    string? anotherdeserializedstringtocheckfornull = "";
                    int somej = 0;

                    rec.Image64 = "";

                    while (anotherdeserializedstringtocheckfornull != null)
                    {
                        try
                        {
                            string? serializedString = (await _protectedLocalStorage.GetAsync<string>(_storageKey + "-" + somei + "_b64-" + somej)).Value;

                            if (string.IsNullOrEmpty(serializedString))
                            {
                                anotherdeserializedstringtocheckfornull = null;
                            }
                            else
                            {
                                rec.Image64 += serializedString.Replace("\"", "");
                            }

                            somej++;
                        }
                        catch
                        {
                            anotherdeserializedstringtocheckfornull = null;
                        }
                    }

                    reclist.Add(rec);

                    somei++;
                }
                catch
                {
                    deserializedstringtocheckfornull = null;
                }
            }

            return reclist;
        }

        public async Task AddRecipe(Recipe recipe)
        {
            //get list of recipes from broswer storage
            List<Recipe> retrievedList = await RetrieveValueAsync();
            //add recipe parameter to the list
            retrievedList.Add(recipe);
            //save new list to browser storage
            await StoreValueAsync(retrievedList);
        }
    }
}
