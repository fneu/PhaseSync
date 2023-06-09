using Yaapii.Atoms;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.Scalar;
using Yaapii.Atoms.Text;

namespace PhaseSync.Core.Entity
{
    /// <summary>
    /// A simple entity with id and memory, that can be updated
    /// </summary>
    public sealed class EntityOf<TMemory> : IEntity<TMemory>
    {
        private readonly IText id;
        private readonly IScalar<TMemory> memory;

        /// <summary>
        /// A simple entity with id and memory, that can be updated
        /// </summary>
        public EntityOf(string id, TMemory memory) : this(
            new TextOf(id),
            memory
        )
        { }

        /// <summary>
        /// A simple entity with id and memory, that can be updated
        /// </summary>
        public EntityOf(IText id, TMemory memory) : this(
            id,
            new ScalarOf<TMemory>(memory)
        )
        { }

        /// <summary>
        /// A simple entity with id and memory, that can be updated
        /// </summary>
        public EntityOf(string id, Func<TMemory> memory) : this(
            new TextOf(id),
            memory
        )
        { }

        /// <summary>
        /// A simple entity with id and memory, that can be updated
        /// </summary>
        public EntityOf(IText id, Func<TMemory> memory) : this(
            id,
            new Live<TMemory>(memory)
        )
        { }

        /// <summary>
        /// A simple entity with id and memory, that can be updated
        /// </summary>
        public EntityOf(string id, IScalar<TMemory> memory) : this(
            new TextOf(id),
            memory
        )
        { }

        /// <summary>
        /// A simple entity with id and memory, that can be updated
        /// </summary>
        public EntityOf(IText id, IScalar<TMemory> memory)
        {
            this.id = id;
            this.memory = memory;
        }

        public string ID()
        {
            return this.id.AsString();
        }

        public TMemory Memory()
        {
            return this.memory.Value();
        }

        public void Update(params IEntityInput<TMemory>[] inputs)
        {
            Update(new ManyOf<IEntityInput<TMemory>>(inputs));
        }

        public void Update(IEnumerable<IEntityInput<TMemory>> inputs)
        {
            foreach (var input in inputs)
            {
                input.Apply(this.memory.Value());
            }
        }
    }
}
