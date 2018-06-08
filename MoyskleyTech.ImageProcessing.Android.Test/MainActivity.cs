using Android.App;
using Android.Widget;
using Android.OS;

namespace MoyskleyTech.ImageProcessing.Android.Test
{
    [Activity(Label = "MoyskleyTech.ImageProcessing.Android.Test", Name ="pkMK.Main" , MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(new DrawView(this));
        }
        
    }
}

