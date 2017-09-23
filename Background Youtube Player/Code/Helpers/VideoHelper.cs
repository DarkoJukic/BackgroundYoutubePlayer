using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Widget;
using System.Threading.Tasks;
using YoutubeExtractor;
using System.Net;

namespace Background_Youtube_Player.Code.Helpers
{
    public class VideoHelper
    {
        public async Task<WebResponse> GetHttpResponse(string url, VideoInfo video)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            return await request.GetResponseAsync();
        }

        public Task DecryptDownloadUrlAsync(VideoInfo video)
        {
            return Task.Factory.StartNew(() =>
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);

                return Task.FromResult(true);
            });
        }

        public async Task<VideoInfo> ResolveDownloadUrls(string link, Android.Content.Context context)
        {
            IEnumerable<VideoInfo> videosInfors = new List<VideoInfo>();
            try
            {
                 videosInfors = await DownloadUrlResolver.GetDownloadUrlsAsync(link, true);
            }
            catch (Exception ex)
            {
                var displayHelper = new DisplayHelper();
                displayHelper.AlertUserOfError(ex.Message, context);
                Toast.MakeText(context, "Cannot play this video", ToastLength.Short);
            }

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
                    await this.DecryptDownloadUrlAsync(video);
                    var response = await this.GetHttpResponse(video.DownloadUrl, video);
                }
            }
            return video;
        }
    }
}