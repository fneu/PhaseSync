namespace PhaseSync.Blazor.Models
{
    public class UIStep
    {
        public string description { get; set; }
        public IEnumerable<string> subSteps { get; set; }

        public UIStep(string description, IEnumerable<string> subSteps)
        {
            this.description = description;
            this.subSteps = subSteps;
        }

    }
}
