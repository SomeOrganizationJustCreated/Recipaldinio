using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
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

        /// <summary>
        /// <para>Stores the given <paramref name="recipes"/> to the Local storage.</para>
        /// </summary>
        /// <param name="recipes"><see cref="List{T}"/> of <see cref="Recipe"/> to store</param>
        /// <returns></returns>
        public async Task StoreValueAsync(List<Recipe> recipes)
        {

            //delete everything from protected local storage

            string? deserializedstringtocheckfornull = "";
            int somei = 0;

            //read all the storages
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
                    else
                    {
                        await _protectedLocalStorage.DeleteAsync(_storageKey + "-" + somei);
                    }

                    string? anotherdeserializedstringtocheckfornull = "";
                    int somej = 0;

                    //read all the base64 storages for currently read storage
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
                                await _protectedLocalStorage.DeleteAsync(_storageKey + "-" + somei + "_b64-" + somej);
                            }

                            somej++;
                        }
                        catch
                        {
                            anotherdeserializedstringtocheckfornull = null;
                        }
                    }

                    somei++;
                }
                catch
                {
                    deserializedstringtocheckfornull = null;
                }
            }

            //save everything to protected local storage

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

        /// <summary>
        /// Splits a String (<paramref name="str"/>) up into chunks of <paramref name="maxChunkSize"/>
        /// </summary>
        /// <param name="str">The string to split up</param>
        /// <param name="maxChunkSize">the size of the resulting strings</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="string"/></returns>
        private static IEnumerable<string> SplitString(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        /// <summary>
        /// <para>Retrieves all the Recipes from the Local storage.</para>
        /// </summary>
        /// <returns><see cref="List{T}"/> of <seealso cref="Recipe"/>s</returns>
        public async Task<List<Recipe>> RetrieveValueAsync()
        {
            string? deserializedstringtocheckfornull = "";
            List<Recipe> reclist = new();
            int somei = 0;

            //read all the storages
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

                    //read all the base64 storages for currently read storage
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

        /// <summary>
        /// <para>Adds the given <see cref="Recipe"/> <paramref name="recipe"/> to Local storage.</para>
        /// </summary>
        /// <param name="recipe">The recipe to add to storage</param>
        /// <returns></returns>
        public async Task AddRecipe(Recipe recipe)
        {
            //get list of recipes from broswer storage
            List<Recipe> retrievedList = await RetrieveValueAsync();
            //add recipe parameter to the list
            retrievedList.Add(recipe);
            //save new list to browser storage
            await StoreValueAsync(retrievedList);
        }

        /// <summary>
        /// <para>Removes the given <see cref="Recipe"/> <paramref name="recipe"/> from Local Storage.</para>
        /// </summary>
        /// <param name="recipe">The recipe to remove from storage</param>
        /// <returns></returns>
        public async Task DeleteRecipe(Recipe recipe)
        {
            //get list of recipes from broswer storage
            List<Recipe> retrievedList = await RetrieveValueAsync();
            //remove recipe from the list
            int recipetodeleteindex = 0;
            for (int i = 0; i < retrievedList.Count; i++)
            {
                if (retrievedList[i].Information.Name == recipe.Information.Name && retrievedList[i].Information.Description == recipe.Information.Description)
                {
                    recipetodeleteindex = i;
                }
            }

            retrievedList.RemoveAt(recipetodeleteindex);

            //save new list to browser storage
            await StoreValueAsync(retrievedList);
        }
    }
}
