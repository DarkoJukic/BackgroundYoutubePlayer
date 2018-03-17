using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using System.Threading.Tasks;

namespace Background_Youtube_Player.Code.Services
{
    public class MediaService
    {
        private static MediaPlayer _mediaPlayer;

        private static Object lockObject = new Object();

        private MediaService()
        {
        }

        public static MediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer != null)
                    return _mediaPlayer;
                lock (lockObject)
                {
                    if (_mediaPlayer != null)
                        return _mediaPlayer;
                    else
                    {
                        _mediaPlayer = new MediaPlayer();
                    }
                    return _mediaPlayer;
                }
            }
        }

        public static async Task Start(string DownloadUrl)
        {
            MediaPlayer.Reset();
            await MediaPlayer.SetDataSourceAsync(DownloadUrl);
            MediaPlayer.Prepare();
            MediaPlayer.Start();
        }

        public static void Stop()
        {
            MediaPlayer.Stop();
        }

        public static void Pause()
        {
            MediaPlayer.Pause();
        }

    }
}