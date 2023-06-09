namespace PhaseSync.Core.Entity
{
    /// <summary>
    /// An entity input envelope
    /// </summary>
    public abstract class EntityInputEnvelope<TMemory> : IEntityInput<TMemory>
    {
        private readonly Action<TMemory> apply;

        /// <summary>
        /// An entity input envelope
        /// </summary>
        public EntityInputEnvelope(Action<TMemory> apply)
        {
            this.apply = apply;
        }

        public void Apply(TMemory memory)
        {
            this.apply.Invoke(memory);
        }
    }
}
