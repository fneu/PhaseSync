using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.PhasedTarget.Input;
using System.Text.Json.Nodes;
using Xive;

namespace PhaseSync.Core.Outgoing.Polar
{
    public sealed class DeleteTarget : IRequest
    {
        private readonly IHive userHive;
        private readonly IEntity<IHoneyComb> target;

        public DeleteTarget(IHive userHive, IEntity<IHoneyComb> target)
        {
            this.userHive = userHive;
            this.target = target;
        }

        public async Task<IResult> Send(HttpClient client)
        {
            if (new PolarID.Has(target).Value())
            {
                await client.DeleteAsync($"/training/target/{new PolarID.Of(target).Value()}");
            }

            this.userHive.Catalog().Remove(target.Memory().Name().Substring(userHive.Scope().Length + 1));

            return new ResultOf(
                true,
                "",
                new JsonObject()
            );
        }
    }
}
