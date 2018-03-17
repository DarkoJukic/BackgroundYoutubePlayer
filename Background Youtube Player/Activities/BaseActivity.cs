
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
        protected DrawerLayout drawerLayout;


        protected NotificationManager notificationManager;
        protected Notification notification;
        const int notificationId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            e.MenuItem.SetChecked(true);

            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    StartActivity(typeof(HomeActivity));
                    Finish();
                    break;

                case Resource.Id.nav_favorites:
                    StartActivity(typeof(FavoritesActivity));
                    Finish();
                    break;

                case Resource.Id.nav_settings:
                    StartActivity(typeof(SettingsActivity));
                    Finish();
                    break;
            }

            drawerLayout.CloseDrawers();
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