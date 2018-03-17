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


//http://solola.ca/xam-android-videoview-part2/
//https://developer.android.com/training/tv/playback/options.html
namespace Background_Youtube_Player
{
    [Activity(Label = "SongActivity")]
    public class SongActivity : BaseActivity
    {

        VideoView videoView;

        VideoHelper VideoHelper = new VideoHelper();

        Android.Widget.MediaController MediaController;

        string link;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            link = Intent.GetStringExtra("link");
            SetContentView(Resource.Layout.Song);
            FindViews();
            HandleEvents();
            var video = await VideoHelper.ResolveDownloadUrls(link, this);
            PlayVideo(video);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();

            RequestVisibleBehind(true);
            //if (videoView.IsPlaying)
            //{
            //    //Argument equals true to notify the system that the activity
            //    //wishes to be visible behind other translucent activities
            //    if (!RequestVisibleBehind(true))
            //    {
            //        // App-specific method to stop playback and release resources
            //        // because call to requestVisibleBehind(true) failed
            //        //videoView.StopPlayback();
            //    }
            //}
            //else
            //{
            //    // Argument equals false because the activity is not playing
            //    RequestVisibleBehind(false);
            //}
        }

        public void FindViews()
        {
            videoView = FindViewById<VideoView>(Resource.Id.videoView);
        }


        [Android.Runtime.Register("requestVisibleBehind", "(Z)Z", "GetRequestVisibleBehind_ZHandler")]
        public override Boolean RequestVisibleBehind(Boolean visible)
        {
            if (videoView.IsPlaying)
            {
                // Argument equals true to notify the system that the activity
                // wishes to be visible behind other translucent activities
                if (!RequestVisibleBehind(true))
                {
                    // App-specific method to stop playback and release resources
                    // because call to requestVisibleBehind(true) failed
                    //stopPlayback();
                    videoView.StopPlayback();
                }
            }
            else
            {
                // Argument equals false because the activity is not playing
                RequestVisibleBehind(false);
            }

            return true;
        }

        public override void OnVisibleBehindCanceled()
        {
            base.OnVisibleBehindCanceled();
            videoView.StopPlayback();
            // App-specific method to stop playback and release resources
        }


        private void HandleEvents()
        {
            videoView.Prepared += OnVideoPlayerPrepared;
        }


        private void OnVideoPlayerPrepared(object sender, EventArgs e)
        {
            MediaController.SetAnchorView(videoView);

            //show media controls for 3 seconds when video starts to play
            MediaController.Show(3000);
        }


        private void CreateMediaController()
        {
        }

        private void PlayVideo(VideoInfo video)
        {
            MediaController = new MediaController(this, true);

            videoView.SetVideoPath(video.DownloadUrl);
            videoView.SetMediaController(MediaController);
            videoView.Start();
        }

    }
}