using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MMLauncher
{
    public partial class Form1 : Form
    {
        private string pathFile;
        string localVersion = "Launcher Version: 0.3";
        public Form1()
        {
            InitializeComponent();

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            pathFile = Path.Combine(exeDir, "mhaj_mod_path.txt");

            modListBox.AllowDrop = true;
            modListBox.MouseDown += modListBox_MouseDown;
            modListBox.MouseMove += modListBox_MouseMove;
            modListBox.MouseUp += modListBox_MouseUp;
            modListBox.DragOver += modListBox_DragOver;
            modListBox.DragDrop += modListBox_DragDrop;
            modListBox.SelectedIndexChanged += modListBox_SelectedIndexChanged;
        }

        private void output(string message)
        {
            outputRichTextBox.AppendText(message + Environment.NewLine);
            outputRichTextBox.ScrollToCaret();
        }

        private async void Form1_Load_1(object sender, EventArgs e)
        {
            await CheckForUpdates();

            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Loading mods...");
            RefreshModListBox();
            modListBox.Refresh();
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Found " + modListBox.Items.Count + " mods");
            ApplyChecksFromTildeMods();

            if (!File.Exists(pathFile))
                return;

            string savedPath = File.ReadAllText(pathFile).Trim();

            if (!Directory.Exists(savedPath))
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Paks folder not found");
                output($"[{DateTime.Now:HH:mm:ss}]  [WARN]  Please reselect the folder");
            }
        }

        private static string ExeDir => AppDomain.CurrentDomain.BaseDirectory;
        private static string ModsRootDir => Path.Combine(ExeDir, "mods");
        private const string MergeModName = "99999dbMerge-Windows_9_P";
        private static string ModOrderFile => Path.Combine(ExeDir, "mod_order.txt");
        private bool allowCheck = false;

        private static bool IsAllowedModFile(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            return ext.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".pak", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".utoc", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".ucas", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".png", StringComparison.OrdinalIgnoreCase);
        }

        private static string MakeSafeFolderName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Trim();
        }

        private void RefreshModListBox()
        {
            Directory.CreateDirectory(ModsRootDir);

            var dirs = Directory.GetDirectories(ModsRootDir);
            var modNames = new System.Collections.Generic.HashSet<string>();

            foreach (var dir in dirs)
            {
                string modName = Path.GetFileName(dir);

                // skip merge
                if (modName.Equals(MergeModName, StringComparison.OrdinalIgnoreCase))
                    continue;

                modNames.Add(modName);
            }

            // save current check states
            var checkStates = new System.Collections.Generic.Dictionary<string, bool>();
            for (int i = 0; i < modListBox.Items.Count; i++)
            {
                string modName = modListBox.Items[i].ToString();
                checkStates[modName] = modListBox.GetItemChecked(i);
            }

            modListBox.BeginUpdate();
            modListBox.Items.Clear();

            // load saved order
            if (File.Exists(ModOrderFile))
            {
                var savedOrder = File.ReadAllLines(ModOrderFile).ToList();

                // add mods in saved order
                foreach (var modName in savedOrder)
                {
                    if (modNames.Contains(modName))
                    {
                        bool wasChecked = checkStates.ContainsKey(modName) && checkStates[modName];
                        modListBox.Items.Add(modName, wasChecked);
                        modNames.Remove(modName);
                    }
                }
            }

            // add any new mods that werent in saved order
            foreach (var modName in modNames.OrderBy(x => x))
            {
                bool wasChecked = checkStates.ContainsKey(modName) && checkStates[modName];
                modListBox.Items.Add(modName, wasChecked);
            }

            modListBox.EndUpdate();
        }

        private void setupButton_Click(object sender, EventArgs e)
        {
            string suggested = @"C:\Program Files (x86)\Steam\steamapps\common\MY HERO ACADEMIA All's Justice\HeroGame\Content\Paks";

            string targetPaksDir;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Alls Justice Mod Setup – Select your HeroGame-Windows.pak inside your paks folder";
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = true;
                dialog.FileName = "Select HeroGame-Windows.pak";
                dialog.InitialDirectory = Directory.Exists(suggested)
                    ? suggested
                    : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                targetPaksDir = Path.GetDirectoryName(dialog.FileName);
            }

            if (string.IsNullOrWhiteSpace(targetPaksDir) || !Directory.Exists(targetPaksDir))
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Invalid folder selected");
                return;
            }

            File.WriteAllText(PathFilePaks, targetPaksDir);

            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Created ~mods folder");
            string modsFolder = Path.Combine(targetPaksDir, "~mods");
            Directory.CreateDirectory(modsFolder);

            File.WriteAllText(PathFileMods, modsFolder);

            string win64Dir = GetWin64FromPaks(targetPaksDir);
            if (string.IsNullOrWhiteSpace(win64Dir) || !Directory.Exists(win64Dir))
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Could not locate HeroGame/Binaries/Win64 from the selected Paks folder");
                return;
            }

            string ajExe = Path.Combine(win64Dir, "AJGAME.exe");
            if (!File.Exists(ajExe))
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  AJGAME.exe not found");
                return;
            }

            try
            {
                ExtractResourceToFile("MMLauncher.Files.dsound.dll", Path.Combine(win64Dir, "dsound.dll"));
                ExtractResourceToFile("MMLauncher.Files.UniversalSigBypasser.asi", Path.Combine(win64Dir, "UniversalSigBypasser.asi"));
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Setup complete");

                MessageBox.Show(
                    "Setup complete.\n\nPaks:\n" + targetPaksDir + "\n\nMods folder:\n" + modsFolder + "\n\nInstalled Sig Bypass to:\n" + win64Dir,
                    "Done",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (UnauthorizedAccessException)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Permission denied");
                MessageBox.Show("Permission denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  ");
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string PathFilePaks => Path.Combine(ExeDir, "mhaj_mod_path.txt");
        private static string PathFileMods => Path.Combine(ExeDir, "mods_folder_mhaj.txt");

        private static string GetWin64FromPaks(string paksDir)
        {
            var paksInfo = new DirectoryInfo(paksDir);
            var contentDir = paksInfo.Parent;
            var heroGameDir = contentDir?.Parent;
            if (heroGameDir == null) return string.Empty;

            return Path.Combine(heroGameDir.FullName, "Binaries", "Win64");
        }

        private static void ExtractResourceToFile(string resourceName, string outputPath)
        {
            var asm = Assembly.GetExecutingAssembly();

            using Stream resourceStream = asm.GetManifestResourceStream(resourceName);
            if (resourceStream == null)
                throw new FileNotFoundException("Embedded resource not found: " + resourceName);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            resourceStream.CopyTo(fileStream);
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            try
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Merging dblist...");
                MergeDbLists();
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Loading mods...");
                SyncTildeModsToCheckedMods();

                Process.Start(new ProcessStartInfo
                {
                    FileName = "steam://rungameid/2362050",
                    UseShellExecute = true
                });
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Launching game!");
            }
            catch (UnauthorizedAccessException)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Permissions denied to open game");
            }
            catch (Exception ex)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  " + ex.Message);
            }
        }

        private void discordButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/W5tXWUjTvc",
                    UseShellExecute = true
                });
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Inviting you to the discord!");
            }
            catch (Exception ex)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Failed to join server");
            }
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/mewxyx/MHAJ-Mod-Launcher",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open site\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void modListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modListBox.SelectedIndex == -1)
            {
                LoadDefaultDescriptionAndImage();
                return;
            }

            string modName = modListBox.SelectedItem.ToString();
            string modDir = Path.Combine(ModsRootDir, modName);

            if (!Directory.Exists(modDir))
            {
                LoadDefaultDescriptionAndImage();
                return;
            }

            string descriptionPath = Path.Combine(modDir, "description.txt");
            if (File.Exists(descriptionPath))
            {
                try
                {
                    descriptionRichTextBox.Text = File.ReadAllText(descriptionPath);
                }
                catch (Exception ex)
                {
                    output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Failed to load description");
                    LoadDefaultDescription();
                }
            }
            else
            {
                LoadDefaultDescription();
            }

            string imagePath = Path.Combine(modDir, "splash.png");
            if (File.Exists(imagePath))
            {
                try
                {
                    if (modImageBox.Image != null)
                    {
                        modImageBox.Image.Dispose();
                        modImageBox.Image = null;
                    }

                    modImageBox.Image = Image.FromFile(imagePath);
                    modImageBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    LoadDefaultImage();
                }
            }
            else
            {
                LoadDefaultImage();
            }
        }

        private void LoadDefaultDescription()
        {
            descriptionRichTextBox.Text = @"[You will see this message when the mod author hasnt included a description]
Mod launcher made by: @mewxy
With help from the remix discord server (click the discord button above to join)

Mod file format should be a .zip
Containing the .pak, .utoc and .ucas files

Additionally the zip can contain a ""description.txt"" to change
The text you see here
And a ""splash.png"" which will change the image you see above";
        }

        private void LoadDefaultImage()
        {
            if (modImageBox.Image != null)
            {
                modImageBox.Image.Dispose();
                modImageBox.Image = null;
            }

            modImageBox.Image = Properties.Resources.rip;
            modImageBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void LoadDefaultDescriptionAndImage()
        {
            LoadDefaultDescription();
            LoadDefaultImage();
        }

        private void addmodsButton_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(ModsRootDir);

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select a mod zip file";
                dialog.Filter = "Zip files (*.zip)|*.zip";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                string zipPath = dialog.FileName;

                try
                {
                    AddModFromZip(zipPath);
                    RefreshModListBox();

                    output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Mod added!");
                }
                catch (Exception ex)
                {
                    output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  " + ex.Message);
                }
            }
        }

        private void AddModFromZip(string zipPath)
        {
            using (var zip = ZipFile.OpenRead(zipPath))
            {
                var allowedEntries = zip.Entries
                    .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                    .Where(e => IsAllowedModFile(e.Name))
                    .ToList();

                if (allowedEntries.Count == 0)
                    throw new InvalidOperationException("No supported files found in the zip.");

                // find all .paks
                var pakEntries = allowedEntries
                    .Where(e => e.Name.EndsWith(".pak", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (pakEntries.Count == 0)
                    throw new InvalidOperationException("No .pak files found in the zip.");

                // check if multi-mod
                if (pakEntries.Count == 1)
                {
                    // single mod
                    ExtractSingleMod(zip, allowedEntries, zipPath);
                }
                else
                {
                    // multi-mod
                    ExtractMultiMod(zip, allowedEntries, pakEntries);
                }
            }
        }

        private void ExtractSingleMod(ZipArchive zip, System.Collections.Generic.List<ZipArchiveEntry> allowedEntries, string zipPath)
        {
            // look for name.txt first
            string folderName = null;
            var nameEntry = zip.Entries.FirstOrDefault(e =>
                e.Name.Equals("name.txt", StringComparison.OrdinalIgnoreCase));

            if (nameEntry != null)
            {
                using (var stream = nameEntry.Open())
                using (var reader = new StreamReader(stream))
                {
                    folderName = reader.ReadToEnd().Trim();
                }
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Using mod name from name.txt: " + folderName);
            }

            // default to pak name if no name.txt
            if (string.IsNullOrWhiteSpace(folderName))
            {
                var pakEntry = allowedEntries
                    .FirstOrDefault(e => e.Name.EndsWith(".pak", StringComparison.OrdinalIgnoreCase));

                if (pakEntry != null)
                    folderName = Path.GetFileNameWithoutExtension(pakEntry.Name);
                else
                    folderName = Path.GetFileNameWithoutExtension(zipPath);

                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Using mod name from pak/zip: " + folderName);
            }

            folderName = MakeSafeFolderName(folderName);

            string modDir = Path.Combine(ModsRootDir, folderName);
            Directory.CreateDirectory(modDir);

            foreach (var entry in allowedEntries)
            {
                string outPath = Path.Combine(modDir, entry.Name);

                using (var inStream = entry.Open())
                using (var outStream = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                {
                    inStream.CopyTo(outStream);
                }
            }

            if (nameEntry != null)
            {
                string nameFilePath = Path.Combine(modDir, "name.txt");
                using (var inStream = nameEntry.Open())
                using (var outStream = new FileStream(nameFilePath, FileMode.Create, FileAccess.Write))
                {
                    inStream.CopyTo(outStream);
                }
            }
        }

        private void ExtractMultiMod(ZipArchive zip, System.Collections.Generic.List<ZipArchiveEntry> allowedEntries, System.Collections.Generic.List<ZipArchiveEntry> pakEntries)
        {
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Detected multi-mod zip with {pakEntries.Count} mods");

            // Group files by their base name (without extension)
            var fileGroups = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ZipArchiveEntry>>(StringComparer.OrdinalIgnoreCase);
            var sharedFiles = new System.Collections.Generic.List<ZipArchiveEntry>();

            foreach (var entry in allowedEntries)
            {
                string baseName = Path.GetFileNameWithoutExtension(entry.Name);
                string extension = Path.GetExtension(entry.Name).ToLower();

                // Check if this file belongs to a pak group (.pak, .utoc, .ucas with same base name)
                bool belongsToGroup = false;
                foreach (var pakEntry in pakEntries)
                {
                    string pakBaseName = Path.GetFileNameWithoutExtension(pakEntry.Name);
                    if (baseName.Equals(pakBaseName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!fileGroups.ContainsKey(pakBaseName))
                            fileGroups[pakBaseName] = new System.Collections.Generic.List<ZipArchiveEntry>();

                        fileGroups[pakBaseName].Add(entry);
                        belongsToGroup = true;
                        break;
                    }
                }

                // Files that don't match any pak name are considered shared (description.txt, splash.png, etc.)
                if (!belongsToGroup)
                {
                    sharedFiles.Add(entry);
                }
            }

            // Extract each mod group
            foreach (var pakEntry in pakEntries)
            {
                string pakBaseName = Path.GetFileNameWithoutExtension(pakEntry.Name);
                string folderName = MakeSafeFolderName(pakBaseName);
                string modDir = Path.Combine(ModsRootDir, folderName);
                Directory.CreateDirectory(modDir);

                // Extract files belonging to this mod
                if (fileGroups.ContainsKey(pakBaseName))
                {
                    foreach (var entry in fileGroups[pakBaseName])
                    {
                        string outPath = Path.Combine(modDir, entry.Name);

                        using (var inStream = entry.Open())
                        using (var outStream = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                        {
                            inStream.CopyTo(outStream);
                        }
                    }
                }

                // Copy shared files (description.txt, splash.png, name.txt, etc.) to each mod folder
                foreach (var entry in sharedFiles)
                {
                    string outPath = Path.Combine(modDir, entry.Name);

                    using (var inStream = entry.Open())
                    using (var outStream = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                    {
                        inStream.CopyTo(outStream);
                    }
                }

                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Extracted mod: {folderName}");
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir, overwrite);
            }
        }

        private static void ClearDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                return;

            foreach (var file in Directory.GetFiles(dir))
                File.Delete(file);

            foreach (var subDir in Directory.GetDirectories(dir))
                Directory.Delete(subDir, true);
        }

        private static string ModsFolderFile => Path.Combine(ExeDir, "mods_folder_mhaj.txt");

        private static string GetTildeModsDir()
        {
            if (!File.Exists(ModsFolderFile))
                throw new InvalidOperationException("mods_folder_mhaj.txt not found. Please run Setup first.");

            string dir = File.ReadAllText(ModsFolderFile).Trim();

            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                throw new InvalidOperationException("Saved ~mods folder path is invalid. Please run Setup again.");

            return dir;
        }

        private void SyncTildeModsToCheckedMods()
        {
            string tildeModsDir = GetTildeModsDir();
            Directory.CreateDirectory(ModsRootDir);

            // delete mods in ~mods folder
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Clearing ~mods folder...");
            ClearDirectory(tildeModsDir);

            // get checked mods order
            var checkedMods = new System.Collections.Generic.List<string>();
            for (int i = 0; i < modListBox.Items.Count; i++)
            {
                if (modListBox.GetItemChecked(i))
                {
                    string modName = modListBox.Items[i].ToString();
                    if (!string.IsNullOrWhiteSpace(modName))
                        checkedMods.Add(modName);
                }
            }

            // copy over checked mods numerically
            int priority = 1;
            foreach (string modName in checkedMods)
            {
                string sourceModDir = Path.Combine(ModsRootDir, modName);
                if (!Directory.Exists(sourceModDir))
                    continue;

                string destModDir = Path.Combine(tildeModsDir, priority.ToString());
                CopyDirectory(sourceModDir, destModDir, overwrite: true);
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Copied '{modName}' as priority {priority}");
                priority++;
            }

            string mergeSourceDir = Path.Combine(ModsRootDir, MergeModName);
            if (Directory.Exists(mergeSourceDir))
            {
                string mergeDestDir = Path.Combine(tildeModsDir, priority.ToString());
                CopyDirectory(mergeSourceDir, mergeDestDir, overwrite: true);
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Copied dbMerge as priority {priority} (loads last)");
            }
        }

        private void ApplyChecksFromTildeMods()
        {
            try
            {
                // get the ~mods folder path
                if (!File.Exists(PathFileMods))
                    return;

                string tildeModsDir = File.ReadAllText(PathFileMods).Trim();

                if (!Directory.Exists(tildeModsDir))
                    return;

                // find all pak files in ~mods folder
                var tildePakFiles = Directory.GetFiles(tildeModsDir, "*.pak", SearchOption.AllDirectories)
                    .Select(f => Path.GetFileName(f))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (tildePakFiles.Count == 0)
                    return;

                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Found {tildePakFiles.Count} pak file(s) in ~mods folder");

                // check each mod in the listbox
                allowCheck = true;

                for (int i = 0; i < modListBox.Items.Count; i++)
                {
                    string modName = modListBox.Items[i].ToString();
                    string modDir = Path.Combine(ModsRootDir, modName);

                    if (!Directory.Exists(modDir))
                        continue;

                    // check if this mod contains any of the pak files from ~mods
                    var modPakFiles = Directory.GetFiles(modDir, "*.pak")
                        .Select(f => Path.GetFileName(f));

                    bool shouldCheck = modPakFiles.Any(pakFile => tildePakFiles.Contains(pakFile));

                    if (shouldCheck)
                    {
                        modListBox.SetItemChecked(i, true);
                        output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Auto-checked '{modName}'");
                    }
                }

                allowCheck = false;
            }
            catch (Exception ex)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Failed to apply checks from ~mods: {ex.Message}");
            }
        }

        private void deleteModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modListBox.SelectedIndex == -1)
            {
                MessageBox.Show("No mod selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string modName = modListBox.SelectedItem.ToString();

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{modName}'?\n\nThis will permanently remove the mod files.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                string modDir = Path.Combine(ModsRootDir, modName);

                if (Directory.Exists(modDir))
                {
                    Directory.Delete(modDir, true);
                    output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Deleted mod: {modName}");
                }

                RefreshModListBox();

                MessageBox.Show("Mod deleted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete mod:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MergeDbLists()
        {
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Starting dblist merge...");

            string masterDbListPath = Path.Combine(ExeDir, "master-dblist.txt");
            string masterDbListUrl = "https://raw.githubusercontent.com/mewxyx/MHAJ-Mod-Launcher/refs/heads/main/db/master-dblist.txt";

            try
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Downloading master-dblist from GitHub...");
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(masterDbListUrl, masterDbListPath);
                }
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Downloaded master-dblist.txt successfully");
            }
            catch (Exception ex)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Failed to download master-dblist.txt: " + ex.Message);
                throw new Exception("Failed to download master-dblist.txt from GitHub. Check your internet connection.");
            }

            var allEntries = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(masterDbListPath))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    allEntries.Add(line.Trim());
            }
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Loaded {allEntries.Count} entries from master-dblist.txt");

            int modsWithDbList = 0;
            foreach (var item in modListBox.CheckedItems)
            {
                string modName = item.ToString();
                string modDir = Path.Combine(ModsRootDir, modName);
                string dbListPath = Path.Combine(modDir, "dblist.txt");

                if (File.Exists(dbListPath))
                {
                    modsWithDbList++;
                    int beforeCount = allEntries.Count;

                    foreach (var line in File.ReadAllLines(dbListPath))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            allEntries.Add(line.Trim());
                    }

                    int added = allEntries.Count - beforeCount;
                    output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Merged dblist from '{modName}' (+{added} new entries)");
                }
            }

            if (modsWithDbList == 0)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  No mods with dblist.txt found, using master only");
            }

            File.WriteAllLines(masterDbListPath, allEntries.OrderBy(x => x));
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Updated master-dblist.txt with {allEntries.Count} total entries");

            string dbMergeRoot = Path.Combine(ExeDir, "dbMerge-Windows_9");
            string dbMergeDbPath = Path.Combine(dbMergeRoot, "HeroGame", "Content", "DB");

            if (Directory.Exists(dbMergeRoot))
                Directory.Delete(dbMergeRoot, true);

            Directory.CreateDirectory(dbMergeDbPath);

            string targetDbListPath = Path.Combine(dbMergeDbPath, "dblist.txt");
            File.Copy(masterDbListPath, targetDbListPath, true);
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Created dbMerge folder structure");

            string repakPath = Path.Combine(ExeDir, "repak.exe");
            string compileBatPath = Path.Combine(ExeDir, "Compile.bat");

            ExtractResourceToFile("MMLauncher.PakDB.repak.exe", repakPath);
            ExtractResourceToFile("MMLauncher.PakDB.Compile.bat", compileBatPath);
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Extracted compilation tools");

            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Running Compile.bat (this may take a moment)...");
            var processInfo = new ProcessStartInfo
            {
                FileName = compileBatPath,
                Arguments = $"\"{dbMergeRoot}\"",
                WorkingDirectory = ExeDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(processInfo))
            {
                string outputText = process.StandardOutput.ReadToEnd();
                string errorText = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(outputText))
                    output($"[{DateTime.Now:HH:mm:ss}]  [BAT]  " + outputText);

                if (!string.IsNullOrWhiteSpace(errorText))
                    output($"[{DateTime.Now:HH:mm:ss}]  [BAT ERR]  " + errorText);

                if (process.ExitCode != 0)
                {
                    output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Compile.bat failed");
                    throw new Exception("Compile.bat failed. Check output log.");
                }
            }

            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Compilation complete");

            System.Threading.Thread.Sleep(500);

            string mergeModName = MergeModName;
            string localMergeModDir = Path.Combine(ModsRootDir, mergeModName);

            if (Directory.Exists(localMergeModDir))
                Directory.Delete(localMergeModDir, true);
            Directory.CreateDirectory(localMergeModDir);

            // find merge mod pak
            string pakFile = "dbMerge-Windows_9_P.pak";
            string sourcePath = Path.Combine(ExeDir, pakFile);

            if (File.Exists(sourcePath))
            {
                string destPath = Path.Combine(localMergeModDir, pakFile);
                File.Copy(sourcePath, destPath, true);
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Copied {pakFile} to local mods");
            }
            else
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [WARN]  {pakFile} not found after compilation");
                throw new Exception("Pak file was not generated. Check the compilation process.");
            }

            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  dblist merge complete!");
        }


        private void modListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!allowCheck)
                e.NewValue = e.CurrentValue;
        }

        private void openModFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modListBox.SelectedIndex == -1)
            {
                MessageBox.Show("No mod selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string modName = modListBox.SelectedItem.ToString();
            string modDir = Path.Combine(ModsRootDir, modName);

            if (!Directory.Exists(modDir))
            {
                MessageBox.Show("Mod folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = modDir,
                    UseShellExecute = true,
                    Verb = "open"
                });
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Opened folder for '{modName}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open folder:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int dragIndex = -1;
        private Rectangle dragBox = Rectangle.Empty;
        private Point mouseDownPoint;

        private void modListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int index = modListBox.IndexFromPoint(e.Location);

            if (index != -1)
            {
                mouseDownPoint = e.Location;
                Rectangle itemRect = modListBox.GetItemRectangle(index);
                Rectangle checkBoxRect = new Rectangle(itemRect.Left, itemRect.Top, 16, itemRect.Height);

                allowCheck = checkBoxRect.Contains(e.Location);
                // prepare for drag
                if (!checkBoxRect.Contains(e.Location))
                {
                    dragIndex = index;
                    Size dragSize = SystemInformation.DragSize;
                    dragBox = new Rectangle(
                        new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)),
                        dragSize
                    );
                }
                else
                {
                    dragIndex = -1;
                    dragBox = Rectangle.Empty;
                }
            }
        }

        private void modListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dragIndex != -1 && !dragBox.IsEmpty)
            {
                // only start drag if mouse moved outside drag box
                if (!dragBox.Contains(e.Location))
                {
                    modListBox.DoDragDrop(modListBox.Items[dragIndex], DragDropEffects.Move);
                    dragBox = Rectangle.Empty;
                }
            }
        }

        private void modListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void modListBox_DragDrop(object sender, DragEventArgs e)
        {
            Point point = modListBox.PointToClient(new Point(e.X, e.Y));
            int targetIndex = modListBox.IndexFromPoint(point);

            if (targetIndex < 0)
                targetIndex = modListBox.Items.Count - 1;

            if (dragIndex != -1 && targetIndex != dragIndex)
            {
                object draggedItem = modListBox.Items[dragIndex];
                bool wasChecked = modListBox.GetItemChecked(dragIndex);

                modListBox.BeginUpdate();

                modListBox.Items.RemoveAt(dragIndex);
                modListBox.Items.Insert(targetIndex, draggedItem);
                modListBox.SetItemChecked(targetIndex, wasChecked);

                modListBox.EndUpdate();
                modListBox.SelectedIndex = targetIndex;

                SaveModOrder();
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Moved '{draggedItem}' to position {targetIndex + 1}");
            }

            dragIndex = -1;
            dragBox = Rectangle.Empty;
        }

        private void modListBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragIndex = -1;
            dragBox = Rectangle.Empty;
        }

        private void SaveModOrder()
        {
            var order = new System.Collections.Generic.List<string>();
            for (int i = 0; i < modListBox.Items.Count; i++)
            {
                order.Add(modListBox.Items[i].ToString());
            }
            File.WriteAllLines(ModOrderFile, order);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void versionLabel_Click(object sender, EventArgs e)
        {

        }
        private async Task CheckForUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://raw.githubusercontent.com/mewxyx/MHAJ-Mod-Launcher/refs/heads/main/updates/version.txt";
                    string content = await client.GetStringAsync(url);

                    if (string.IsNullOrWhiteSpace(content)) return;

                    string onlineVersion = content.Split('\n')[0].Trim();

                    versionLabel.Text = onlineVersion;

                    if (onlineVersion != localVersion)
                    {
                        DialogResult result = MessageBox.Show(
                            "A new version is available. Would you like to update?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information
                        );

                        if (result == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://github.com/mewxyx/MHAJ-Mod-Launcher",
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch
            {
                versionLabel.Text = localVersion;
            }
        }

        private void vanillaButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(PathFileMods))
                {
                    string modsPath = File.ReadAllText(PathFileMods).Trim();

                    // check if the mods directory exists
                    if (Directory.Exists(modsPath))
                    {
                        // get all subdirectories and delete them
                        string[] folders = Directory.GetDirectories(modsPath);

                        foreach (string folder in folders)
                        {
                            Directory.Delete(folder, true);
                        }
                        output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Removing Mods");

                    }
                    else
                    {
                        output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Mods folder path does not exist.");
                    }
                }
                else
                {
                    output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  Config file not found: " + PathFileMods);
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "steam://rungameid/2362050",
                    UseShellExecute = true
                });
                output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Launching Game!");
            }
            catch (Exception ex)
            {
                output($"[{DateTime.Now:HH:mm:ss}]  [ERR]  ");
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            modListBox.Refresh();
            output($"[{DateTime.Now:HH:mm:ss}]  [INFO]  Mods Refreshed");
        }
    }
}