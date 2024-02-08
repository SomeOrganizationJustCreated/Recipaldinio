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
            // Serialize the list of recipes to JSON
            RecipeList rspList = new RecipeList();
            rspList.recipelist = recipes;
            string serializedRecipes = JsonConvert.SerializeObject(rspList);

            // Store the JSON string in protected storage
            await _protectedLocalStorage.SetAsync(_storageKey, serializedRecipes);
        }

        public async Task<List<Recipe>> RetrieveValueAsync()
        {
            // Retrieve the JSON string from protected storage
            string serializedRecipes = (await _protectedLocalStorage.GetAsync<string>(_storageKey)).Value;

            if (string.IsNullOrEmpty(serializedRecipes))
            {
                // Return an empty list if no data is found in storage
                return new List<Recipe>();
            }

            //// Deserialize the JSON string back to a list of recipes
            //List<Recipe> recipes = JsonConvert.DeserializeObject<List<Recipe>>(serializedRecipes);
            RecipeList ret = JsonConvert.DeserializeObject<RecipeList>(serializedRecipes);
            return ret.recipelist;
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
    public class RecipeList
    {
        public List<Recipe> recipelist { get; set; }
    }
}
