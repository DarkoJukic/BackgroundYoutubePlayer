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

namespace Background_Youtube_Player.Code.Services
{
    public class MediaService
    {
        MediaPlayer player;

        public void CreateMediaPlayer()
        {
            if (player == null)
            {
                player = new MediaPlayer();
            }
        }

        public void StopMediaPlayer()
        {
            player.Stop();
        }

        public void StartPlayingNewSong(string DownloadUrl)
        {
            player.Stop();
            player.Reset();
            player.SetDataSource(DownloadUrl);
            player.Prepare();
            player.Start();
        }

    }
}