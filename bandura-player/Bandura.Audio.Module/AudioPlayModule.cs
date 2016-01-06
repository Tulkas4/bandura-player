using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Bandura.Audio.Module {


    public class AudioPlayModule : IDisposable {

        //for web audio location
        private WebResponse response;
        private Stream web_stream;
        private volatile int postion = -1;          //using for cache point
        //Audio moduls and streams
        private Mp3FileReader mp3_reader;
        private WaveStream wave_stream;
        private BlockAlignReductionStream bar_stream;


        protected readonly AutoResetEvent wait;


        internal const int BLOCK_SIZE = 32768;
        internal static readonly object locker = new object();

        protected bool IsCached { get; private set; }


        protected bool IsDisposed { get; private set; }


        protected WaveOut WaveOut { get; private set; }


        protected Stream Audio { get; private set; }


        protected int AverageBytesPerSecond { get; private set; }


        protected long BytePostion {
            get {
                return this.Audio.Position;
            }
            set {
                this.Audio.Position = value;
            }
        }


        public bool IsLocal { get; private set; }


        public string Location { get; private set; }


        public long Length { get; private set; }


        public AudioPlayModule( string location ) : this( location, new AutoResetEvent( true ) ) {

        }


        protected AudioPlayModule( string location, AutoResetEvent wait ) {
            try {
                if( string.IsNullOrWhiteSpace( location ) ) {
                    throw new NullReferenceException( "location is empty" );
                }
                if( this.IsCached = this.IsLocal = !location.StartsWith( "http" ) ) {
                    this.Audio = new FileStream( location, FileMode.Open, FileAccess.Read, FileShare.None );
                    this.Length = this.Audio.Length;
                }
                else {
                    this.Audio = new MemoryStream();
                }
                this.wait = wait;
                this.Location = location;
                this.StartCaching();
                //Init audio module
                this.mp3_reader = new Mp3FileReader( this.Audio );
                this.AverageBytesPerSecond = this.mp3_reader.Mp3WaveFormat.AverageBytesPerSecond;
                this.wave_stream = WaveFormatConversionStream.CreatePcmStream( this.mp3_reader );
                this.bar_stream = new BlockAlignReductionStream( this.wave_stream );
                this.WaveOut = new WaveOut( WaveCallbackInfo.FunctionCallback() );
                this.WaveOut.Init( this.bar_stream );
            }
            catch( Exception ex ) {
                this.Dispose();
                throw ex;
            }
        }


        public void Play() {
            this.CheckIfDisposed();
            this.WaveOut.Play();
            this.EndCaching();
        }


        protected virtual void StartCaching() {
            if( !this.IsCached ) {
                WebRequest request = WebRequest.Create( this.Location );
                this.response = request.GetResponse();
                this.CheckResonseLenght();
                this.Length = this.response.ContentLength;
                this.web_stream = this.response.GetResponseStream();
                byte[] buffer = new byte[BLOCK_SIZE];                               //2 ^ 15
                for( int i = 0; i < 5; i++ ) {
                    this.postion = web_stream.Read( buffer, 0, buffer.Length );
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
                while( (this.postion = this.web_stream.Read( buffer, 0, BLOCK_SIZE )) > 0 ) {
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
                if( this.web_stream != null ) {
                    this.web_stream.Close();
                    this.web_stream = null;
                }
                if( this.response != null ) {
                    this.response.Close();
                    this.response = null;
                }
            }
        }


        protected virtual void CheckResonseLenght() {
            if(this.response.ContentLength < 0) {
                throw new InvalidOperationException( "Incorrect response lenght" );
            }
        }


        protected void CheckIfDisposed() {
            if( this.IsDisposed ) {
                throw new ObjectDisposedException( "WaveOut" );
            }
        }


        protected virtual void OnDispose() {

        }


        public void Dispose() {
            if( !this.IsDisposed ) {
                //Dispose response (if exist)
                this.KillResponse();
                //Dispose audio module
                if( this.WaveOut != null ) {
                    this.WaveOut.Dispose();
                }
                if( this.bar_stream != null ) {
                    this.bar_stream.Close();
                }
                if( this.wave_stream != null ) {
                    this.wave_stream.Close();
                }
                if( this.mp3_reader != null ) {
                    this.mp3_reader.Close();
                }
                if(this.Audio != null) {
                    this.Audio.Close();
                }
                this.IsDisposed = true;
                this.OnDispose();
            }
        }
    }
}