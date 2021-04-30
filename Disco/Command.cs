using System.Threading.Tasks;

namespace Disco
{
    public abstract class Command : Singleton<Command>
    {
        public abstract string Icon { get; }
        public abstract string[] Aliases { get; }
        public abstract string Description { get; }
        public abstract string[] Syntax { get; }
        public abstract int MinArgs { get; }
        public abstract int MaxArgs { get; }
        public abstract void Run(CommandArgs commandArgs);
    }
}
