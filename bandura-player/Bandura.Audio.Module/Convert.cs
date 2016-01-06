using System;

namespace Bandura.Audio.Module {


    public static class Convert {

        public static TimeSpan ToTime( int average_bytes_per_second, long length ) {
            double totalSeconds = length / average_bytes_per_second;
            totalSeconds = Math.Round( totalSeconds, MidpointRounding.AwayFromZero );
            return TimeSpan.FromSeconds( totalSeconds );
        }


        public static long ToBytePosition( TimeSpan time, int average_bytes_per_second ) {
            return (long)Math.Round( time.TotalSeconds, MidpointRounding.AwayFromZero ) * average_bytes_per_second;
        }
    }
}
