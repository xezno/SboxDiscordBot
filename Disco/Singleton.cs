using System;

namespace Disco
{
    public class Singleton<T>
    {
        static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null) Create();
                return instance;
            }
        }

        public static void Create()
        {
            instance = Activator.CreateInstance<T>();
        }
    }
}
