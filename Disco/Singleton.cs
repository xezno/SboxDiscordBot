using System;

namespace Disco
{
    public class Singleton<TSingletonType>
    {
        private static TSingletonType instance;

        public static TSingletonType Instance
        {
            get
            {
                if (instance == null) Create();
                return instance;
            }
        }

        public static void Create()
        {
            instance = Activator.CreateInstance<TSingletonType>();
        }
    }
}