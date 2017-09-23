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
using Android.Support.V7.App;
using YoutubeExtractor;
using System.Threading.Tasks;
using System.Net;
using Background_Youtube_Player.Code.Services;
using Background_Youtube_Player.Code.Settings;
using Background_Youtube_Player.Code.Helpers;

namespace Background_Youtube_Player
{
    [Activity(Label = "SongActivity")]
    public class SongActivity : BaseActivity
    {
        VideoView videoView;
        MediaService mediaService = new MediaService();
        string link;

        NotificationManager notificationManager;
        Notification notification;
        const int notificationId = 0;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            link = Intent.GetStringExtra("link");
            SetContentView(Resource.Layout.Song);
            FindViews();
            mediaService.CreateMediaPlayer();
            var video = await VideoHelper.ResolveDownloadUrls(link);
        }

        public void FindViews()
        {
            videoView = FindViewById<VideoView>(Resource.Id.videoView);
        }

        private void PlayVideo(VideoInfo video)
        {
            videoView.SetVideoPath(video.DownloadUrl);
            videoView.SetMediaController(new MediaController(this));
            videoView.Start();
        }

        public async Task Play(VideoInfo video)
        {
            if (AppSettings.PlayOnlySound)
            {
                await PlaySong(video);
            }
            else PlayVideo(video);
        }


        private async Task PlaySong(VideoInfo video)
        {
            await mediaService.StartPlayingSong(video.DownloadUrl);
            CreateNotification(video);
        }

        public void CreateNotification(VideoInfo video)
        {
            notificationManager =
                GetSystemService(NotificationService) as NotificationManager;

            var intent =
                this.PackageManager.GetLaunchIntentForPackage(this.PackageName);
            intent.AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            Notification.Builder builder = new Notification.Builder(this)
            .SetContentTitle("Currently playing:")
            .SetContentText(video.Title)
            .SetSmallIcon(Resource.Drawable.ic_play_circle)
            .SetContentIntent(pendingIntent);
            notification = builder.Build();

            notificationManager.Notify(notificationId, notification);
        }
    }
}