namespace PhaseSync.Core.Zones
{
    internal class ZoneOf : IZone
    {
        private readonly double min;
        private readonly double max;

        public ZoneOf(double min, double max) {
            if (min > max)
            {
                throw new ArgumentException("Zone min exceeds max!");
            }

            this.min = min;
            this.max = max;
        }

        public double Min()
        {
            return this.min;
        }

        public double Max()
        {
            return this.max;
        }
    }
}
