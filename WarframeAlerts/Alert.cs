using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WarframeAlerts
{
    public class Alert
    {
        public DateTime expiration;
        public int credits, endo;
        public List<Reward> rewards;
        public string planet;

        Alert()
        {
            rewards = new List<Reward>();
        }

        public static Alert FromTweet(string tweet, DateTime start)
        {
            if (tweet.StartsWith("Sortie"))
                return null;
            Alert alert = new Alert();
            List<string> rewardStrings = new List<string>();
            if (tweet.Contains("Invasion:"))
            {
                alert.expiration = start.AddMinutes(60);
                MatchCollection matches = Regex.Matches(tweet, @"\(([^\)]+)\)");
                alert.planet = matches[0].Value.Substring(1, matches[0].Value.Length - 2);
                for (int i = 1; i < matches.Count; i++)
                    rewardStrings.Add(matches[i].Value.Substring(1, matches[i].Value.Length - 2));
            }
            else
            {
                alert.planet = Regex.Match(tweet, @"\(([^\)]+)\)").Value;
                alert.planet = alert.planet.Substring(1, alert.planet.Length - 2);
                string[] tokens = tweet.Split(new string[] { " - " }, StringSplitOptions.None);
                alert.expiration = start.AddMinutes(int.Parse(tokens[1].Substring(0, tokens[1].Length - 1)));
                alert.credits = int.Parse(tokens[2].Substring(0, tokens[2].Length - 2));
                if (tokens.Length == 4)
                {
                    string rewardString = tokens[3];
                    if (rewardString.Contains("ENDO"))
                        alert.endo = int.Parse(rewardString.Split(' ')[0]);
                    else
                        rewardStrings.Add(rewardString);
                }
            }
            for (int i = 0; i < rewardStrings.Count; i++)
            {
                string rewardString = rewardStrings[i];
                if (rewardString.Contains('('))
                    rewardString = rewardString.Substring(0, rewardString.IndexOf('(') - 1);
                if (rewardString.Split(' ')[0].EndsWith("x"))
                    rewardString = rewardString.Substring(rewardString.IndexOf('x') + 2);
                alert.rewards.Add(Reward.FromString(rewardString));
            }
            return alert;
        }
    }
}
