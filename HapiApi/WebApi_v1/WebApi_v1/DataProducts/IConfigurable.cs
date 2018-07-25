using System;
using System.Collections.Generic;

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
        string SC { get; set; }
        string Level { get; set; }
        string RecordType { get; set; }
        DateTime TimeMin { get; set; }
        DateTime TimeMax { get; set; }
        List<string> Parameters { get; set; }
        bool IncludeHeader { get; set; }
        string Format { get; set; }
        Exception Error { get; set; }

        void Assign(Dictionary<string, string> dict);

        string ToString();
    }
}