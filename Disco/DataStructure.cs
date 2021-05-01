using System;
using System.IO;
using Newtonsoft.Json;

namespace Disco
{
    public interface DataStructureBase
    {
        void Load();
        void Save();
    }

    public abstract class DataStructure<TDataType, TSingletonType> : Singleton<TSingletonType>, DataStructureBase
    {
        private TDataType data;
        public abstract bool ReadOnly { get; }
        public abstract string Location { get; }

        public TDataType Data
        {
            get
            {
                if (data == null)
                    Load();

                return data;
            }
            set => data = value;
        }

        public void Load()
        {
            CreateDirectoryIfMissing();
            if (!File.Exists(Location))
            {
                Utils.Log($"No previous data found (file {Location} is missing); starting from scratch");
                Data = Activator.CreateInstance<TDataType>();
                return;
            }

            Utils.Log($"Loaded {Location}");

            using var sr = new StreamReader(Location);
            Data = JsonConvert.DeserializeObject<TDataType>(sr.ReadToEnd());
        }

        public void Save()
        {
            if (!ReadOnly)
            {
                CreateDirectoryIfMissing();

                using var sw = new StreamWriter(Location);
                sw.Write(JsonConvert.SerializeObject(Data));

                Utils.Log($"Saved {Location}");
            }
        }

        private void CreateDirectoryIfMissing()
        {
            var directoryName = Path.GetDirectoryName(Location);
            if (string.IsNullOrEmpty(directoryName))
                return;
            Directory.CreateDirectory(directoryName);
        }
    }
}