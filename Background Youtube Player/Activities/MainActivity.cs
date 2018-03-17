using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using YoutubeExtractor;
using Background_Youtube_Player.Resources.Model;
using Newtonsoft.Json;
using Android.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Background_Youtube_Player.Code.Services;
using Background_Youtube_Player.Code.Helpers;
using Background_Youtube_Player.Code;

namespace Background_Youtube_Player
{
    [Activity(Label = "Youtube", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HomeActivity : BaseActivity
    {
        SearchView songSearchView;

        VideoHelper VideoHelper = new VideoHelper();

        ListView songListView;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            FindViews();
            HandleEvents();
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

        protected override void FindViews()
        {
            base.FindViews();

            songListView = FindViewById<ListView>(Resource.Id.resultsListView);

            songSearchView = FindViewById<SearchView>(Resource.Id.songSearchView);
        }


        private async Task SearchForSong(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            var dialog = DisplayHelper.MakeProgressDialog(this, "Searching...");
            dialog.Show();
            var dataService = new DataService();
            var content = await dataService.GetRequestJson(Constants.Tag + songSearchView.Query);
            var result = await DeserializeObjectAsync(content);

            dialog.Hide();
            songSearchView.ClearFocus();
            songListView.Adapter = new VideoAdapter(Application.Context, result.items);
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

        private async Task StartPlayingSong(object sender, AdapterView.ItemClickEventArgs e)
        {
            var dialog = DisplayHelper.MakeProgressDialog(this, "Wait...");
            dialog.Show();
            var adapter = songListView.Adapter as VideoAdapter;
            var id = adapter[e.Position].id.videoId;
            string link = Constants.YoutubeBaseUrl + id;


            var video = await VideoHelper.ResolveDownloadUrls(link, this);
            await Play(video);

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

    }
}
