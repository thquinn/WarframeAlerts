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
        static string presetSpecificsText = "comma-separated exact reward names: e.g. Excalibur Avalon Helmet, Dual Zoren Dagger-axe Skin";

        bool eventFreeze = false;
        List<CheckBox> planetBoxes, rewardBoxes;

        public Form1()
        {
            InitializeComponent();

            planetBoxes = new List<CheckBox>();
            rewardBoxes = new List<CheckBox>();
            AddCheckBoxes(new string[]{ "Mercury", "Venus", "Earth", "Lua", "Mars", "Phobos", "Ceres", "Jupiter", "Europa", "Saturn", "Uranus", "Neptune", "Pluto", "Sedna", "Eris", "Void", "Kuva Fortress" }, tabPagePlanets, planetBoxes);
            AddCheckBoxes(Reward.AURAS, tabPage3, rewardBoxes);
            AddCheckBoxes(Reward.BLUEPRINTS, tabPage5, rewardBoxes);
            AddCheckBoxes(Reward.MODS, tabPage4, rewardBoxes);
            AddCheckBoxes(Reward.RESOURCES, tabPage2, rewardBoxes);
            AddCheckBoxes(Reward.WEAPON_PARTS, tabPage7, rewardBoxes);
            AddCheckBoxes(new string[] { "All Helmet Blueprints", "All Weapon Skin Blueprints", "Kubrow Egg" }, tabPage6, rewardBoxes);

            this.FormClosing += Form1_Closing;

            if (Properties.Settings.Default.Planets == null)
            {
                Properties.Settings.Default.Planets = new StringCollection();
                Properties.Settings.Default.Rewards = new StringCollection();
                Properties.Settings.Default.Specifics = "";
                WriteSettings();
            }
            else
                ReadSettings();
            if (textBox2.Text.Length == 0)
                textBox2.Text = presetSpecificsText;
            else
                textBox2.ForeColor = SystemColors.ControlText;
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

        private void Specifics_Enter(object sender, EventArgs e)
        {
            textBox2.ForeColor = SystemColors.ControlText;
            if (textBox2.Text == presetSpecificsText)
                textBox2.Text = "";
        }
        private void Specifics_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.ForeColor = SystemColors.GrayText;
                textBox2.Text = presetSpecificsText;
            }
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
                foreach (string specific in textBox2.Text.Split(','))
                    if (specific.Trim().Equals(reward.name, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                string name;
                if (reward.name.EndsWith("Helmet"))
                    name = "All Helmet Blueprints";
                else if (reward.name.EndsWith("Skin"))
                    name = "All Weapon Skin Blueprints";
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
            if (Properties.Settings.Default.Specifics.Length > 0)
                textBox2.Text = Properties.Settings.Default.Specifics;
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
            if (textBox2.Text != presetSpecificsText)
                Properties.Settings.Default.Specifics = textBox2.Text;
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
