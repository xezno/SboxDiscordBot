using Newtonsoft.Json;
using System;
using System.IO;

namespace Disco
{
    public interface DataStructureBase
    {
        void Load();
        void Save();
    }

    public abstract class DataStructure<Y, T> : Singleton<T>, DataStructureBase
    {
        public abstract bool ReadOnly { get; }
        public abstract string Location { get; }
        
        private Y data;

        public Y Data
        {
            get
            {
                if (data == null)
                    Load();

                return data;
            }
            set => data = value;
        }

        private void CreateDirectoryIfMissing()
        {
            var directoryName = Path.GetDirectoryName(Location);
            if (string.IsNullOrEmpty(directoryName))
                return;
            Directory.CreateDirectory(directoryName);
        }

        public void Load()
        {
            CreateDirectoryIfMissing();
            if (!File.Exists(Location))
            {
                Utils.Log($"No previous data found (file {Location} is missing); starting from scratch");
                Data = Activator.CreateInstance<Y>();
                return;
            }

            Utils.Log($"Loaded {Location}");

            using StreamReader sr = new StreamReader(Location);
            Data = JsonConvert.DeserializeObject<Y>(sr.ReadToEnd());
        }

        public void Save()
        {
            if (!ReadOnly)
            {
                CreateDirectoryIfMissing();

                using StreamWriter sw = new StreamWriter(Location);
                sw.Write(JsonConvert.SerializeObject(Data));

                Utils.Log($"Saved {Location}");
            }
        }
    }
}
