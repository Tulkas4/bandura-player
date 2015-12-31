using System;
using System.Collections;
using System.Collections.Generic;

namespace Bandura.Audio {


    public sealed class Playlist<T> : IPlaylist<T> where T : Audio {


        private readonly List<T> playlist = new List<T>();


        public int Count {
            get {
                return playlist.Count;
            }
        }

        public bool Loop {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public bool AutoRepeat {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public void Order() {
            //throw new NotImplementedException();
        }


        public void Shuffle() {
            //throw new NotImplementedException();
        }


        IEnumerator IEnumerable.GetEnumerator() {
            return this.playlist.GetEnumerator();
        }


        public IEnumerator<T> GetEnumerator() {
            return playlist.GetEnumerator();
        }

        public Audio Next() {
            throw new NotImplementedException();
        }

        public Audio Previouse() {
            throw new NotImplementedException();
        }
    }
}
