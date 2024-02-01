using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TR_SaveMaster
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            GetDirectories();

            slblStatus.Text = !string.IsNullOrEmpty(directoryTR1) ?
                $"{cmbSavegamesTR1.Items.Count} savegames found in \"{directoryTR1}\"" : "Ready";
        }

        public class Savegame
        {
            public string Path { get; set; }
            public string Name { get; set; }

            public Savegame(string filePath, string fileName)
            {
                Path = filePath;
                Name = fileName;
            }
        }

        readonly TR1Utilities TR1 = new TR1Utilities();
        readonly TR1UBUtilities TR1UB = new TR1UBUtilities();
        readonly TR2Utilities TR2 = new TR2Utilities();
        readonly TR2GUtilities TR2G = new TR2GUtilities();
        readonly TR3Utilities TR3 = new TR3Utilities();
        readonly TR3TLAUtilities TR3TLA = new TR3TLAUtilities();
        readonly TR4Utilities TR4 = new TR4Utilities();
        readonly TRCUtilities TRC = new TRCUtilities();

        private List<Savegame> savegamesTR1 = new List<Savegame>();
        private List<Savegame> savegamesTR1UB = new List<Savegame>();
        private List<Savegame> savegamesTR2 = new List<Savegame>();
        private List<Savegame> savegamesTR2G = new List<Savegame>();
        private List<Savegame> savegamesTR3 = new List<Savegame>();
        private List<Savegame> savegamesTR3TLA = new List<Savegame>();
        private List<Savegame> savegamesTR4 = new List<Savegame>();
        private List<Savegame> savegamesTRC = new List<Savegame>();

        private string directoryTR1;
        private string directoryTR1UB;
        private string directoryTR2;
        private string directoryTR2G;
        private string directoryTR3;
        private string directoryTR3TLA;
        private string directoryTR4;
        private string directoryTRC;

        private bool isLoading = false;
        private bool isFilePresent = true;

        private void GetDirectories()
        {
            string rootFolder = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(rootFolder, "TR_directories.ini");

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('=');

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (key.Equals("TR1", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR1 = value;
                            txtDirectoryTR1.Text = directoryTR1;

                            DisplaySavegamesTR1();
                        }
                        else if (key.Equals("TR1UB", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR1UB = value;
                            txtDirectoryTR1UB.Text = directoryTR1UB;

                            DisplaySavegamesTR1UB();
                        }
                        else if (key.Equals("TR2", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR2 = value;
                            txtDirectoryTR2.Text = directoryTR2;

                            DisplaySavegamesTR2();
                        }
                        else if (key.Equals("TR2G", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR2G = value;
                            txtDirectoryTR2G.Text = directoryTR2G;

                            DisplaySavegamesTR2G();
                        }
                        else if (key.Equals("TR3", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR3 = value;
                            txtDirectoryTR3.Text = directoryTR3;

                            DisplaySavegamesTR3();
                        }
                        else if (key.Equals("TR3TLA", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR3TLA = value;
                            txtDirectoryTR3TLA.Text = directoryTR3TLA;

                            DisplaySavegamesTR3TLA();
                        }
                        else if (key.Equals("TR4", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTR4 = value;
                            txtDirectoryTR4.Text = directoryTR4;

                            DisplaySavegamesTR4();
                        }
                        else if (key.Equals("TRC", StringComparison.OrdinalIgnoreCase) && Directory.Exists(value))
                        {
                            directoryTRC = value;
                            txtDirectoryTRC.Text = directoryTRC;

                            DisplaySavegamesTRC();
                        }
                        else if (key.Equals("CreateBackups", StringComparison.OrdinalIgnoreCase))
                        {
                            tsmiCreateBackups.Checked = (value == "True");
                        }
                        else if (key.Equals("StatusBar", StringComparison.OrdinalIgnoreCase))
                        {
                            if (value == "True" && !tsmiStatusBar.Checked)
                            {
                                tsmiStatusBar.Checked = true;
                                ssrStatusStrip.Visible = true;
                                slblStatus.Visible = true;
                                this.Height += ssrStatusStrip.Height;
                            }
                            else if (value == "False" && tsmiStatusBar.Checked)
                            {
                                tsmiStatusBar.Checked = false;
                                ssrStatusStrip.Visible = false;
                                slblStatus.Visible = false;
                                this.Height -= ssrStatusStrip.Height;
                            }
                        }
                        else if (key.Equals("Theme", StringComparison.OrdinalIgnoreCase))
                        {
                            if (value == "Classic")
                            {
                                tsmiClassicTheme.PerformClick();
                            }
                            else
                            {
                                tsmiModernTheme.PerformClick();
                            }
                        }
                    }
                }
            }
            else
            {
                string defaultContent =
                    $"TR1={directoryTR1}" + Environment.NewLine +
                    $"TR1UB={directoryTR1UB}" + Environment.NewLine +
                    $"TR2={directoryTR2}" + Environment.NewLine +
                    $"TR2G={directoryTR2G}" + Environment.NewLine +
                    $"TR3={directoryTR3}" + Environment.NewLine +
                    $"TR3TLA={directoryTR3TLA}" + Environment.NewLine +
                    $"TR4={directoryTR4}" + Environment.NewLine +
                    $"TRC={directoryTRC}" + Environment.NewLine +
                    $"CreateBackups={tsmiCreateBackups.Checked}" + Environment.NewLine +
                    $"StatusBar={tsmiStatusBar.Checked}" + Environment.NewLine +
                    $"Theme={(tsmiClassicTheme.Checked ? "Classic" : "Modern")}";

                File.WriteAllText(filePath, defaultContent);
            }
        }

        private void UpdateDirectories()
        {
            string rootFolder = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(rootFolder, "TR_directories.ini");

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('=');

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();

                        if (key.Equals("TR1", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR1}";
                        }
                        else if (key.Equals("TR1UB", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR1UB}";
                        }
                        else if (key.Equals("TR2", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR2}";
                        }
                        else if (key.Equals("TR2G", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR2G}";
                        }
                        else if (key.Equals("TR3", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR3}";
                        }
                        else if (key.Equals("TR3TLA", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR3TLA}";
                        }
                        else if (key.Equals("TR4", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTR4}";
                        }
                        else if (key.Equals("TRC", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={directoryTRC}";
                        }
                        else if (key.Equals("CreateBackups", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={tsmiCreateBackups.Checked}";
                        }
                        else if (key.Equals("StatusBar", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={tsmiStatusBar.Checked}";
                        }
                        else if (key.Equals("Theme", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"{key}={(tsmiClassicTheme.Checked ? "Classic" : "Modern")}";
                        }
                    }
                }

                File.WriteAllLines(filePath, lines);
            }
            else
            {
                string defaultContent =
                    $"TR1={directoryTR1}" + Environment.NewLine +
                    $"TR1UB={directoryTR1UB}" + Environment.NewLine +
                    $"TR2={directoryTR2}" + Environment.NewLine +
                    $"TR2G={directoryTR2G}" + Environment.NewLine +
                    $"TR3={directoryTR3}" + Environment.NewLine +
                    $"TR3TLA={directoryTR3TLA}" + Environment.NewLine +
                    $"TR4={directoryTR4}" + Environment.NewLine +
                    $"TRC={directoryTRC}" + Environment.NewLine +
                    $"CreateBackups={tsmiCreateBackups.Checked}" + Environment.NewLine +
                    $"StatusBar={tsmiStatusBar.Checked}" + Environment.NewLine +
                    $"Theme={(tsmiClassicTheme.Checked ? "Classic" : "Modern")}";

                File.WriteAllText(filePath, defaultContent);
            }
        }

        private void BrowseSavegames()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = "C:\\";
                folderBrowserDialog.Description = $"Select {tabGame.TabPages[tabGame.SelectedIndex].Text} directory";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    if (tabGame.SelectedIndex == 0)
                    {
                        txtDirectoryTR1.Text = folderBrowserDialog.SelectedPath;
                        directoryTR1 = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR1();

                        slblStatus.Text = $"{cmbSavegamesTR1.Items.Count} savegames found in \"{directoryTR1}\"";
                    }
                    else if (tabGame.SelectedIndex == 1)
                    {
                        txtDirectoryTR1UB.Text = folderBrowserDialog.SelectedPath;
                        directoryTR1UB = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR1UB();

                        slblStatus.Text = $"{cmbSavegamesTR1UB.Items.Count} savegames found in \"{directoryTR1UB}\"";
                    }
                    else if (tabGame.SelectedIndex == 2)
                    {
                        txtDirectoryTR2.Text = folderBrowserDialog.SelectedPath;
                        directoryTR2 = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR2();

                        slblStatus.Text = $"{cmbSavegamesTR2.Items.Count} savegames found in \"{directoryTR2}\"";
                    }
                    else if (tabGame.SelectedIndex == 3)
                    {
                        txtDirectoryTR2G.Text = folderBrowserDialog.SelectedPath;
                        directoryTR2G = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR2G();

                        slblStatus.Text = $"{cmbSavegamesTR2G.Items.Count} savegames found in \"{directoryTR2G}\"";
                    }
                    else if (tabGame.SelectedIndex == 4)
                    {
                        txtDirectoryTR3.Text = folderBrowserDialog.SelectedPath;
                        directoryTR3 = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR3();

                        slblStatus.Text = $"{cmbSavegamesTR3.Items.Count} savegames found in \"{directoryTR3}\"";
                    }
                    else if (tabGame.SelectedIndex == 5)
                    {
                        txtDirectoryTR3TLA.Text = folderBrowserDialog.SelectedPath;
                        directoryTR3TLA = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR3TLA();

                        slblStatus.Text = $"{cmbSavegamesTR3TLA.Items.Count} savegames found in \"{directoryTR3TLA}\"";
                    }
                    else if (tabGame.SelectedIndex == 6)
                    {
                        txtDirectoryTR4.Text = folderBrowserDialog.SelectedPath;
                        directoryTR4 = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTR4();

                        slblStatus.Text = $"{cmbSavegamesTR4.Items.Count} savegames found in \"{directoryTR4}\"";
                    }
                    else if (tabGame.SelectedIndex == 7)
                    {
                        txtDirectoryTRC.Text = folderBrowserDialog.SelectedPath;
                        directoryTRC = folderBrowserDialog.SelectedPath;
                        DisplaySavegamesTRC();

                        slblStatus.Text = $"{cmbSavegamesTRC.Items.Count} savegames found in \"{directoryTRC}\"";
                    }

                    UpdateDirectories();
                }
            }
        }

        private void ClearControlsInGroupBox(GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.Enabled = true;
                    numericUpDown.Value = 0;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Enabled = true;
                    checkBox.Checked = false;
                }
                else if (control is TrackBar trackBar)
                {
                    trackBar.Enabled = true;
                    trackBar.Value = 0;
                }
            }
        }

        private void ClearControlsTR1()
        {
            ClearControlsInGroupBox(grpLevelTR1);
            ClearControlsInGroupBox(grpItemsTR1);
            ClearControlsInGroupBox(grpWeaponsTR1);
            ClearControlsInGroupBox(grpHealthTR1);

            lblHealthErrorTR1.Visible = false;
            lblHealthTR1.Visible = true;
            lblHealthTR1.Text = "0.0%";
        }

        private void ClearControlsTR1UB()
        {
            ClearControlsInGroupBox(grpLevelTR1UB);
            ClearControlsInGroupBox(grpItemsTR1UB);
            ClearControlsInGroupBox(grpWeaponsTR1UB);
            ClearControlsInGroupBox(grpHealthTR1UB);

            lblHealthErrorTR1UB.Visible = false;
            lblHealthTR1UB.Visible = true;
            lblHealthTR1UB.Text = "0.0%";
        }

        private void ClearControlsTR2()
        {
            ClearControlsInGroupBox(grpLevelTR2);
            ClearControlsInGroupBox(grpItemsTR2);
            ClearControlsInGroupBox(grpWeaponsTR2);
            ClearControlsInGroupBox(grpHealthTR2);

            lblHealthErrorTR2.Visible = false;
            lblHealthTR2.Visible = true;
            lblHealthTR2.Text = "0.0%";
        }

        private void ClearControlsTR2G()
        {
            ClearControlsInGroupBox(grpLevelTR2G);
            ClearControlsInGroupBox(grpItemsTR2G);
            ClearControlsInGroupBox(grpWeaponsTR2G);
            ClearControlsInGroupBox(grpHealthTR2G);

            lblHealthErrorTR2G.Visible = false;
            lblHealthTR2G.Visible = true;
            lblHealthTR2G.Text = "0.0%";
        }

        private void ClearControlsTR3()
        {
            ClearControlsInGroupBox(grpLevelTR3);
            ClearControlsInGroupBox(grpItemsTR3);
            ClearControlsInGroupBox(grpWeaponsTR3);
            ClearControlsInGroupBox(grpHealthTR3);

            lblHealthErrorTR3.Visible = false;
            lblHealthTR3.Visible = true;
            lblHealthTR3.Text = "0.0%";
        }

        private void ClearControlsTR3TLA()
        {
            ClearControlsInGroupBox(grpLevelTR3TLA);
            ClearControlsInGroupBox(grpItemsTR3TLA);
            ClearControlsInGroupBox(grpWeaponsTR3TLA);
            ClearControlsInGroupBox(grpHealthTR3TLA);

            lblHealthErrorTR3TLA.Visible = false;
            lblHealthTR3TLA.Visible = true;
            lblHealthTR3TLA.Text = "0.0%";
        }

        private void ClearControlsTR4()
        {
            ClearControlsInGroupBox(grpLevelTR4);
            ClearControlsInGroupBox(grpItemsTR4);
            ClearControlsInGroupBox(grpWeaponsTR4);
        }

        private void ClearControlsTRC()
        {
            ClearControlsInGroupBox(grpLevelTRC);
            ClearControlsInGroupBox(grpItemsTRC);
            ClearControlsInGroupBox(grpWeaponsTRC);
            ClearControlsInGroupBox(grpHealthTRC);

            lblHealthErrorTRC.Visible = false;
            lblHealthTRC.Visible = true;
            lblHealthTRC.Text = "0.0%";
        }

        static void CreateBackup(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);

            string backupFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}{fileExtension}.bak");

            if (File.Exists(backupFilePath))
            {
                File.SetAttributes(backupFilePath, File.GetAttributes(backupFilePath) & ~FileAttributes.ReadOnly);
            }

            File.Copy(filePath, backupFilePath, true);
        }

        private void DisplaySavegamesTR1()
        {
            List<string> savegamePaths = TR1.GetSavegamePaths(directoryTR1);

            cmbSavegamesTR1.Items.Clear();
            savegamesTR1.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR1.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);

                    savegamesTR1.Add(newSavegame);
                    cmbSavegamesTR1.Items.Add(displayString);
                }
            }

            if (savegamesTR1.Count > 0)
            {
                cmbSavegamesTR1.SelectedIndex = 0;
                DisplayGameInfoTR1();
            }
            else
            {
                ClearControlsTR1();

                btnSaveTR1.Enabled = false;
                btnCancelTR1.Enabled = false;
            }
        }

        private void DisplaySavegamesTR1UB()
        {
            List<string> savegamePaths = TR1UB.GetSavegamePaths(directoryTR1UB);

            cmbSavegamesTR1UB.Items.Clear();
            savegamesTR1UB.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR1UB.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);
                    savegamesTR1UB.Add(newSavegame);

                    cmbSavegamesTR1UB.Items.Add(displayString);
                }
            }

            if (savegamesTR1UB.Count > 0)
            {
                cmbSavegamesTR1UB.SelectedIndex = 0;
                DisplayGameInfoTR1UB();
            }
            else
            {
                ClearControlsTR1UB();

                btnSaveTR1UB.Enabled = false;
                btnCancelTR1UB.Enabled = false;
            }
        }

        private void DisplaySavegamesTR2()
        {
            List<string> savegamePaths = TR2.GetSavegamePaths(directoryTR2);

            cmbSavegamesTR2.Items.Clear();
            savegamesTR2.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR2.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);

                    savegamesTR2.Add(newSavegame);
                    cmbSavegamesTR2.Items.Add(displayString);
                }
            }

            if (savegamesTR2.Count > 0)
            {
                cmbSavegamesTR2.SelectedIndex = 0;
                DisplayGameInfoTR2();
            }
            else
            {
                ClearControlsTR2();

                btnSaveTR2.Enabled = false;
                btnCancelTR2.Enabled = false;
            }
        }

        private void DisplaySavegamesTR2G()
        {
            List<string> savegamePaths = TR2G.GetSavegamePaths(directoryTR2G);

            cmbSavegamesTR2G.Items.Clear();
            savegamesTR2G.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR2G.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);
                    savegamesTR2G.Add(newSavegame);

                    cmbSavegamesTR2G.Items.Add(displayString);
                }
            }

            if (savegamesTR2G.Count > 0)
            {
                cmbSavegamesTR2G.SelectedIndex = 0;
                DisplayGameInfoTR2G();
            }
            else
            {
                ClearControlsTR2G();

                btnSaveTR2G.Enabled = false;
                btnCancelTR2G.Enabled = false;
            }
        }

        private void DisplaySavegamesTR3()
        {
            List<string> savegamePaths = TR3.GetSavegamePaths(directoryTR3);

            cmbSavegamesTR3.Items.Clear();
            savegamesTR3.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR3.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);

                    savegamesTR3.Add(newSavegame);
                    cmbSavegamesTR3.Items.Add(displayString);
                }
            }

            if (savegamesTR3.Count > 0)
            {
                cmbSavegamesTR3.SelectedIndex = 0;
                DisplayGameInfoTR3();
            }
            else
            {
                ClearControlsTR3();

                btnSaveTR3.Enabled = false;
                btnCancelTR3.Enabled = false;
            }
        }

        private void DisplaySavegamesTR3TLA()
        {
            List<string> savegamePaths = TR3TLA.GetSavegamePaths(directoryTR3TLA);

            cmbSavegamesTR3TLA.Items.Clear();
            savegamesTR3TLA.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR3TLA.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);

                    savegamesTR3TLA.Add(newSavegame);
                    cmbSavegamesTR3TLA.Items.Add(displayString);
                }
            }

            if (savegamesTR3TLA.Count > 0)
            {
                cmbSavegamesTR3TLA.SelectedIndex = 0;
                DisplayGameInfoTR3TLA();
            }
            else
            {
                ClearControlsTR3TLA();

                btnSaveTR3TLA.Enabled = false;
                btnCancelTR3TLA.Enabled = false;
            }
        }

        private void DisplaySavegamesTR4()
        {
            List<string> savegamePaths = TR4.GetSavegamePaths(directoryTR4);

            cmbSavegamesTR4.Items.Clear();
            savegamesTR4.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TR4.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);
                    savegamesTR4.Add(newSavegame);

                    cmbSavegamesTR4.Items.Add(displayString);
                }
            }

            if (savegamesTR4.Count > 0)
            {
                cmbSavegamesTR4.SelectedIndex = 0;
                DisplayGameInfoTR4();
            }
            else
            {
                ClearControlsTR4();

                btnSaveTR4.Enabled = false;
                btnCancelTR4.Enabled = false;
            }
        }

        private void DisplaySavegamesTRC()
        {
            List<string> savegamePaths = TRC.GetSavegamePaths(directoryTRC);

            cmbSavegamesTRC.Items.Clear();
            savegamesTRC.Clear();

            for (int i = 0; i < savegamePaths.Count; i++)
            {
                string displayString = Path.GetFileName(savegamePaths[i]);

                if (TRC.IsValidSavegame(savegamePaths[i]))
                {
                    Savegame newSavegame = new Savegame(savegamePaths[i], displayString);
                    savegamesTRC.Add(newSavegame);

                    cmbSavegamesTRC.Items.Add(displayString);
                }
            }

            if (savegamesTRC.Count > 0)
            {
                cmbSavegamesTRC.SelectedIndex = 0;
                DisplayGameInfoTRC();
            }
            else
            {
                ClearControlsTRC();

                btnSaveTRC.Enabled = false;
                btnCancelTRC.Enabled = false;
            }
        }

        private void DisplayGameInfoTR1()
        {
            if (cmbSavegamesTR1.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR1.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR1.Count; i++)
                {
                    if (savegamesTR1[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR1[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR1.SetSavegamePath(savegamesTR1[i].Path);
                            TR1.DetermineOffsets();

                            TR1.DisplayGameInfo(txtLvlNameTR1, chkPistolsTR1, chkShotgunTR1, chkUzisTR1, chkMagnumsTR1,
                                nudSaveNumberTR1, nudSmallMedipacksTR1, nudLargeMedipacksTR1, nudUziAmmoTR1, nudMagnumAmmoTR1,
                                nudShotgunAmmoTR1, trbHealthTR1, lblHealthTR1, lblHealthErrorTR1);

                            isLoading = false;

                            btnSaveTR1.Enabled = false;
                            btnCancelTR1.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTR1();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTR1UB()
        {
            if (cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR1UB.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR1UB.Count; i++)
                {
                    if (savegamesTR1UB[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR1UB[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR1UB.SetSavegamePath(savegamesTR1UB[i].Path);
                            TR1UB.DetermineOffsets();

                            TR1UB.DisplayGameInfo(txtLevelNameTR1UB, nudSaveNumberTR1UB, nudSmallMedipacksTR1UB, nudLargeMedipacksTR1UB,
                                nudShotgunAmmoTR1UB, nudMagnumAmmoTR1UB, nudUziAmmoTR1UB, chkPistolsTR1UB, chkMagnumsTR1UB, chkUzisTR1UB,
                                chkShotgunTR1UB, trbHealthTR1UB, lblHealthTR1UB, lblHealthErrorTR1UB);

                            isLoading = false;

                            btnSaveTR1UB.Enabled = false;
                            btnCancelTR1UB.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTRC();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTR2()
        {
            if (cmbSavegamesTR2.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR2.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR2.Count; i++)
                {
                    if (savegamesTR2[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR2[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR2.SetSavegamePath(savegamesTR2[i].Path);
                            TR2.DetermineOffsets();

                            TR2.DisplayGameInfo(txtLvlNameTR2, chkPistolsTR2, chkAutomaticPistolsTR2, chkUzisTR2, chkM16TR2,
                                chkGrenadeLauncherTR2, chkHarpoonGunTR2, nudAutomaticPistolsAmmoTR2, chkShotgunTR2, nudUziAmmoTR2,
                                nudM16AmmoTR2, nudGrenadeLauncherAmmoTR2, nudHarpoonGunAmmoTR2, nudShotgunAmmoTR2, nudSaveNumberTR2,
                                nudFlaresTR2, nudSmallMedipacksTR2, nudLargeMedipacksTR2, trbHealthTR2, lblHealthTR2, lblHealthErrorTR2);

                            isLoading = false;

                            btnSaveTR2.Enabled = false;
                            btnCancelTR2.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTR2();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTR2G()
        {
            if (cmbSavegamesTR2G.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR2G.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR2G.Count; i++)
                {
                    if (savegamesTR2G[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR2G[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR2G.SetSavegamePath(savegamesTR2G[i].Path);
                            TR2G.DetermineOffsets();

                            TR2G.DisplayGameInfo(txtLvlNameTR2G, chkPistolsTR2G, chkAutomaticPistolsTR2G, chkUzisTR2G,
                                chkM16TR2G, chkGrenadeLauncherTR2G, chkHarpoonGunTR2G, nudAutomaticPistolsAmmoTR2G,
                                chkShotgunTR2G, nudUziAmmoTR2G, nudM16AmmoTR2G, nudGrenadeLauncherAmmoTR2G,
                                nudHarpoonGunAmmoTR2G, nudShotgunAmmoTR2G, nudSaveNumberTR2G, nudFlaresTR2G,
                                nudSmallMedipacksTR2G, nudLargeMedipacksTR2G, trbHealthTR2G, lblHealthTR2G, lblHealthErrorTR2G);

                            isLoading = false;

                            btnSaveTR2G.Enabled = false;
                            btnCancelTR2G.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTR2G();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTR3()
        {
            if (cmbSavegamesTR3.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR3.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR3.Count; i++)
                {
                    if (savegamesTR3[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR3[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR3.SetSavegamePath(savegamesTR3[i].Path);
                            TR3.DetermineOffsets();

                            TR3.DisplayGameInfo(chkPistolsTR3, chkShotgunTR3, chkDeagleTR3, chkUziTR3, chkMP5TR3, chkRocketLauncherTR3,
                                chkGrenadeLauncherTR3, chkHarpoonGunTR3, txtLvlNameTR3, nudSaveNumberTR3, nudSmallMedipacksTR3,
                                nudLargeMedipacksTR3, nudFlaresTR3, nudShotgunAmmoTR3, nudDeagleAmmoTR3, nudGrenadeLauncherAmmoTR3,
                                nudRocketLauncherAmmoTR3, nudHarpoonGunAmmoTR3, nudMP5AmmoTR3, nudUziAmmoTR3, trbHealthTR3, lblHealthTR3,
                                lblHealthErrorTR3);

                            isLoading = false;

                            btnSaveTR3.Enabled = false;
                            btnCancelTR3.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTR3();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTR3TLA()
        {
            if (cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR3TLA.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR3TLA.Count; i++)
                {
                    if (savegamesTR3TLA[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR3TLA[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR3TLA.SetSavegamePath(savegamesTR3TLA[i].Path);
                            TR3TLA.DetermineOffsets();


                            TR3TLA.DisplayGameInfo(chkPistolsTR3TLA, chkShotgunTR3TLA, chkDeagleTR3TLA, chkUzisTR3TLA,
                                chkMP5TR3TLA, chkRocketLauncherTR3TLA, chkGrenadeLauncherTR3TLA, chkHarpoonGunTR3TLA, txtLvlNameTR3TLA,
                                nudSaveNumberTR3TLA, nudSmallMedipacksTR3TLA, nudLargeMedipacksTR3TLA, nudFlaresTR3TLA, nudShotgunAmmoTR3TLA,
                                nudDeagleAmmoTR3TLA, nudGrenadeLauncherAmmoTR3TLA, nudRocketLauncherAmmoTR3TLA, nudHarpoonGunAmmoTR3TLA,
                                nudMP5AmmoTR3TLA, nudUziAmmoTR3TLA, trbHealthTR3TLA, lblHealthTR3TLA, lblHealthErrorTR3TLA);


                            isLoading = false;

                            btnSaveTR3TLA.Enabled = false;
                            btnCancelTR3TLA.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTR3TLA();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTR4()
        {
            if (cmbSavegamesTR4.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR4.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR4.Count; i++)
                {
                    if (savegamesTR4[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR4[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TR4.SetSavegamePath(savegamesTR4[i].Path);

                            TR4.SetLevelParams(chkBinocularsTR4, chkLaserSightTR4, chkCrowbarTR4);

                            TR4.DisplayGameInfo(txtLvlNameTR4, nudSaveNumberTR4, nudSecretsTR4, nudFlaresTR4, nudSmallMedipacksTR4,
                                nudLargeMedipacksTR4, nudRevolverAmmoTR4, nudUziAmmoTR4, nudShotgunNormalAmmoTR4,
                                nudShotgunWideshotAmmoTR4, nudGrenadeGunNormalAmmoTR4, nudGrenadeGunSuperAmmoTR4,
                                nudGrenadeGunFlashAmmoTR4, nudCrossbowNormalAmmoTR4, nudCrossbowPoisonAmmoTR4,
                                nudCrossbowExplosiveAmmoTR4, chkBinocularsTR4, chkCrowbarTR4, chkLaserSightTR4,
                                chkPistolsTR4, chkRevolverTR4, chkUziTR4, chkShotgunTR4, chkCrossbowTR4, chkGrenadeGunTR4);

                            isLoading = false;

                            btnSaveTR4.Enabled = false;
                            btnCancelTR4.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTR4();
                        }

                        break;
                    }
                }
            }
        }

        private void DisplayGameInfoTRC()
        {
            if (cmbSavegamesTRC.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTRC.SelectedItem.ToString();

                for (int i = 0; i < savegamesTRC.Count; i++)
                {
                    if (savegamesTRC[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTRC[i].Path))
                        {
                            isFilePresent = true;
                            isLoading = true;

                            TRC.SetSavegamePath(savegamesTRC[i].Path);

                            TRC.SetLevelParams(chkRevolverTRC, chkDeagleTRC, nudRevolverAmmoTRC, nudDeagleAmmoTRC, chkUziTRC,
                                nudUziAmmoTRC, chkShotgunTRC, nudShotgunNormalAmmoTRC, nudShotgunWideshotAmmoTRC, chkGrapplingGunTRC,
                                nudGrapplingGunAmmoTRC, chkHkGunTRC, nudHkAmmoTRC, chkCrowbarTRC, chkPistolsTRC, nudFlaresTRC,
                                chkLaserSightTRC, chkBinocularsOrHeadsetTRC);

                            TRC.DisplayGameInfo(txtLvlNameTRC, nudSmallMedipacksTRC, nudLargeMedipacksTRC, nudFlaresTRC, nudHkAmmoTRC,
                                nudSecretsTRC, nudSaveNumberTRC, nudShotgunNormalAmmoTRC, nudShotgunWideshotAmmoTRC, nudUziAmmoTRC,
                                nudGrapplingGunAmmoTRC, nudRevolverAmmoTRC, nudDeagleAmmoTRC, chkPistolsTRC, chkBinocularsOrHeadsetTRC,
                                chkLaserSightTRC, chkCrowbarTRC, chkRevolverTRC, chkDeagleTRC, chkShotgunTRC, chkUziTRC, chkHkGunTRC,
                                chkGrapplingGunTRC, trbHealthTRC, lblHealthTRC, lblHealthErrorTRC);

                            isLoading = false;

                            btnSaveTRC.Enabled = false;
                            btnCancelTRC.Enabled = false;
                            tsmiSave.Enabled = false;

                            slblStatus.Text = $"Successfully loaded save file: {selectedDisplayString}";
                        }
                        else
                        {
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isFilePresent = false;
                            DisplaySavegamesTRC();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR1()
        {
            if (cmbSavegamesTR1.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR1.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR1.Count; i++)
                {
                    if (savegamesTR1[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR1[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR1[i].Path, File.GetAttributes(savegamesTR1[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR1[i].Path);
                                }

                                TR1.WriteChanges(chkPistolsTR1, chkMagnumsTR1, chkUzisTR1, chkShotgunTR1, nudSaveNumberTR1,
                                    nudSmallMedipacksTR1, nudLargeMedipacksTR1, nudUziAmmoTR1, nudMagnumAmmoTR1,
                                    nudShotgunAmmoTR1, trbHealthTR1);

                                btnSaveTR1.Enabled = false;
                                btnCancelTR1.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR1();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR1UB()
        {
            if (cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR1UB.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR1UB.Count; i++)
                {
                    if (savegamesTR1UB[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR1UB[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR1UB[i].Path, File.GetAttributes(savegamesTR1UB[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR1UB[i].Path);
                                }

                                TR1UB.WriteChanges(nudSaveNumberTR1UB, nudSmallMedipacksTR1UB, nudLargeMedipacksTR1UB, nudShotgunAmmoTR1UB,
                                    nudMagnumAmmoTR1UB, nudUziAmmoTR1UB, chkPistolsTR1UB, chkMagnumsTR1UB, chkUzisTR1UB, chkShotgunTR1UB,
                                    trbHealthTR1UB);

                                btnCancelTR1UB.Enabled = false;
                                btnSaveTR1UB.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR1UB();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR2()
        {
            if (cmbSavegamesTR2.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR2.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR2.Count; i++)
                {
                    if (savegamesTR2[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR2[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR2[i].Path, File.GetAttributes(savegamesTR2[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR2[i].Path);
                                }

                                TR2.WriteChanges(chkPistolsTR2, chkAutomaticPistolsTR2, chkUzisTR2, chkShotgunTR2, chkM16TR2,
                                    chkGrenadeLauncherTR2, chkHarpoonGunTR2, nudSaveNumberTR2, nudFlaresTR2, nudSmallMedipacksTR2,
                                    nudLargeMedipacksTR2, nudAutomaticPistolsAmmoTR2, nudUziAmmoTR2, nudM16AmmoTR2,
                                    nudGrenadeLauncherAmmoTR2, nudHarpoonGunAmmoTR2, nudShotgunAmmoTR2, trbHealthTR2);

                                btnSaveTR2.Enabled = false;
                                btnCancelTR2.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR2();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR2G()
        {
            if (cmbSavegamesTR2G.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR2G.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR2G.Count; i++)
                {
                    if (savegamesTR2G[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR2G[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR2G[i].Path, File.GetAttributes(savegamesTR2G[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR2G[i].Path);
                                }

                                TR2G.WriteChanges(chkPistolsTR2G, chkAutomaticPistolsTR2G, chkUzisTR2G, chkShotgunTR2G, chkM16TR2G,
                                    chkGrenadeLauncherTR2G, chkHarpoonGunTR2G, nudSaveNumberTR2G, nudFlaresTR2G, nudSmallMedipacksTR2G,
                                    nudLargeMedipacksTR2G, nudAutomaticPistolsAmmoTR2G, nudUziAmmoTR2G, nudM16AmmoTR2G, nudGrenadeLauncherAmmoTR2G,
                                    nudHarpoonGunAmmoTR2G, nudShotgunAmmoTR2G, trbHealthTR2G);

                                btnCancelTR2G.Enabled = false;
                                btnSaveTR2G.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR2G();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR3()
        {
            if (cmbSavegamesTR3.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR3.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR3.Count; i++)
                {
                    if (savegamesTR3[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR3[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR3[i].Path, File.GetAttributes(savegamesTR3[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR3[i].Path);
                                }

                                TR3.WriteChanges(chkPistolsTR3, chkDeagleTR3, chkUziTR3, chkShotgunTR3, chkMP5TR3, chkRocketLauncherTR3,
                                    chkGrenadeLauncherTR3, chkHarpoonGunTR3, nudFlaresTR3, nudSmallMedipacksTR3, nudLargeMedipacksTR3,
                                    nudSaveNumberTR3, nudShotgunAmmoTR3, nudDeagleAmmoTR3, nudGrenadeLauncherAmmoTR3,
                                    nudRocketLauncherAmmoTR3, nudHarpoonGunAmmoTR3, nudMP5AmmoTR3, nudUziAmmoTR3, trbHealthTR3);

                                btnCancelTR3.Enabled = false;
                                btnSaveTR3.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR3();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR3TLA()
        {
            if (cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR3TLA.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR3TLA.Count; i++)
                {
                    if (savegamesTR3TLA[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR3TLA[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR3TLA[i].Path, File.GetAttributes(savegamesTR3TLA[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR3TLA[i].Path);
                                }

                                TR3TLA.WriteChanges(chkPistolsTR3TLA, chkDeagleTR3TLA, chkUzisTR3TLA, chkShotgunTR3TLA,
                                    chkMP5TR3TLA, chkRocketLauncherTR3TLA, chkGrenadeLauncherTR3TLA, chkHarpoonGunTR3TLA,
                                    nudFlaresTR3TLA, nudSmallMedipacksTR3TLA, nudLargeMedipacksTR3TLA, nudSaveNumberTR3TLA,
                                    nudShotgunAmmoTR3TLA, nudDeagleAmmoTR3TLA, nudGrenadeLauncherAmmoTR3TLA, nudRocketLauncherAmmoTR3TLA,
                                    nudHarpoonGunAmmoTR3TLA, nudMP5AmmoTR3TLA, nudUziAmmoTR3TLA, trbHealthTR3TLA);

                                btnCancelTR3TLA.Enabled = false;
                                btnSaveTR3TLA.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR3TLA();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTR4()
        {
            if (cmbSavegamesTR4.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTR4.SelectedItem.ToString();

                for (int i = 0; i < savegamesTR4.Count; i++)
                {
                    if (savegamesTR4[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTR4[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTR4[i].Path, File.GetAttributes(savegamesTR4[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTR4[i].Path);
                                }

                                TR4.WriteChanges(nudSaveNumberTR4, nudSecretsTR4, nudFlaresTR4, nudSmallMedipacksTR4,
                                    nudLargeMedipacksTR4, nudRevolverAmmoTR4, nudUziAmmoTR4, nudGrenadeGunNormalAmmoTR4,
                                    nudGrenadeGunSuperAmmoTR4, nudGrenadeGunFlashAmmoTR4, nudCrossbowNormalAmmoTR4, nudCrossbowPoisonAmmoTR4,
                                    nudCrossbowExplosiveAmmoTR4, nudShotgunNormalAmmoTR4, nudShotgunWideshotAmmoTR4,
                                    chkPistolsTR4, chkUziTR4, chkShotgunTR4, chkCrossbowTR4, chkGrenadeGunTR4,
                                    chkRevolverTR4, chkBinocularsTR4, chkCrowbarTR4, chkLaserSightTR4);

                                btnSaveTR4.Enabled = false;
                                btnCancelTR4.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTR4();
                        }

                        break;
                    }
                }
            }
        }

        private void WriteChangesTRC()
        {
            if (cmbSavegamesTRC.SelectedIndex != -1)
            {
                string selectedDisplayString = cmbSavegamesTRC.SelectedItem.ToString();

                for (int i = 0; i < savegamesTRC.Count; i++)
                {
                    if (savegamesTRC[i].Name.Equals(selectedDisplayString))
                    {
                        if (File.Exists(savegamesTRC[i].Path))
                        {
                            try
                            {
                                File.SetAttributes(savegamesTRC[i].Path, File.GetAttributes(savegamesTRC[i].Path) & ~FileAttributes.ReadOnly);

                                if (tsmiCreateBackups.Checked)
                                {
                                    CreateBackup(savegamesTRC[i].Path);
                                }

                                TRC.WriteChanges(nudSaveNumberTRC, nudSecretsTRC, nudSmallMedipacksTRC,
                                    nudLargeMedipacksTRC, nudFlaresTRC, nudRevolverAmmoTRC, nudDeagleAmmoTRC, nudUziAmmoTRC,
                                    nudHkAmmoTRC, nudGrapplingGunAmmoTRC, nudShotgunNormalAmmoTRC,
                                    nudShotgunWideshotAmmoTRC, chkPistolsTRC, chkUziTRC, chkRevolverTRC,
                                    chkDeagleTRC, chkShotgunTRC, chkHkGunTRC, chkGrapplingGunTRC, chkBinocularsOrHeadsetTRC,
                                    chkCrowbarTRC, chkLaserSightTRC, trbHealthTRC);

                                btnCancelTRC.Enabled = false;
                                btnSaveTRC.Enabled = false;
                                tsmiSave.Enabled = false;

                                slblStatus.Text = $"Successfully patched save file: {selectedDisplayString}";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                slblStatus.Text = $"Error writing to save file: {selectedDisplayString}";
                            }
                        }
                        else
                        {
                            isFilePresent = false;
                            MessageBox.Show("Could not find file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DisplaySavegamesTRC();
                        }

                        break;
                    }
                }
            }
        }

        private void cmbSavegamesTR1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR1.SelectedIndex != -1)
            {
                if (btnCancelTR1.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show($"Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR1();
                    }
                }

                DisplayGameInfoTR1();
            }
        }

        private void cmbSavegamesTR1UB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                if (btnCancelTR1UB.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR1UB();
                    }
                }

                DisplayGameInfoTR1UB();
            }
        }

        private void cmbSavegamesTR2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR2.SelectedIndex != -1)
            {
                if (btnCancelTR2.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR2();
                    }
                }

                DisplayGameInfoTR2();
            }
        }

        private void cmbSavegamesTR2G_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR2G.SelectedIndex != -1)
            {
                if (btnCancelTR2G.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR2G();
                    }
                }

                DisplayGameInfoTR2G();
            }
        }

        private void cmbSavegamesTR3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR3.SelectedIndex != -1)
            {
                if (btnCancelTR3.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR3();
                    }
                }

                DisplayGameInfoTR3();
            }
        }

        private void cmbSavegamesTR3TLA_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                if (btnCancelTR3TLA.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR3TLA();
                    }
                }

                DisplayGameInfoTR3TLA();
            }
        }

        private void cmbSavegamesTR4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTR4.SelectedIndex != -1)
            {
                if (btnCancelTR4.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTR4();
                    }
                }

                DisplayGameInfoTR4();
            }
        }

        private void cmbSavegamesTRC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavegamesTRC.SelectedIndex != -1)
            {
                if (btnCancelTRC.Enabled && isFilePresent)
                {
                    DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                        "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        WriteChangesTRC();
                    }
                }

                DisplayGameInfoTRC();
            }
        }

        private void EnableButtonsTR1()
        {
            btnCancelTR1.Enabled = true;
            btnSaveTR1.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTR1UB()
        {
            btnCancelTR1UB.Enabled = true;
            btnSaveTR1UB.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTR2()
        {
            btnCancelTR2.Enabled = true;
            btnSaveTR2.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTR2G()
        {
            btnCancelTR2G.Enabled = true;
            btnSaveTR2G.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTR3()
        {
            btnCancelTR3.Enabled = true;
            btnSaveTR3.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTR3TLA()
        {
            btnCancelTR3TLA.Enabled = true;
            btnSaveTR3TLA.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTR4()
        {
            btnCancelTR4.Enabled = true;
            btnSaveTR4.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void EnableButtonsTRC()
        {
            btnCancelTRC.Enabled = true;
            btnSaveTRC.Enabled = true;
            tsmiSave.Enabled = true;
        }

        private void trbHealthTR1_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTR1.Value;
            lblHealthTR1.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void trbHealthTR1UB_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTR1UB.Value;
            lblHealthTR1UB.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void trbHealthTR2_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTR2.Value;
            lblHealthTR2.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void trbHealthTR2G_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTR2G.Value;
            lblHealthTR2G.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void trbHealthTR3_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTR3.Value;
            lblHealthTR3.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void trbHealthTR3TLA_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTR3TLA.Value;
            lblHealthTR3TLA.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void trbHealthTRC_Scroll(object sender, EventArgs e)
        {
            double healthPercentage = (double)trbHealthTRC.Value;
            lblHealthTRC.Text = healthPercentage.ToString("0.0") + "%";

            if (cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void btnCancelTR1_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR1.SelectedIndex != -1)
            {
                DisplayGameInfoTR1();
            }
        }

        private void btnCancelTR1UB_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                DisplayGameInfoTR1UB();
            }
        }

        private void btnCancelTR2_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR2.SelectedIndex != -1)
            {
                DisplayGameInfoTR2();
            }
        }

        private void btnCancelTR2G_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR2G.SelectedIndex != -1)
            {
                DisplayGameInfoTR2G();
            }
        }

        private void btnCancelTR3_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR3.SelectedIndex != -1)
            {
                DisplayGameInfoTR3();
            }
        }

        private void btnCancelTR3TLA_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                DisplayGameInfoTR3TLA();
            }
        }

        private void btnCancelTR4_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTR4.SelectedIndex != -1)
            {
                DisplayGameInfoTR4();
            }
        }

        private void btnCancelTRC_Click(object sender, EventArgs e)
        {
            if (cmbSavegamesTRC.SelectedIndex != -1)
            {
                DisplayGameInfoTRC();
            }
        }

        private void btnBrowseTR1_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTR1UB_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTR2_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTR2G_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTR3_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTR3TLA_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTR4_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnBrowseTRC_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnExitTR1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTR1UB_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTR2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTR2G_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTR3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTR3TLA_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTR4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExitTRC_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tsmiBrowseDirectory_Click(object sender, EventArgs e)
        {
            BrowseSavegames();
        }

        private void btnSaveTR1_Click(object sender, EventArgs e)
        {
            WriteChangesTR1();
        }

        private void btnSaveTR1UB_Click(object sender, EventArgs e)
        {
            WriteChangesTR1UB();
        }

        private void btnSaveTR2_Click(object sender, EventArgs e)
        {
            WriteChangesTR2();
        }

        private void btnSaveTR2G_Click(object sender, EventArgs e)
        {
            WriteChangesTR2G();
        }

        private void btnSaveTR3_Click(object sender, EventArgs e)
        {
            WriteChangesTR3();
        }

        private void btnSaveTR3TLA_Click(object sender, EventArgs e)
        {
            WriteChangesTR3TLA();
        }

        private void btnSaveTR4_Click(object sender, EventArgs e)
        {
            WriteChangesTR4();
        }

        private void btnSaveTRC_Click(object sender, EventArgs e)
        {
            WriteChangesTRC();
        }

        private void chkBinocularsTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkCrowbarTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkLaserSightTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkPistolsTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkShotgunTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkCrossbowTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkGrenadeGunTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkRevolverTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkUziTR4_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudSaveNumberTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudSmallMedipacksTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudLargeMedipacksTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudFlaresTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudShotgunNormalAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudCrossbowNormalAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudGrenadeGunNormalAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudRevolverAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudUziAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudShotgunWideshotAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudCrossbowPoisonAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudCrossbowExplosiveAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudGrenadeGunSuperAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudGrenadeGunFlashAmmoTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudSecretsTR4_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudSecretsTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudGrenadeGunFlashAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudGrenadeGunSuperAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudCrossbowExplosiveAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudCrossbowPoisonAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudSaveNumberTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudShotgunWideshotAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudSmallMedipacksTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudLargeMedipacksTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudFlaresTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudShotgunNormalAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudCrossbowNormalAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudGrenadeGunNormalAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudRevolverAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void nudUziAmmoTR4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR4.SelectedIndex != -1)
            {
                EnableButtonsTR4();
            }
        }

        private void chkHkGunTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkUziTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkRevolverTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkGrapplingGunTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkPistolsTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkCrowbarTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkShotgunTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkDeagleTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkLaserSightTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void chkBinocularsOrHeadsetTRC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSaveNumberTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSecretsTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSmallMedipacksTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudLargeMedipacksTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudFlaresTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudRevolverAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudDeagleAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudUziAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudHkAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudGrapplingGunAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudShotgunNormalAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudShotgunWideshotAmmoTRC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSaveNumberTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSecretsTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSmallMedipacksTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudLargeMedipacksTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudFlaresTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudRevolverAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudDeagleAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudUziAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudHkAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudGrapplingGunAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudShotgunNormalAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudShotgunWideshotAmmoTRC_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTRC.SelectedIndex != -1)
            {
                EnableButtonsTRC();
            }
        }

        private void nudSaveNumberTR1_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudSmallMedipacksTR1_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudLargeMedipacksTR1_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudUziAmmoTR1_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudMagnumAmmoTR1_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudShotgunAmmoTR1_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudSaveNumberTR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudSmallMedipacksTR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudLargeMedipacksTR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudUziAmmoTR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudMagnumAmmoTR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudShotgunAmmoTR1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void chkPistolsTR1_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void chkUzisTR1_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void chkMagnumsTR1_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void chkShotgunTR1_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1.SelectedIndex != -1)
            {
                EnableButtonsTR1();
            }
        }

        private void nudSaveNumberTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudSmallMedipacksTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudLargeMedipacksTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudFlaresTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudShotgunAmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudDeagleAmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudGrenadeLauncherAmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudRocketLauncherAmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudHarpoonGunAmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudMP5AmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudUziAmmoTR3_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkPistolsTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkShotgunTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkDeagleTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkGrenadeLauncherTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkRocketLauncherTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkHarpoonGunTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkMP5TR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkUziTR3_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudSaveNumberTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudSmallMedipacksTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudLargeMedipacksTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudFlaresTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudShotgunAmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudDeagleAmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudGrenadeLauncherAmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudRocketLauncherAmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudHarpoonGunAmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudMP5AmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void nudUziAmmoTR3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3.SelectedIndex != -1)
            {
                EnableButtonsTR3();
            }
        }

        private void chkPistolsTR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkShotgunTR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkAutomaticPistolsTR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkGrenadeLauncherTR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkHarpoonGunTR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkM16TR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkUzisTR2_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudSaveNumberTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudShotgunAmmoTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudAutomaticPistolsAmmoTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudGrenadeLauncherAmmoTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudHarpoonGunAmmoTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudM16AmmoTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudUziAmmoTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudSmallMedipacksTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudLargeMedipacksTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudFlaresTR2_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudSaveNumberTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudSmallMedipacksTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudLargeMedipacksTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudFlaresTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudShotgunAmmoTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudAutomaticPistolsAmmoTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudGrenadeLauncherAmmoTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudHarpoonGunAmmoTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudM16AmmoTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void nudUziAmmoTR2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2.SelectedIndex != -1)
            {
                EnableButtonsTR2();
            }
        }

        private void chkPistolsTR1UB_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void chkShotgunTR1UB_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void chkMagnumsTR1UB_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void chkUzisTR1UB_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudSaveNumberTR1UB_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudSmallMedipacksTR1UB_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudLargeMedipacksTR1UB_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudShotgunAmmoTR1UB_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudMagnumAmmoTR1UB_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudUziAmmoTR1UB_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudSaveNumberTR1UB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudSmallMedipacksTR1UB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudLargeMedipacksTR1UB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudShotgunAmmoTR1UB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudMagnumAmmoTR1UB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void nudUziAmmoTR1UB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                EnableButtonsTR1UB();
            }
        }

        private void chkPistolsTR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkShotgunTR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkAutomaticPistolsTR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkUzisTR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkM16TR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkGrenadeLauncherTR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkHarpoonGunTR2G_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudSaveNumberTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudSmallMedipacksTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudLargeMedipacksTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudFlaresTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudShotgunAmmoTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudAutomaticPistolsAmmoTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudUziAmmoTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudM16AmmoTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudGrenadeLauncherAmmoTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudHarpoonGunAmmoTR2G_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudSaveNumberTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudSmallMedipacksTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudLargeMedipacksTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudFlaresTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudShotgunAmmoTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudAutomaticPistolsAmmoTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudUziAmmoTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudM16AmmoTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudGrenadeLauncherAmmoTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void nudHarpoonGunAmmoTR2G_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                EnableButtonsTR2G();
            }
        }

        private void chkPistolsTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkShotgunTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkDeagleTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkGrenadeLauncherTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkRocketLauncherTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkHarpoonGunTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkMP5TR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void chkUzisTR3TLA_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudSaveNumberTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudSmallMedipacksTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudLargeMedipacksTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudFlaresTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudShotgunAmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudDeagleAmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudGrenadeLauncherAmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudRocketLauncherAmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudHarpoonGunAmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudMP5AmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudUziAmmoTR3TLA_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudSaveNumberTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudSmallMedipacksTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudLargeMedipacksTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudFlaresTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudShotgunAmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudDeagleAmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudGrenadeLauncherAmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudRocketLauncherAmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudHarpoonGunAmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudMP5AmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void nudUziAmmoTR3TLA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                EnableButtonsTR3TLA();
            }
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tsmiAlwaysOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = tsmiAlwaysOnTop.Checked;
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            if (tabGame.SelectedIndex == 0 && cmbSavegamesTR1.SelectedIndex != -1)
            {
                WriteChangesTR1();
            }
            else if (tabGame.SelectedIndex == 1 && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                WriteChangesTR1UB();
            }
            else if (tabGame.SelectedIndex == 2 && cmbSavegamesTR2.SelectedIndex != -1)
            {
                WriteChangesTR2();
            }
            else if (tabGame.SelectedIndex == 3 && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                WriteChangesTR2G();
            }
            else if (tabGame.SelectedIndex == 4 && cmbSavegamesTR3.SelectedIndex != -1)
            {
                WriteChangesTR3();
            }
            else if (tabGame.SelectedIndex == 5 && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                WriteChangesTR3TLA();
            }
            else if (tabGame.SelectedIndex == 6 && cmbSavegamesTR4.SelectedIndex != -1)
            {
                WriteChangesTR4();
            }
            else if (tabGame.SelectedIndex == 7 && cmbSavegamesTRC.SelectedIndex != -1)
            {
                WriteChangesTRC();
            }
        }

        private void UpdateSaveButtonState()
        {
            if (tabGame.SelectedIndex == 0)
            {
                tsmiSave.Enabled = btnSaveTR1.Enabled;
            }
            else if (tabGame.SelectedIndex == 1)
            {
                tsmiSave.Enabled = btnSaveTR1UB.Enabled;
            }
            else if (tabGame.SelectedIndex == 2)
            {
                tsmiSave.Enabled = btnSaveTR2.Enabled;
            }
            else if (tabGame.SelectedIndex == 3)
            {
                tsmiSave.Enabled = btnSaveTR2G.Enabled;
            }
            else if (tabGame.SelectedIndex == 4)
            {
                tsmiSave.Enabled = btnSaveTR3.Enabled;
            }
            else if (tabGame.SelectedIndex == 5)
            {
                tsmiSave.Enabled = btnSaveTR3TLA.Enabled;
            }
            else if (tabGame.SelectedIndex == 6)
            {
                tsmiSave.Enabled = btnSaveTR4.Enabled;
            }
            else if (tabGame.SelectedIndex == 7)
            {
                tsmiSave.Enabled = btnSaveTRC.Enabled;
            }
        }

        private void UpdateSavegameInfo()
        {
            if (tabGame.SelectedIndex == 0 && cmbSavegamesTR1.SelectedIndex != -1)
            {
                DisplayGameInfoTR1();
            }
            else if (tabGame.SelectedIndex == 1 && cmbSavegamesTR1UB.SelectedIndex != -1)
            {
                DisplayGameInfoTR1UB();
            }
            else if (tabGame.SelectedIndex == 2 && cmbSavegamesTR2.SelectedIndex != -1)
            {
                DisplayGameInfoTR2();
            }
            else if (tabGame.SelectedIndex == 3 && cmbSavegamesTR2G.SelectedIndex != -1)
            {
                DisplayGameInfoTR2G();
            }
            else if (tabGame.SelectedIndex == 4 && cmbSavegamesTR3.SelectedIndex != -1)
            {
                DisplayGameInfoTR3();
            }
            else if (tabGame.SelectedIndex == 5 && cmbSavegamesTR3TLA.SelectedIndex != -1)
            {
                DisplayGameInfoTR3TLA();
            }
            else if (tabGame.SelectedIndex == 6 && cmbSavegamesTR4.SelectedIndex != -1)
            {
                DisplayGameInfoTR4();
            }
            else if (tabGame.SelectedIndex == 7 && cmbSavegamesTRC.SelectedIndex != -1)
            {
                DisplayGameInfoTRC();
            }
        }

        private void tabGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < tabGame.TabPages.Count; i++)
            {
                if (tabGame.TabPages[i] == tabGame.SelectedTab)
                {
                    for (int j = 0; j < tabGame.TabPages[i].Controls.Count; j++)
                    {
                        tabGame.TabPages[i].Controls[j].Visible = true;
                    }
                }
                else
                {
                    for (int j = 0; j < tabGame.TabPages[i].Controls.Count; j++)
                    {
                        tabGame.TabPages[i].Controls[j].Visible = false;
                    }
                }
            }

            UpdateSaveButtonState();
            UpdateSavegameInfo();
        }

        private void tabGame_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabGame.SelectedIndex == 0 && cmbSavegamesTR1.SelectedIndex != -1 && btnCancelTR1.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR1();
                }
                else
                {
                    DisplayGameInfoTR1();
                }
            }
            else if (tabGame.SelectedIndex == 1 && cmbSavegamesTR1UB.SelectedIndex != -1 && btnCancelTR1UB.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR1UB();
                }
                else
                {
                    DisplayGameInfoTR1UB();
                }
            }
            else if (tabGame.SelectedIndex == 2 && cmbSavegamesTR2.SelectedIndex != -1 && btnCancelTR2.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR2();
                }
                else
                {
                    DisplayGameInfoTR2();
                }
            }
            else if (tabGame.SelectedIndex == 3 && cmbSavegamesTR2G.SelectedIndex != -1 && btnCancelTR2G.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR2G();
                }
                else
                {
                    DisplayGameInfoTR2G();
                }
            }
            else if (tabGame.SelectedIndex == 4 && cmbSavegamesTR3.SelectedIndex != -1 && btnCancelTR3.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR3();
                }
                else
                {
                    DisplayGameInfoTR3();
                }
            }
            else if (tabGame.SelectedIndex == 5 && cmbSavegamesTR3TLA.SelectedIndex != -1 && btnCancelTR3TLA.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR3TLA();
                }
                else
                {
                    DisplayGameInfoTR3TLA();
                }
            }
            else if (tabGame.SelectedIndex == 6 && cmbSavegamesTR4.SelectedIndex != -1 && btnCancelTR4.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR4();
                }
                else
                {
                    DisplayGameInfoTR4();
                }
            }
            else if (tabGame.SelectedIndex == 7 && cmbSavegamesTRC.SelectedIndex != -1 && btnCancelTRC.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTRC();
                }
                else
                {
                    DisplayGameInfoTRC();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tabGame.SelectedIndex == 0 && cmbSavegamesTR1.SelectedIndex != -1 && btnCancelTR1.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR1();
                }
            }
            else if (tabGame.SelectedIndex == 1 && cmbSavegamesTR1UB.SelectedIndex != -1 && btnCancelTR1UB.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR1UB();
                }
            }
            else if (tabGame.SelectedIndex == 2 && cmbSavegamesTR2.SelectedIndex != -1 && btnCancelTR2.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR2();
                }
            }
            else if (tabGame.SelectedIndex == 3 && cmbSavegamesTR2G.SelectedIndex != -1 && btnCancelTR2G.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR2G();
                }
            }
            else if (tabGame.SelectedIndex == 4 && cmbSavegamesTR3.SelectedIndex != -1 && btnCancelTR3.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR3();
                }
            }
            else if (tabGame.SelectedIndex == 5 && cmbSavegamesTR3TLA.SelectedIndex != -1 && btnCancelTR3TLA.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR3TLA();
                }
            }
            else if (tabGame.SelectedIndex == 6 && cmbSavegamesTR4.SelectedIndex != -1 && btnCancelTR4.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTR4();
                }
            }
            else if (tabGame.SelectedIndex == 7 && cmbSavegamesTRC.SelectedIndex != -1 && btnCancelTRC.Enabled)
            {
                DialogResult result = MessageBox.Show("Would you like to apply changes to the savegame file?",
                    "Tomb Raider - Savegame Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    WriteChangesTRC();
                }
            }

            UpdateDirectories();
        }

        private void tsmiStatusBar_Click(object sender, EventArgs e)
        {
            if (tsmiStatusBar.Checked)
            {
                ssrStatusStrip.Visible = true;
                slblStatus.Visible = true;
                this.Height += ssrStatusStrip.Height;
            }
            else
            {
                ssrStatusStrip.Visible = false;
                slblStatus.Visible = false;
                this.Height -= ssrStatusStrip.Height;
            }
        }

        private void tsmiResources_Click(object sender, EventArgs e)
        {
            ResourcesForm resourcesForm = new ResourcesForm();
            resourcesForm.ShowDialog();
        }

        private void tsmiViewReadme_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/JulianOzelRose/TR-SaveMaster/blob/master/README.md");
        }

        private void tsmiSendFeedback_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/JulianOzelRose/TR-SaveMaster/issues");
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void tsmiModernTheme_Click(object sender, EventArgs e)
        {
            ChangeTheme(sender);
        }

        private void tsmiClassicTheme_Click(object sender, EventArgs e)
        {
            ChangeTheme(sender);
        }

        private void ChangeTheme(object sender)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;

            if (clickedItem == tsmiModernTheme)
            {
                tpTR1.BackColor = Color.White;
                trbHealthTR1.BackColor = Color.White;

                tpTR1UB.BackColor = Color.White;
                trbHealthTR1UB.BackColor = Color.White;

                tpTR2.BackColor = Color.White;
                trbHealthTR2.BackColor = Color.White;

                tpTR2G.BackColor = Color.White;
                trbHealthTR2G.BackColor = Color.White;

                tpTR3.BackColor = Color.White;
                trbHealthTR3.BackColor = Color.White;

                tpTR3TLA.BackColor = Color.White;
                trbHealthTR3TLA.BackColor = Color.White;

                tpTR4.BackColor = Color.White;

                tpTRC.BackColor = Color.White;
                trbHealthTRC.BackColor = Color.White;

                tsmiClassicTheme.Checked = false;
                tsmiClassicTheme.Enabled = true;
                tsmiModernTheme.Enabled = false;
            }
            else if (clickedItem == tsmiClassicTheme)
            {
                tpTR1.BackColor = Color.WhiteSmoke;
                trbHealthTR1.BackColor = Color.WhiteSmoke;

                tpTR1UB.BackColor = Color.WhiteSmoke;
                trbHealthTR1UB.BackColor = Color.WhiteSmoke;

                tpTR2.BackColor = Color.WhiteSmoke;
                trbHealthTR2.BackColor = Color.WhiteSmoke;

                tpTR2G.BackColor = Color.WhiteSmoke;
                trbHealthTR2G.BackColor = Color.WhiteSmoke;

                tpTR3.BackColor = Color.WhiteSmoke;
                trbHealthTR3.BackColor = Color.WhiteSmoke;

                tpTR3TLA.BackColor = Color.WhiteSmoke;
                trbHealthTR3TLA.BackColor = Color.WhiteSmoke;

                tpTR4.BackColor = Color.WhiteSmoke;

                tpTRC.BackColor = Color.WhiteSmoke;
                trbHealthTRC.BackColor = Color.WhiteSmoke;

                tsmiModernTheme.Checked = false;
                tsmiModernTheme.Enabled = true;
                tsmiClassicTheme.Enabled = false;
            }
        }
    }
}