using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_v1.DataProducts
{
    public interface IConfigurable
    {
        bool Configure();
    }

    public interface IProperties
    {
        string RequestType { get; set; }
        string Id { get; set; }
        DateTime TimeMin { get; set; }
        DateTime TimeMax { get; set; }
        List<string> Parameters { get; set; }
        bool IncludeHeader { get; set; }
        Exception Error { get; set; }
        void Assign(Dictionary<string, string> dict);
        string ToString();
    }
}
