using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RepCounterBand
{
    public static class RepCounterHelper
    {
        static RepCounterHelper()
        {
            ExerciseXDocument = new XDocument();
        }

        public static int GlobalCount { get; set; }

        public static XDocument ExerciseXDocument { get; set; }
    }
}