using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Twitterizer;

namespace WarframeAlerts
{
    class Context1 : ApplicationContext
    {
        static string OAUTH_CONSUMER_KEY = "<YOUR TWITTER API KEYS HERE>";
        static string OAUTH_CONSUMER_SECRET = "<YOUR TWITTER API KEYS HERE>";

        Form1 configWindow;
        NotifyIcon notifyIcon;
        OAuthTokens tokens;
        ConnectionStatus status = ConnectionStatus.Connecting;

        DateTime lastTweetTime;

        public Context1()
        {
            configWindow = new Form1();

            MenuItem configMenuItem = new MenuItem("Modify filter...", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += ShowConfig;

            if (OAUTH_CONSUMER_KEY == "<YOUR TWITTER API KEYS HERE>")
                throw new Exception("Fill in your own Twitter API keys above.");
            tokens = new OAuthTokens();
            tokens.ConsumerKey = OAUTH_CONSUMER_KEY;
            tokens.ConsumerSecret = OAUTH_CONSUMER_SECRET;

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(TimerEvent);
            timer.Interval = 60000;
            timer.Enabled = true;

            configWindow.Log("Connecting...");
            CheckAlert();

            configWindow.Show();
        }

        public void TimerEvent(object source, ElapsedEventArgs e)
        {
            CheckAlert();
        }
        void CheckAlert()
        {
            UserTimelineOptions userOptions = new UserTimelineOptions();
            userOptions.APIBaseAddress = "https://api.twitter.com/1.1/";
            userOptions.Count = 10;
            userOptions.UseSSL = true;
            userOptions.ScreenName = "WarframeAlerts";
            TwitterResponse<TwitterStatusCollection> timeline = null;
            try
            {
                timeline = TwitterTimeline.UserTimeline(tokens, userOptions);
            }
            catch (Exception) { }

            if (timeline == null || timeline.Content == null)
            {
                if (status != ConnectionStatus.Issue)
                    configWindow.Log("Having trouble reaching Twitter. Will retry every 60 seconds.");
                status = ConnectionStatus.Issue;
                return;
            }

            if (status == ConnectionStatus.Connecting)
                configWindow.Log("Connected. Scanning for alerts matching your filter...");
            if (status == ConnectionStatus.Issue)
                configWindow.Log("Reconnected.");
            status = ConnectionStatus.Connected;

            for (int i = 9; i >= 0; i--)
            {
                dynamic tweet = JArray.Parse(timeline.Content)[i];
                string createdAt = tweet.created_at;
                DateTime dt = DateTime.ParseExact(createdAt, "ddd MMM dd HH:mm:ss \"+0000\" yyyy", CultureInfo.InvariantCulture).ToLocalTime();
                if (dt <= lastTweetTime)
                    continue;
                lastTweetTime = dt;
                string text = tweet.text;
                try
                {
                    NewAlert(text, dt);
                }
                catch (Exception e)
                {
                    configWindow.Log("Trouble parsing: " + text);
                    configWindow.Log(e.ToString());
                }
            }
        }

        void NewAlert(string tweet, DateTime start)
        {
            Alert alert = Alert.FromTweet(tweet, start);
            if (alert == null)
                return;
            if (alert.expiration < DateTime.Now)
                return;
            if (!configWindow.IsPlanetChecked(alert.planet))
                return;
            if (!configWindow.HasWantedReward(alert))
                return;
            
            if (!tweet.Contains("Invasion: "))
            {
                List<string> tokens = tweet.Split(new string[] { " - " }, StringSplitOptions.None).ToList();
                tokens.RemoveAt(1);
                tweet = string.Join(" - ", tokens);
            }
            int minutesLeft = (int)(alert.expiration - DateTime.Now).TotalMinutes;
            string minutesString;
            if (minutesLeft > 1)
                minutesString = minutesLeft + " minutes remaining";
            else if (minutesLeft == 1)
                minutesString = "1 minute remaining";
            else
                minutesString = "<1 minute remaining";
            tweet += ": " + minutesString;
            configWindow.Log("New alert matching your filter: " + tweet);
            Popup(tweet);
        }
        void Popup(string text)
        {
            notifyIcon.BalloonTipTitle = "New alert matching your filter!";
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.ShowBalloonTip(10000);
        }

        void ShowConfig(object sender, EventArgs e)
        {
            configWindow.Popup();
        }
        public void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Environment.Exit(0);
        }
    }

    enum ConnectionStatus
    {
        Connecting, Connected, Issue
    }
}
