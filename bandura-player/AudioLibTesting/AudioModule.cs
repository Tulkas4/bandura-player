using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace AudioLibTesting {


    internal sealed class AudioModule {

        private static readonly object locker = new object();

        private readonly WaveOut wave = new WaveOut( WaveCallbackInfo.FunctionCallback() );
        private readonly MemoryStream buffer = new MemoryStream();
        private WebResponse response;
        private Stream webStream;
        private volatile int read = 1;
        private int lastPosition = 0;
        private volatile bool playBuffered = false;


        private const int BLOCK_SIZE = 32768;



        public AudioModule() {
            //this.buffer.Flush();
        }



        public void Play( string url ) {
            if( !this.playBuffered ) {
                WebRequest request = WebRequest.Create( url );
                response = request.GetResponse();
                webStream = response.GetResponseStream();
                byte[] buffer = new byte[32768];                        //2 ^ 15
                for( int i = 0; i < 5; i++ ) {
                    this.read = webStream.Read( buffer, 0, buffer.Length );
                    this.buffer.Write( buffer, 0, read );
                    if(read == 0) {
                        break;
                    }
                }
                this.buffer.Position = 0;
            }
            new Thread( PlayAudio ).Start();
            //if( !this.playBuffered ) {
                this.EndBuffefing();
            //}
        }


        private void EndBuffefing() {
            byte[] buffer = new byte[BLOCK_SIZE];
            while( (this.read = this.webStream.Read( buffer, 0, BLOCK_SIZE )) > 0 ) {
                lock ( locker ) {
                    long postion = this.buffer.Position;
                    this.buffer.Position = this.buffer.Length;
                    this.buffer.Write( buffer, 0, this.read );
                    this.buffer.Position = postion;
                }
            }
            this.webStream.Close();
            response.Close();
        }


        private void PlayAudio() {
            //int lenght = 327680;
            //position = this.buffer.Length > position + lenght ? position : position;
            //byte[] buffer = new byte[lenght];
            //Array.Copy( this.buffer.GetBuffer(), position, buffer, 0, lenght );
            //using( MemoryStream ms = new MemoryStream( buffer ) ) {
            using( var rdr = new Mp3FileReader( this.buffer ) ) {
                using( var wavStream = WaveFormatConversionStream.CreatePcmStream( rdr ) ) {
                    using( var baStream = new BlockAlignReductionStream( wavStream ) ) {
                        using( var waveOut = new WaveOut( WaveCallbackInfo.FunctionCallback() ) ) {
                            waveOut.Init( baStream );
                            waveOut.Play();
                            //waveOut.PlaybackStopped += ( o, e ) => {
                                
                            //};
                            while( waveOut.PlaybackState == PlaybackState.Playing ) {
                                Thread.Sleep( 100 );
                                //TODO: expand memory stream here

                            }
                            //if( !this.playBuffered ) {
                            //    long position = stream.Length;
                            //    byte[] bf = new byte[BLOCK_SIZE]; //this.buffer.GetBuffer();
                            //    this.buffer.Position = position;
                            //    StreamWriter sw = new StreamWriter( stream );
                            //    sw.Write( bf );
                            //    //BufferedStream bs = new BufferedStream( stream );
                            //    int read = this.buffer.Read( bf, 0, BLOCK_SIZE );
                            //    bs.Write( bf, 0, read );
                            //    this.PlayAudio( bs );
                            //}
                        }
                    }
                }
            }
            //}
        }
    }
}
