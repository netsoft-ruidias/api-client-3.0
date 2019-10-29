namespace Netsoft.Core.Model
{
    using System.Collections.Generic;

    public class ApiResponseHeader
    {
        public string Name { get; set; }

        public IEnumerable<string> Values { get; set; }
    }
}
