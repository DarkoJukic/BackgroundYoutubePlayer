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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.View;

namespace Background_Youtube_Player
{

    [Activity(Label = "Settings")]
    public class SettingsActivity : AppCompatActivity
    {
        Toolbar toolbar;
        Toolbar bottomToolbar;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            FindViews();
        }

        public void FindViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetTitle(Resource.String.ToolbarTitle);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);
            navigationView = FindViewById<NavigationView>(Resource.Id.NavigationView);
            navigationView.SetCheckedItem(Resource.Id.nav_settings);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Settings";

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        StartActivity(typeof(HomeActivity));
                        break;

                    case Resource.Id.nav_favorites:
                        Toast.MakeText(this, "", ToastLength.Short);
                        break;

                    case Resource.Id.nav_settings:
                        StartActivity(typeof(SettingsActivity));
                        break;

                }
            };
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

    }
}