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
using System.Threading.Tasks;
using YoutubeExtractor;
using System.Net;
using Background_Youtube_Player.Code.Settings;

namespace Background_Youtube_Player.Code.Helpers
{
    public static class VideoHelper
    {
        public static async Task<WebResponse> GetHttpResponse(string url, VideoInfo video)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            return await request.GetResponseAsync();
        }

        public static Task DecryptDownloadUrlAsync(VideoInfo video)
        {
            return Task.Factory.StartNew(() =>
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);

                return Task.FromResult(true);
            });
        }

        public static async Task<VideoInfo> ResolveDownloadUrls(string link)
        {
            IEnumerable<VideoInfo> videosInfors = await DownloadUrlResolver.GetDownloadUrlsAsync(link, false);
            VideoInfo video = videosInfors.FirstOrDefault(infor => (infor.AudioType == AudioType.Aac || infor.AudioType == AudioType.Mp3) && infor.Resolution == 144 );

            if (video == null)
            {
                video = videosInfors.FirstOrDefault(infor => (infor.AudioType == AudioType.Aac || infor.AudioType == AudioType.Mp3) && infor.Resolution == 240);
            }

            if (video == null)
            {
                video = videosInfors.FirstOrDefault(infor => infor.AudioType == AudioType.Aac || infor.AudioType == AudioType.Mp3);
            }

            if (video != null)
            {
                if (video.RequiresDecryption)
                {
                    await VideoHelper.DecryptDownloadUrlAsync(video);
                    //Toast.MakeText(this, "Cannot play this video", ToastLength.Short);
                    var response = await VideoHelper.GetHttpResponse(video.DownloadUrl, video);
                }
            }
            return video;
        }
    }
}