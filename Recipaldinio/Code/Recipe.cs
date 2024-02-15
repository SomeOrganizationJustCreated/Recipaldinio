using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Recipaldinio.Code
{
    public class Recipe
    {
        public string? Image64 { get; set; }
        public RecipeUnits Units { get; set; } = new RecipeUnits();
        public List<string> Ingredients { get; set; } = new();
        public RecipeInformation Information { get; set; } = new RecipeInformation();
        public List<string> Steps { get; set; } = new();
    }

    public class RecipeInformation
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Calories { get; set; } = 0;
        public int Fats { get; set; } = 0;
        public int Proteins { get; set; } = 0;
        public int Carbohydrates { get; set; } = 0;
        public bool? IsVegan { get; set; } = false;
        public bool? ContainsPig { get; set; } = false;
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
