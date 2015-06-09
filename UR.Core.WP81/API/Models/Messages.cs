using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UR.Core.WP81.API.Models
{
    public class DataHandlerStatusChanged
    {
        public bool IsStarted { get; private set; }

        public DataHandlerStatusChanged(bool isStarted)
        {
            IsStarted = isStarted;
        }
    }
}
