using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using System.IO;
using NAudio.Wave;
using System.Threading;
using System.Net;

namespace AudioLibTesting {


    class Program {


        private static MemoryStream streamBuffer = new MemoryStream();


        static void Main( string[] args ) {
            string location = "http://cs7-5v4.vk-cdn.net/p8/46090c141cdf7e.mp3?extra=6SkHWYB4TCwOFOX_oFZ2FeA8ksHlx37p6TJYsEQCgcKfrNDiPuv9STgbrT_e0PzGEIsBQTg2X3e9a4dkhoi_L5UySnz0aIyU";
            int read = 0;
            WebResponse response;
            Stream responseStream;
            StartBuffering( location, out read, out response, out responseStream );
            Thread t = new Thread( PlayAudio );
            t.Start();
            //EndBuffering( read, response, responseStream );
        }



        private static void StartBuffering( string location, out int readPosition, out WebResponse response, out Stream streamResponse ) {
            streamBuffer.Flush();
            WebRequest request = WebRequest.Create( location );
            response = request.GetResponse();
            streamResponse = response.GetResponseStream();
            byte[] buffer = new byte[32768];                        //2 ^ 15
            readPosition = 1;
            for( int i = 0; i < 10 && readPosition > 0; i++ ) {
                readPosition = streamResponse.Read( buffer, 0, buffer.Length );
                streamBuffer.Write( buffer, 0, readPosition );
            }
            streamBuffer.Position = 0;
        }


        private static void EndBuffering( int readPosition, WebResponse response, Stream streamResponse ) {
            byte[] buffer = new byte[32768];
            while( (readPosition = streamResponse.Read( buffer, 0, buffer.Length )) > 0 ) {
                streamBuffer.Write( buffer, 0, readPosition );
            }
            streamResponse.Close();
            response.Close();
        }



        //public static void PlayMp3FromUrl( string url ) {
        //    WebRequest request = WebRequest.Create( url );
        //    using( WebResponse response = request.GetResponse() ) {
        //        using( Stream stream = response.GetResponseStream() ) {
        //            MemoryStream ms = new MemoryStream();

        //            byte[] buffer = new byte[32768];                        //2 ^ 15
        //            int lastPosition = 0;
        //            int read = 1;
        //            for( int i = 0; i < 5 && read > 0; i++ ) {
        //                read = stream.Read( buffer, 0, buffer.Length );
        //                ms.Write( buffer, 0, read );
        //            }
        //            ms.Position = lastPosition;
        //            PlayAudio( ms );
        //            ms.Dispose();
        //        }
        //    }

        //}



        






        public static void PlayAudio( ) {
            using( var rdr = new Mp3FileReader( streamBuffer ) ) {
                using( var wavStream = WaveFormatConversionStream.CreatePcmStream( rdr ) ) {
                    using( var baStream = new BlockAlignReductionStream( wavStream ) ) {
                        using( var waveOut = new WaveOut( WaveCallbackInfo.FunctionCallback() ) ) {
                            waveOut.Init( baStream );
                            waveOut.Play();
                            while( waveOut.PlaybackState == PlaybackState.Playing ) {
                                Thread.Sleep( 100 );
                            }
                        }
                    }
                }
            }
        }
    }
}
