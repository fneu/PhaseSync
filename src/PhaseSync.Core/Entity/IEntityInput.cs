namespace PhaseSync.Core.Entity
{
    /// <summary>
    /// Entity input to apply to the memory
    /// </summary>
    public interface IEntityInput<TMemory>
    {
        void Apply(TMemory memory);
    }
}
