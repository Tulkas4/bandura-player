using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bandura.Audio;

namespace Bandura.Audio.Module {


    public class Player : IPlayer {


        private WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();


        protected WMPLib.IWMPControls Controls {
            get {
                return player.controls;
            }
        }


        public Player( ) {

        }


        public void Pause() {
            this.Controls.pause();
        }


        public void Play() {
            this.Controls.play();
        }


        public void Stop() {
            this.Controls.stop();
        }
    }
}
