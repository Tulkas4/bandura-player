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
            string location = //@"C:\Users\amashko\Desktop\dc8af947b6d4c8.mp3";
                              //"http://cs7-1v4.vk-cdn.net/p17/a315d046d29503.mp3?extra=v7vYtg_GxqgjbGZIANK86RRLhCGLxnv6mpKPFIgggcL4IJFvUHXPQ2UnPG-vTAZ97gXzmb084Wklm0ZoX7BtBNoKTJhfl3eo,229";
                "http://cs7-5v4.vk-cdn.net/p5/dc8af947b6d4c8.mp3?extra=rmtHB9KnzsyZ_iLmkiP1T91RQwS555t6D2kEf-a_nD9q6Bv9FI_eDssHEMDo6Na9wYZldChMWgqPMe15uzOzlniWSLFceU1AXw,210";
            using( AudioModule a = new AudioModule( location ) ) {
                a.Play();
                //a.Play();
                //a.Play();
            }
            Console.ReadKey( true );
            //int read = 0;
            //WebResponse response;
            //Stream responseStream;
            //StartBuffering( location, out read, out response, out responseStream );
            //Thread t = new Thread( PlayAudio );
            //t.Start();
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
