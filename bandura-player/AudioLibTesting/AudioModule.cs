using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace AudioLibTesting {


    internal class AudioPlayModule : IDisposable {

        //for web audio location
        private WebResponse response;
        private Stream webStream;
        private volatile int postion = -1;


        protected readonly AutoResetEvent wait;


        internal const int BLOCK_SIZE = 32768;
        internal static readonly object locker = new object();


        protected bool IsCached { get; private set; }


        protected bool IsDisposed { get; private set; }


        protected Stream Audio { get; private set; }


        public bool IsLocal { get; private set; }


        public string Location { get; private set; }


        public AudioPlayModule( string location ) : this( location, new AutoResetEvent( true ) ) {

        }


        protected AudioPlayModule( string location, AutoResetEvent wait ) {
            Uri result = null;
            if( this.IsCached = this.IsLocal = !Uri.TryCreate( location, UriKind.Absolute, out result ) ) {
                this.Audio = new FileStream( location, FileMode.Open, FileAccess.Read, FileShare.None );
            }
            else {
                this.Audio = new MemoryStream();
            }
            this.wait = wait;
            this.Location = location;
        }



        public void Play() {
            if(this.IsDisposed) {
                throw new ObjectDisposedException( "Audio" );
            }
            this.PlayAudio();
        }


        protected virtual void PlayAudio() {
            //start caching
            this.StartCaching();
            long fsize = this.response.ContentLength;
            Id3Lib.TagHandler tag = null;
            if( (tag = Id3Lib.TagHandler.TryCreate( this.Audio )) != null ) {
                Console.WriteLine( "Artist - {0}\nSong = {1}\nGenre = {2}", tag.Artist, tag.Song, tag.Genre );
            }
            using( var rdr = new Mp3FileReader( this.Audio ) ) {
                int abps = rdr.Mp3WaveFormat.AverageBytesPerSecond;
                double totalSeconds = fsize / abps;
                TimeSpan totalTime = TimeSpan.FromSeconds( Math.Round( totalSeconds, MidpointRounding.AwayFromZero ) );
                Console.WriteLine( "total time = {0}", totalTime.ToString() );
                using( var wavStream = WaveFormatConversionStream.CreatePcmStream( rdr ) ) {
                    using( var baStream = new BlockAlignReductionStream( wavStream ) ) {
                        using( var waveOut = new WaveOut( WaveCallbackInfo.FunctionCallback() ) ) {
                            waveOut.Init( baStream );
                            waveOut.PlaybackStopped += ( o, e ) => {
                                this.wait.Set();
                            };
                            waveOut.Play();
                            //cache sound here
                            new Thread( this.EndCaching ).Start();
                            while( waveOut.PlaybackState == PlaybackState.Playing ) {
                                double pos = this.Audio.Position / abps;
                                TimeSpan position = TimeSpan.FromSeconds( Math.Round( pos, MidpointRounding.AwayFromZero ) );
                                Console.Write( "\rcurrent position = {0}", position.ToString() );
                                Thread.Sleep( 100 );
                            }
                            //this.wait.Reset();
                            //this.wait.WaitOne();
                        }
                    }
                }
            }
        }


        protected virtual void StartCaching() {
            if( !this.IsCached ) {
                WebRequest request = WebRequest.Create( this.Location );
                response = request.GetResponse();
                webStream = response.GetResponseStream();
                byte[] buffer = new byte[32768];                        //2 ^ 15
                for( int i = 0; i < 25; i++ ) {
                    this.postion = webStream.Read( buffer, 0, buffer.Length );
                    this.Audio.Write( buffer, 0, this.postion );
                    if( 0.Equals( this.postion ) ) {
                        break;
                    }
                }
                this.Audio.Position = 0;
            }
        }


        protected virtual void EndCaching() {
            if( !this.IsCached ) {
                byte[] buffer = new byte[BLOCK_SIZE];
                while( (this.postion = this.webStream.Read( buffer, 0, BLOCK_SIZE )) > 0 ) {
                    lock ( locker ) {
                        long postion = this.Audio.Position;
                        this.Audio.Position = this.Audio.Length;
                        this.Audio.Write( buffer, 0, this.postion );
                        this.Audio.Position = postion;
                    }
                }
                this.KillResponse();
                this.postion = -1;
                this.IsCached = true;
            }
        }


        private void KillResponse() {
            if( !this.IsLocal ) {
                if( this.webStream != null ) {
                    this.webStream.Close();
                    this.webStream = null;
                }
                if( this.response != null ) {
                    this.response.Close();
                    this.response = null;
                }
            }
        }


        public void Dispose() {
            if(!this.IsDisposed) {
                this.KillResponse();
                this.Audio.Close();
                //this.wave.Dispose();
                this.IsDisposed = true;
            }
        }
    }
}
