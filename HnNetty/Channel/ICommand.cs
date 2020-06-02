using DotNetty.Transport.Channels;

namespace HnNetty.Channel
{
    public interface ICommand
    {
        string Name { get; }
    }

    public interface ICommand<TKey> : ICommand
    {
        TKey Key { get; }
    }

    public interface ICommand<TKey, TChannel, TPackageInfo> : ICommand<TKey>
    {
        void Execute(TChannel session, TPackageInfo package);
    }

    public abstract class CommandBase : ICommand<int, IChannel, Message>
    {
        public virtual void Execute(IChannel session, Message package)
        {
        }
        public virtual string Name { get; }

        public abstract int Key { get; }
    }
}