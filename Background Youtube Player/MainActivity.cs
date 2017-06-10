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

namespace Background_Youtube_Player
{
    [Activity(Label = "Darkov Youtube Player", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        MediaPlayer player;
        SearchView songSearchView;
        Toolbar toolbar;
        ListView songListView;
        string tag = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=50&type=video&key=AIzaSyADs8hX9blKmzfBRkVGxLcQhRdMB80qBTc&q=";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            FindViews();
            HandleEvents();
            CreateMediaPlayer();
        }

        private void HandleEvents()
        {
            songSearchView.QueryTextSubmit += async (sender, e) => await SearchForSong(sender, e);
        }

        private void CreateMediaPlayer()
        {
            if (player == null)
            {
                player = new MediaPlayer();
            }
        }

        protected void FindViews()
        {
            songListView = FindViewById<ListView>(Resource.Id.resultsListView);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.InflateMenu(Resource.Menu.toolbar_menu);
            toolbar.SetTitle(Resource.String.ToolbarTitle);

            toolbar.MenuItemClick += (sender, e) => {
                player.Stop();
            };
            songSearchView = FindViewById<SearchView>(Resource.Id.songSearchView);
        }

        private async Task SearchForSong(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            var dialog = MakeProgressDialog("Searching...");
            dialog.Show();
            var dataService = new DataService();
            var content = await dataService.GetRequestJson(tag + songSearchView.Query);
            var items = JsonConvert.DeserializeObject<Youtube.RootObject>(content);
            dialog.Hide();
            songListView.Adapter = new VideoAdapter(Application.Context, items.items);
            songListView.ItemClick += async (s, events) => await StartPlayingSong(s, events);
            Window.SetSoftInputMode(SoftInput.StateHidden);
        }

        private async Task StartPlayingSong(object sender, AdapterView.ItemClickEventArgs e)
        {
            var dialog = MakeProgressDialog("Wait...");
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
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                    Toast.MakeText(this, "Cannot play this video", ToastLength.Short);
                    GetHttpResponse(video.DownloadUrl, video);
                }
                else
                {
                    PlaySong(video);
                }
                dialog.Hide();
            }
        }

        private void PlaySong(VideoInfo video)
        {
            player.Stop();
            player.Reset();
            player.SetDataSource(video.DownloadUrl);
            player.Prepare();
            player.Start();
        }

        public void GetHttpResponse(string url, VideoInfo video)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                PlaySong(video);
            }
            catch (WebException ex)
            {
                Toast.MakeText(this, "Cannot play this video" + ex.Status, ToastLength.Short);
            }
        }

        private ProgressDialog MakeProgressDialog(string title)
        {
            var progressDialog = new ProgressDialog(this);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            //progressDialog.SetTitle(title);
            return progressDialog;
        }
    }
}
