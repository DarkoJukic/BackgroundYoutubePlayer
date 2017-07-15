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
using System.Net.Http;

namespace Background_Youtube_Player
{
    class DataService
    {
        public async Task<string> GetRequestJson(string link)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage respone = await client.GetAsync(link);
            return await respone.Content.ReadAsStringAsync();
        }
    }
}
