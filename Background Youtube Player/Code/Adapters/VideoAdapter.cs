﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;

namespace Background_Youtube_Player
{
    public class VideoAdapter : BaseAdapter<Resources.model.Youtube.Item>

    {
        LayoutInflater inflater;

        public List<Resources.model.Youtube.Item> video { get; set; }

        public VideoAdapter(Context context, List<Resources.model.Youtube.Item> videos)
        {
            inflater = LayoutInflater.FromContext(context);
            video = videos;
        }


        public override Resources.model.Youtube.Item this[int index]
        {
            get { return video[index]; }
        }



        public override int Count
        {
            get
            {
                return video.Count;
            }
        }



        public override long GetItemId(int position)
        {
            return position;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            //throw new System.NotImplementedException();
            View view = convertView ?? inflater.Inflate(Resource.Layout.SongListItemLayout, parent, false);

            var item = video[position];

            var viewHolder = view.Tag as ViewHolder;

            if (viewHolder == null)
            {
                viewHolder = new ViewHolder();
                viewHolder.Title = view.FindViewById<TextView>(Resource.Id.Title);
                viewHolder.ChannelTitle = view.FindViewById<TextView>(Resource.Id.ChannelTitle);
                //viewHolder.ViewCount = view.FindViewById<TextView>(Resource.Id.ViewCount);
                viewHolder.Thumbnail = view.FindViewById<ImageViewAsync>(Resource.Id.Thumbnail);
                view.Tag = viewHolder;
            }



            viewHolder.Title.Text = item.snippet.title;
            viewHolder.ChannelTitle.Text = item.snippet.channelTitle;

            if (item.snippet.thumbnails.@default.url != null)
            {
                ImageService.Instance.LoadUrl(item.snippet.thumbnails.@default.url)
                            .Retry(5, 200)
                            .Into(viewHolder.Thumbnail);
            }

            else
            {
                ImageService.Instance.LoadUrl("https://3.bp.blogspot.com/-uq0bk_FR1vw/VtAJPybeGkI/AAAAAAAAAq0/MMPAzz0ABgI/s1600/no-thumbnail.png")
                            .Retry(5, 200)
                            .Into(viewHolder.Thumbnail);
            }
            return view;
        }


        public class ViewHolder : Java.Lang.Object

        {
            public TextView Title { get; set; }
            public TextView ChannelTitle { get; set; }
            //public TextView ViewCount { get; set; }
            public ImageViewAsync Thumbnail { get; set; }
        }
    }
}