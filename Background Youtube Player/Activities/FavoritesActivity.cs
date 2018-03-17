using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using YoutubeExtractor;
using Background_Youtube_Player.Resources.model;
using Newtonsoft.Json;
using Android.Views;
using Android.Content;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Background_Youtube_Player.Code.Services;
using Background_Youtube_Player.Code.Helpers;
using Background_Youtube_Player.Code.Data;

namespace Background_Youtube_Player
{
    [Activity(Label = "Favorites", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FavoritesActivity : BaseActivity
    {
        SearchView songSearchView;
        Toolbar toolbar;
        NavigationView navigationView;

        NotificationManager notificationManager;
        Notification notification;
        const int notificationId = 0;

        VideoHelper VideoHelper = new VideoHelper();

        ListView songListView;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Home);
            FindViews();
            HandleEvents();
             GetFavorites(this, null);
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
            //songSearchView.QueryTextSubmit += async (sender, e) => await SearchForSong(sender, e);
            songSearchView.Click += SongSearchView_Click;
        }

        private void SongSearchView_Click(object sender, System.EventArgs e)
        {
            songSearchView.Iconified = false;
        }

        protected void FindViews()
        {
            songListView = FindViewById<ListView>(Resource.Id.resultsListView);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.ToolbarTitle);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);
            navigationView = FindViewById<NavigationView>(Resource.Id.NavigationView);
            navigationView.SetCheckedItem(Resource.Id.nav_home);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            songSearchView = FindViewById<SearchView>(Resource.Id.songSearchView);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            toolbar.InflateMenu(Resource.Menu.toolbar_menu);
            toolbar.MenuItemClick += (sender, e) =>
            {
                if (notificationManager != null)
                    notificationManager.CancelAll();
                MediaService.Stop();
            };
            return base.OnPrepareOptionsMenu(menu);
        }

        private void GetFavorites(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            var listOfVideos = FavoriteVideos.List;
            if (listOfVideos != null)
            {
                songListView.Adapter = new VideoAdapter(Application.Context, listOfVideos);
                songListView.ItemClick += async (s, events) => await StartPlayingSong(s, events);
            }
        }

        private Task<Youtube.RootObject> DeserializeObjectAsync(string content)
        {
            return Task.Factory.StartNew(() =>
            {
                var items = JsonConvert.DeserializeObject<Youtube.RootObject>(content);
                return items;
            });
        }

        private async Task StartPlayingSong(object sender, AdapterView.ItemClickEventArgs e)
        {
            var dialog = DisplayHelper.MakeProgressDialog(this, "Wait...");
            dialog.Show();
            var adapter = songListView.Adapter as VideoAdapter;
            var id = adapter[e.Position].id.videoId;
            string link = "https://www.youtube.com/watch?v=" + id;

            var x = this;

            var video = await VideoHelper.ResolveDownloadUrls(link, this);
            await Play(video);

            //var songActivity = new Intent(this, typeof(SongActivity));
            //songActivity.PutExtra("link", link);
            //StartActivity(songActivity);
            dialog.Hide();
        }

        public async Task Play(VideoInfo video)
        {
            if (video != null)
            {
                await PlaySong(video);
            }
        }

        private async Task PlaySong(VideoInfo video)
        {
            await MediaService.Start(video.DownloadUrl);
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
