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
using YoutubeExtractor;
using Background_Youtube_Player.Resources.model;

namespace Background_Youtube_Player.Code.Data
{
    public static class FavoriteVideos
    {
        public static List<Youtube.Item> List = new List<Youtube.Item>();
    }
}