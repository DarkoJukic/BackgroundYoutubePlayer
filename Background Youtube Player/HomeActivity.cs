using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using YoutubeExtractor;
using Background_Youtube_Player.Resources.model;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Android.Views;
using Android.Content;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using Background_Youtube_Player.Code.Services;
using Background_Youtube_Player.Code.Helpers;

namespace Background_Youtube_Player
{
    [Activity(Label = "Darkov Youtube Player", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HomeActivity : AppCompatActivity
    {
        SearchView songSearchView;
        Toolbar toolbar;
        Toolbar bottomToolbar;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        NotificationManager notificationManager;
        Notification notification;
        const int notificationId = 0;

        MediaService mediaService = new MediaService();


        ListView songListView;
        string tag = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=50&type=video&key=AIzaSyADs8hX9blKmzfBRkVGxLcQhRdMB80qBTc&q=";

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            FindViews();
            HandleEvents();
            mediaService.CreateMediaPlayer();
            await SearchForSong(this, null);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
        }


        protected override void OnStop()
        {
            base.OnStop();
        }

        private void HandleEvents()
        {
            songSearchView.QueryTextSubmit += async (sender, e) => await SearchForSong(sender, e);
            songSearchView.Click += SongSearchView_Click;
        }

        private void SongSearchView_Click(object sender, System.EventArgs e)
        {
            songSearchView.Iconified = false;
        }



        protected void FindViews()
        {


            //bottomToolbar = FindViewById<Toolbar>(Resource.Id.toolbar_bottom);
            //bottomToolbar.Title = "Song";
            //bottomToolbar.InflateMenu(Resource.Menu.bottom_menu);
            //bottomToolbar.MenuItemClick += (sender, e) => {
            //    Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            //};

            songListView = FindViewById<ListView>(Resource.Id.resultsListView);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.ToolbarTitle);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);
            navigationView = FindViewById<NavigationView>(Resource.Id.NavigationView);
            navigationView.SetCheckedItem(Resource.Id.nav_home);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        Toast.MakeText(this, "", ToastLength.Short);
                        break;

                    case Resource.Id.nav_favorites:
                        Toast.MakeText(this, "", ToastLength.Short);
                        break;
                }
                drawerLayout.CloseDrawers();
            };

            songSearchView = FindViewById<SearchView>(Resource.Id.songSearchView);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            toolbar.InflateMenu(Resource.Menu.toolbar_menu);
            toolbar.MenuItemClick += (sender, e) =>
            {
                if (notificationManager != null)
                    notificationManager.CancelAll();
                mediaService.StopMediaPlayer();
            };
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:

                    if (!drawerLayout.IsDrawerOpen(GravityCompat.Start))
                    {
                        drawerLayout.OpenDrawer(GravityCompat.Start);
                        return true;
                    }
                    else
                    {
                        drawerLayout.CloseDrawer(GravityCompat.Start);
                        return true;
                    }
            }
            return base.OnOptionsItemSelected(item);
        }

        private async Task SearchForSong(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            var dialog = DisplayHelper.MakeProgressDialog(this, "Searching...");
            dialog.Show();
            var dataService = new DataService();
            var content = await dataService.GetRequestJson(tag + songSearchView.Query);
            var items = await DeserializeObjectAsync(content);

            dialog.Hide();
            songSearchView.ClearFocus();
            songListView.Adapter = new VideoAdapter(Application.Context, items.items);
            songListView.ItemClick += async (s, events) => await StartPlayingSong(s, events);
            Window.SetSoftInputMode(SoftInput.StateHidden);
        }


        private Task<Youtube.RootObject> DeserializeObjectAsync(string content)
        {
            return Task.Factory.StartNew(() =>
            {
                var items = JsonConvert.DeserializeObject<Youtube.RootObject>(content);
                return items;
            });
        }

        private Task DecryptDownloadUrlAsync(VideoInfo video)
        {
            return Task.Factory.StartNew(() =>
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);

                return Task.FromResult(true);
            });
        }

        private async Task StartPlayingSong(object sender, AdapterView.ItemClickEventArgs e)
        {
            var dialog = DisplayHelper.MakeProgressDialog(this, "Wait...");
            dialog.Show();
            var adapter = songListView.Adapter as VideoAdapter;
            var id = adapter[e.Position].id.videoId;
            string link = "https://www.youtube.com/watch?v=" + id;

            IEnumerable<VideoInfo> videosInfors = await DownloadUrlResolver.GetDownloadUrlsAsync(link, false);
            VideoInfo video = videosInfors.FirstOrDefault(infor => infor.AudioType == AudioType.Aac);
            if (video != null)
            {
                if (video.RequiresDecryption)
                {
                    await DecryptDownloadUrlAsync(video);
                    Toast.MakeText(this, "Cannot play this video", ToastLength.Short);
                    await GetHttpResponse(video.DownloadUrl, video);
                }
                else
                {
                    await PlaySong(video);
                }
                dialog.Hide();
            }
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


        public async Task GetHttpResponse(string url, VideoInfo video)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            try
            {
                var response = await request.GetResponseAsync();
                await PlaySong(video);
            }
            catch (WebException ex)
            {
                Toast.MakeText(this, "Cannot play this video" + ex.Status, ToastLength.Short);
            }
        }
    }
}
