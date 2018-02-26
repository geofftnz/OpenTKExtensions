namespace OpenTKExtensions.Framework
{
    public interface ICompositeGameComponent
    {
        GameComponentCollection Components { get; }

        void Add(IGameComponent component);
        void Remove(IGameComponent component);
    }
}