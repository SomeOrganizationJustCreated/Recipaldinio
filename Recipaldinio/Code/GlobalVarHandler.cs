using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace Recipaldinio.Code
{
    public class GlobalVarHandler
    {
        private Config? conf;
        private const string path = "config.json";
        public bool DebugEnabled
        {
            get => conf.DebugEnabled;
            set => SetDebug(value);
        }

        public GlobalVarHandler()
        {
            bool FinishedLoading = false;
            while (!FinishedLoading)
            {
                FinishedLoading = LoadConfig();
            }
        }

        public async Task<bool> SetDebugAsync(bool TheBool)
        {
            conf.DebugEnabled = TheBool;
            return await SaveConfigAsync();
        }

        public bool SetDebug(bool TheBool)
        {
            conf.DebugEnabled = TheBool;
            return SaveConfig();
        }

        public async Task<bool> LoadConfigAsync()
        {
            string ReadTest = await File.ReadAllTextAsync(path);
            conf = JsonConvert.DeserializeObject<Config>(ReadTest);
            return conf != null;
        }

        public bool LoadConfig()
        {
            string ReadTest = File.ReadAllText(path);
            conf = JsonConvert.DeserializeObject<Config>(ReadTest);
            return conf != null;
        }

        public async Task<bool> SaveConfigAsync()
        {
            string? config = JsonConvert.SerializeObject(conf);
            await File.WriteAllTextAsync(path, config);
            return !string.IsNullOrEmpty(config);
        }

        public bool SaveConfig()
        {
            string config = JsonConvert.SerializeObject(conf);
            File.WriteAllText(path, config);
            return !string.IsNullOrEmpty(config);
        }
    }

    public class Config
    {
        public bool DebugEnabled { get; set; }
    }
}
