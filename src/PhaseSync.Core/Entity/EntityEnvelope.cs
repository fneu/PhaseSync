using Yaapii.Atoms;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity
{
    /// <summary>
    /// An entity envelope
    /// </summary>
    public abstract class EntityEnvelope<TMemory> : IEntity<TMemory>
    {
        private readonly IScalar<IEntity<TMemory>> origin;

        /// <summary>
        /// An entity envelope
        /// </summary>
        public EntityEnvelope(IEntity<TMemory> origin) : this(
            new ScalarOf<IEntity<TMemory>>(origin))
        { }

        /// <summary>
        /// An entity envelope
        /// </summary>
        public EntityEnvelope(Func<IEntity<TMemory>> origin) : this(
            new ScalarOf<IEntity<TMemory>>(origin))
        { }

        /// <summary>
        /// An entity envelope
        /// </summary>
        public EntityEnvelope(IScalar<IEntity<TMemory>> origin)
        {
            this.origin = origin;
        }

        public string ID()
        {
            return this.origin.Value().ID();
        }

        public TMemory Memory()
        {
            return this.origin.Value().Memory();
        }

        public void Update(params IEntityInput<TMemory>[] inputs)
        {
            this.origin.Value().Update(inputs);
        }

        public void Update(IEnumerable<IEntityInput<TMemory>> inputs)
        {
            this.origin.Value().Update(inputs);
        }
    }
}
