#nullable disable

namespace Mint.Database.Local
{
    using System.Collections.Generic;

    public class TypeNode
    {
        // asgvm-b93

        public string TypeKey { get; set; }

        public List<TypeNode> Children { get; set; }
    }
}
