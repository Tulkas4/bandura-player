using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLibTesting {


    [Flags]
    public enum AudioState {
        Prapare = 0, 
        Play = 1,
        Pause = 2,
        Buffering = 3,
        Buffered = 4,
        Stop = 5
    }
}
