using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSMHI.FuncApi.Models
{
    public partial class Forecast
    {
        public DateTimeOffset ApprovedTime { get; set; }
        public DateTimeOffset ReferenceTime { get; set; }
        public Geometry Geometry { get; set; }
        public List<TimeSery> TimeSeries { get; set; }
    }

    public partial class Geometry
    {
        public string Type { get; set; }
        public List<List<double>> Coordinates { get; set; }
    }

    public partial class TimeSery
    {
        public DateTimeOffset ValidTime { get; set; }
        public List<Parameter> Parameters { get; set; }
    }

    public partial class Parameter
    {
        public string Name { get; set; }
        public string LevelType { get; set; }
        public long Level { get; set; }
        public string Unit { get; set; }
        public List<double> Values { get; set; }
    }
}

