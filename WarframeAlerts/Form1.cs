using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WarframeAlerts
{
    public partial class Form1 : Form
    {
        bool eventFreeze = false;
        List<CheckBox> planetBoxes, rewardBoxes;

        public Form1()
        {
            InitializeComponent();

            planetBoxes = new List<CheckBox>();
            AddCheckBoxes(new string[]{ "Mercury", "Venus", "Earth", "Lua", "Mars", "Phobos", "Ceres", "Jupiter", "Europa", "Saturn", "Uranus", "Neptune", "Pluto", "Sedna", "Eris", "Void", "Kuva Fortress" }, tabPagePlanets, planetBoxes);
            rewardBoxes = new List<CheckBox>();
            AddCheckBoxes(Reward.AURAS, tabPage3, rewardBoxes);
            AddCheckBoxes(Reward.BLUEPRINTS, tabPage5, rewardBoxes);
            AddCheckBoxes(Reward.MODS, tabPage4, rewardBoxes);
            AddCheckBoxes(Reward.RESOURCES, tabPage2, rewardBoxes);
            AddCheckBoxes(Reward.WEAPON_PARTS, tabPage7, rewardBoxes);
            AddCheckBoxes(new string[] { "Helmet Blueprints", "Kubrow Egg", "Weapon Skin Blueprints" }, tabPage6, rewardBoxes);

            this.FormClosing += Form1_Closing;

            if (Properties.Settings.Default.Planets == null)
            {
                Properties.Settings.Default.Planets = new StringCollection();
                Properties.Settings.Default.Rewards = new StringCollection();
                WriteSettings();
            }
            else
                ReadSettings();
        }

        public void AddCheckBoxes(string[] labels, TabPage tabPage, List<CheckBox> boxList)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Location = new Point(6 + (i / 15) * 178, 6 + (i % 15) * 23);
                checkBox.Size = new System.Drawing.Size(175, 17);
                checkBox.Text = labels[i];
                checkBox.Checked = true;
                checkBox.CheckedChanged += SettingsChanged;
                tabPage.Controls.Add(checkBox);
                boxList.Add(checkBox);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.AppendText("");
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            if (eventFreeze)
                return;
            WriteSettings();
        }

        public bool IsPlanetChecked(string planet)
        {
            foreach (CheckBox checkBox in planetBoxes)
                if (checkBox.Text == planet)
                    return checkBox.Checked;
            throw new Exception("Unknown planet name!");
        }
        public bool HasWantedReward(Alert alert)
        {
            if (checkBox1.Checked && alert.credits >= creditAmount.Value)
                return true;
            if (checkBox2.Checked && alert.endo >= endoAmount.Value)
                return true;
            foreach (Reward reward in alert.rewards)
            {
                string name;
                if (reward.name.EndsWith("Helmet"))
                    name = "Helmet Blueprints";
                else if (reward.name.EndsWith("Skin"))
                    name = "Weapon Skin Blueprints";
                else
                    name = reward.name;
                foreach (CheckBox checkBox in rewardBoxes)
                    if (checkBox.Text == reward.name)
                        return checkBox.Checked;
            }
            return false;
        }

        // SETTINGS
        public void ReadSettings()
        {
            eventFreeze = true;
            foreach (CheckBox checkBox in planetBoxes)
                checkBox.Checked = Properties.Settings.Default.Planets.Contains(checkBox.Text);
            foreach (CheckBox checkBox in rewardBoxes)
                checkBox.Checked = Properties.Settings.Default.Rewards.Contains(checkBox.Text);
            checkBox1.Checked = Properties.Settings.Default.Credits;
            creditAmount.Value = Properties.Settings.Default.CreditAmount;
            checkBox2.Checked = Properties.Settings.Default.Endo;
            endoAmount.Value = Properties.Settings.Default.EndoAmount;
            eventFreeze = false;
        }
        public void WriteSettings()
        {
            Properties.Settings.Default.Planets.Clear();
            foreach (CheckBox checkBox in planetBoxes)
                if (checkBox.Checked)
                    Properties.Settings.Default.Planets.Add(checkBox.Text);
            Properties.Settings.Default.Rewards.Clear();
            foreach (CheckBox checkBox in rewardBoxes)
                if (checkBox.Checked)
                    Properties.Settings.Default.Rewards.Add(checkBox.Text);
            Properties.Settings.Default.Credits = checkBox1.Checked;
            Properties.Settings.Default.CreditAmount = (int)creditAmount.Value;
            Properties.Settings.Default.Endo = checkBox2.Checked;
            Properties.Settings.Default.EndoAmount = (int)endoAmount.Value;
            Properties.Settings.Default.Save();
        }

        public void Popup()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Popup));
                return;
            }
            if (!Visible)
                Show();
            Activate();
        }
        public void Log(string message)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action<string>(Log), message);
                return;
            }
            if (textBox1.Text.Length > 0)
                textBox1.AppendText(Environment.NewLine);
            textBox1.AppendText(DateTime.Now.ToString("h:mm:ss tt") + ": " + message);
        }
    }
}
