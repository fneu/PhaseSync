namespace PhaseSync.Core.Entity
{
    /// <summary>
    /// An entity envelope
    /// </summary>
    public abstract class EntityEnvelope<TMemory> : IEntity<TMemory>
    {
        private readonly IEntity<TMemory> origin;

        /// <summary>
        /// An entity envelope
        /// </summary>
        public EntityEnvelope(IEntity<TMemory> origin)
        {
            this.origin = origin;
        }

        public string ID()
        {
            return this.origin.ID();
        }

        public TMemory Memory()
        {
            return this.origin.Memory();
        }

        public void Update(params IEntityInput<TMemory>[] inputs)
        {
            this.origin.Update(inputs);
        }

        public void Update(IEnumerable<IEntityInput<TMemory>> inputs)
        {
            this.origin.Update(inputs);
        }
    }
}
