using System;

namespace Bandura.Audio.Module {


    public class AudioControlModule : AudioPlayModule, IPlayer {


        public TimeSpan TotalTime { get; private set; }


        public TimeSpan CachedTime {
            get {
                return Convert.ToTime( this.AverageBytesPerSecond, this.Audio.Length );
            }
        }


        public TimeSpan Position {
            get {
                return Convert.ToTime( this.AverageBytesPerSecond, this.BytePostion );
            }
            set {
                long result = Convert.ToBytePosition( value, this.AverageBytesPerSecond );
                if( result < this.Length ) {
                    this.BytePostion = result;
                }
            }
        }


        public IAudioTags Tags { get; private set; }


        public AudioControlModule( string location ) : base( location ) {
            this.TotalTime = Convert.ToTime( this.AverageBytesPerSecond, this.Length );
        }


        public void Pause() {
            this.CheckIfDisposed();
            this.WaveOut.Pause();
        }


        public void Stop() {
            this.CheckIfDisposed();
            this.WaveOut.Stop();
        }


        protected override void StartCaching() {
            base.StartCaching();
            try {
                this.Tags = Module.Tags.Create( this.Audio );
            }
            catch {
                this.Tags = new Tags();
            }
        }
    }
}
