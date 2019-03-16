using CollectionManager.Annotations;
using LiveCharts;
using LiveVisualizer.Annotations;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LiveVisualizer
{
    public class VisualizerDataModel : IWpfVisualizerData
    {
        public IVisualizerDisplayData Display { get; set; } = new VisualizerDisplayData();
        public IVisualizerConfiguration Configuration { get; set; } = new VisualizerConfiguration();
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}