using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace audio_module_test {


    public partial class Form1 : Form {

        private Bandura.Audio.Module.AudioControlModule player;

        public Form1() {
            InitializeComponent();
            player = new Bandura.Audio.Module.AudioControlModule(
                //@"C:\Users\amashko\Desktop\Calvin Harris and Disciples - How Deep Is Your Love (Chris Lake Remix).mp3" 
                @"C:\Users\amashko\Desktop\dc8af947b6d4c8.mp3"
                //"http://cs422923v4.vk.me/u227861693/audios/0868b1df2500.mp3?extra=UlNJzzWJn1fAYHLO7Hr1tIO0LwR0YjEvu9CXPOCaEVmIdMDP3if-sxSr7P2y84uqyh9N7Q2QjaJOEn5nheI7LkMCAi6jsslQ,278"
            );
            this.label2.Text = player.TotalTime.ToString( @"hh\:mm\:ss" );
            this.player.Play();
            var tags = this.player.Tags;
            if( tags != null ) { }
        }


        private void trackBar1_ValueChanged( object sender, EventArgs e ) {
            double val = player.TotalTime.TotalSeconds * 0.01d;         //1%
            var ts = TimeSpan.FromSeconds( val * this.trackBar1.Value );
            this.player.Position = ts;
            this.label1.Text = ts.ToString( @"hh\:mm\:ss" );     
        }
    }
}
