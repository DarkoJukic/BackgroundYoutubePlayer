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

namespace Background_Youtube_Player.Code.Helpers
{
    public class DisplayHelper
    {

        public static ProgressDialog MakeProgressDialog(Context context, string title)
        {
            var progressDialog = new ProgressDialog(context);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            //progressDialog.SetTitle(title);
            return progressDialog;
        }

        public void AlertUserOfError(string message, Android.Content.Context context)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle("Error");
            alert.SetMessage(message);
            alert.SetPositiveButton("Ok", (senderAlert, args) => {
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

    }
}