using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using Background_Youtube_Player.Code.Services;
using YoutubeExtractor;
using Android.Content;

namespace Background_Youtube_Player
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected Toolbar toolbar;
        protected Toolbar bottomToolbar;
        protected NavigationView navigationView;
        protected DrawerLayout drawerLayout;

        protected NotificationManager notificationManager;
        protected Notification notification;
        const int notificationId = 0;

        Toast backToast;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        protected override void OnRestart()
        {
            base.OnRestart();
            HandleButtonsVisibility();
        }

        public override void OnBackPressed()
        {
            if (backToast != null && backToast.View.WindowToken != null)
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
            else
            {
                backToast = Toast.MakeText(ApplicationContext, " Press Back again to Exit ", ToastLength.Short);
                backToast.Show();
            }
        }

        private void HandleButtonsVisibility()
        {
            bottomToolbar.Visibility = ViewStates.Visible;

            if (MediaService.IsPlaying())
            {
                bottomToolbar.Menu.FindItem(Resource.Id.menu_play).SetVisible(false);
                bottomToolbar.Menu.FindItem(Resource.Id.menu_pause).SetVisible(true);
                bottomToolbar.Menu.FindItem(Resource.Id.menu_stop).SetVisible(true);
                return;
            }
            if (!MediaService.IsPlaying() && MediaService.MediaPlayer.CurrentPosition > 1)
            {
                bottomToolbar.Menu.FindItem(Resource.Id.menu_play).SetVisible(true);
                bottomToolbar.Menu.FindItem(Resource.Id.menu_pause).SetVisible(false);
                bottomToolbar.Menu.FindItem(Resource.Id.menu_stop).SetVisible(true);
                return;
            }
            else
            {
                bottomToolbar.Visibility = ViewStates.Gone;
            }
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

        protected virtual void FindViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.ToolbarTitle);
            bottomToolbar = FindViewById<Toolbar>(Resource.Id.bottomToolbar);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);

            navigationView = FindViewById<NavigationView>(Resource.Id.NavigationView);
            navigationView.SetCheckedItem(Resource.Id.nav_home);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            bottomToolbar.InflateMenu(Resource.Menu.bottom_menu);

            bottomToolbar.MenuItemClick += BottomToolbar_MenuItemClick;

            return base.OnCreateOptionsMenu(menu);
        }

        private void BottomToolbar_MenuItemClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            var item = e.Item;
            switch (item.ItemId)
            {
                case Resource.Id.menu_pause:
                    {
                        bottomToolbar.Menu.FindItem(Resource.Id.menu_play).SetVisible(true);
                        e.Item.SetVisible(false);
                        MediaService.Pause();
                        break;
                    }
                case Resource.Id.menu_stop:
                    {
                        if (notificationManager != null)
                            notificationManager.CancelAll();
                        bottomToolbar.Visibility = ViewStates.Gone;
                        MediaService.Stop();
                        break;
                    }
                case Resource.Id.menu_play:
                    {                       
                        bottomToolbar.Menu.FindItem(Resource.Id.menu_pause).SetVisible(true);
                        e.Item.SetVisible(false);
                        MediaService.Continue();
                        break;
                    }
                default:
                    break;
            }
        }

        protected void CreateNotification(VideoInfo video)
        {
            notificationManager = GetSystemService(NotificationService) as NotificationManager;

            var intent = this.PackageManager.GetLaunchIntentForPackage(this.PackageName);
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