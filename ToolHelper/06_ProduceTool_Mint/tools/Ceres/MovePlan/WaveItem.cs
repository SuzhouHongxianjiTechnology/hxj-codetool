#nullable disable

namespace Ceres
{
    using System.Collections.Generic;

    public class WaveItem
    {
        public int Wave { get; set; }

        public List<string> Types { get; set; }

        public WaveItem() { }

        public WaveItem(int wave, List<string> types)
        {
            this.Wave = wave;
            this.Types = types;
        }
    }
}
