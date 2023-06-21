using System.IO;
using Yaapii.Atoms;
using Yaapii.Atoms.IO;
using Yaapii.Atoms.Scalar;

namespace Test.PhaseSync.Datum.Datum
{
    /// <summary>
    /// A test datum from this assembly.
    /// </summary>
    public sealed class DatumOf : IInput
    {
        private readonly IScalar<Stream> data;

        /// <summary>
        /// A test datum from this assembly.
        /// </summary>
        public DatumOf(string name)
        {
            data =
                new ScalarOf<Stream>(() =>
                    new ResourceOf($"Datum/{name}", GetType().Assembly).Stream()
                );
        }

        public Stream Stream()
        {
            return data.Value();
        }
    }
}
