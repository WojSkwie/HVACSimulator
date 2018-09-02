using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterConfigViewModel
    {
        public List<string> SelectableOutputs { get; set; }
        public string SelectedOutput { get; set; }
        private AdapterManager AdapterManager;

        public AdapterConfigViewModel(AdapterManager adapterManager)
        {
            AdapterManager = adapterManager;
            SelectableOutputs = new List<string>
            {
                "Wilgotność w pomieszczeniu",
                "Temperatura na wylocie wymiennika"
            };
        }
    }
}
