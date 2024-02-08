namespace Recipaldinio.Code
{
    public class Recipe
    {
        public RecipeUnits Units { get; set; } = new RecipeUnits();
        public List<string> Ingredients { get; set; }
        public RecipeInformation Information { get; set; } = new RecipeInformation();
        public List<string> Steps { get; set; }
    }

    public class RecipeInformation
    {
        public string Name { get; set; }
        public int Calories { get; set; }
        public int Fats { get; set; }
        public int Proteins { get; set; }
        public int Carbohydrates { get; set; }
        public bool IsVegan { get; set; }
        public bool ContainsPig { get; set; }
    }

    public class RecipeUnits
    {
        public string Litres { get; set; } = "l";
        public string MilliLitres { get; set; } = "ml";
        public string Gramms { get; set; } = "g";
        public RecipeInformationUnits InformationUnits { get; set; } = new RecipeInformationUnits();
    }

    public class RecipeInformationUnits
    {
        public string Calories { get; set; } = "kcal";
        public string Fats { get; set; } = "g";
        public string Proteins { get; set; } = "g";
        public string Carbohydrates { get; set; } = "g";
    }
}
