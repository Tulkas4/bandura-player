using System.Collections.Generic;

namespace Bandura.Audio {


    public interface IPlaylist<T> : IEnumerable<T> where T : Audio {


        int Count {
            get;
        }


        bool Loop {
            get;
            set;
        }


        bool AutoRepeat {
            get;
            set;
        }



        void Shuffle();


        void Order();


        Audio Next();


        Audio Previouse();

    }
}
