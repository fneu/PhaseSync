namespace PhaseSync.Core.Entity
{
    /// <summary>
    /// The entity, that can expose its memory and can be updated
    /// </summary>
    public interface IEntity<TMemory>
    {
        string ID();

        TMemory Memory();

        void Update(params IEntityInput<TMemory>[] inputs);

        void Update(IEnumerable<IEntityInput<TMemory>> inputs);
    }
}
