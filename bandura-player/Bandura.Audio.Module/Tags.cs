namespace Bandura.Audio.Module {

    /// <summary>
    /// 
    /// </summary>
    internal class Tags : IAudioTags {


        /// <summary>
        /// 
        /// </summary>
        public string Album {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Artist {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Comment {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Composer {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Disc {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Genre {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Length {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Lyrics {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public System.Drawing.Image Picture {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Title {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Track {
            get;
            internal set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string Year {
            get;
            internal set;
        }


        public Tags( ) {
            
        }


        internal static IAudioTags Create( System.IO.Stream audio_stream ) {
            Id3Lib.TagModel model = Id3Lib.TagManager.Deserialize( audio_stream );
            Tags t = new Tags();
            foreach( var item in model ) {
                string id = item.FrameId;
                switch( id ) {
                    case "TALB": {
                        t.Album = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TPE1": {
                        t.Artist = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "COMM": {
                        t.Comment = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TCOM": {
                        t.Composer = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TPOS": {
                        t.Disc = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TCON": {
                        t.Genre = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TLEN": {
                        t.Length = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "USLT": {
                        t.Lyrics = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "APIC": {
                        t.Picture = ((Id3Lib.Frames.FramePicture)item).Picture;
                        break;
                    }
                    case "TIT2": {
                        t.Title = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TRCK": {
                        t.Track = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                    case "TYER": {
                        t.Year = ((Id3Lib.Frames.FrameText)item).Text;
                        break;
                    }
                }
            }
            return t;
        }
    }
}
