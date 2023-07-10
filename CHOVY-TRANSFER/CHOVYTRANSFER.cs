using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

using Li.Progress;
using Param;
using PspCrypto;
using Vita.ContentManager;
using Vita.PsvImgTools;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CHOVY_TRANSFER
{
    public partial class CHOVYTRANSFER : Form
    {
        const int FW_VERSION = 0x3600000;
        private byte[] chovy_gen(string ebootpbp, UInt64 AID)
        {
            bool ps1 = IsPs1(ebootpbp);
            using (FileStream fs = File.OpenRead(ebootpbp))
            {
                byte[] ebootSig = new byte[0x200];
                SceNpDrm.Aid = AID;
                
                if(ps1)
                    SceNpDrm.KsceNpDrmEbootSigGenPs1(fs, ebootSig, FW_VERSION);
                else
                    SceNpDrm.KsceNpDrmEbootSigGenPsp(fs, ebootSig, FW_VERSION);

                return ebootSig;
            }
        }

        public bool IsDexAidSet()
        {
            string isDex = ReadSetting("dexAid");
            if (isDex == "false" || isDex == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string ReadSetting(string Setting)
        {
            string Value = "";
            try
            {

                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey(@"Software\CHOVYProject\Chovy-Trans");
                Value = key.GetValue(Setting).ToString();
                key.Close();
            }
            catch (Exception) { return ""; }
            return Value;
        }

        public void WriteSetting(string Setting, string Value)
        {
            try
            {
                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey(@"Software\CHOVYProject\Chovy-Trans");
                key.SetValue(Setting, Value);
                key.Close();
            }
            catch (Exception) { }
        }

        public void ChangeCmaDir(string newDir)
        {
            string DriveLetter = Path.GetPathRoot(newDir);
            driveLetterDst.Text = DriveLetter;
            cmaDir.Text = newDir.Remove(0, DriveLetter.Length);
        }

        public void ChangePspDir(string newDir)
        {
            string DriveLetter = Path.GetPathRoot(newDir);
            driveLetterSrc.Text = DriveLetter;
            pspFolder.Text = newDir.Remove(0, DriveLetter.Length);
        }

        public string FindPspDir()
        {
            for(char i = 'A'; i < 'Z'; i++)
            {
                if (Directory.Exists(Path.Combine(i + ":\\", pspFolder.Text)))
                {
                    return Path.Combine(i + ":\\", pspFolder.Text);
                }
            }
            return Path.Combine("D:\\",pspFolder.Text);
        }

        public int ReadInt32(Stream s)
        {
            byte[] IntData = new byte[0x4];
            s.Read(IntData, 0x00, 0x4);
            return BitConverter.ToInt32(IntData, 0x00);
        }

        public void PopulatePspGameList()
        {
            string PspDir = Path.Combine(driveLetterSrc.Text, pspFolder.Text);
            pspGames.Items.Clear();
            try
            {
                string[] games = Directory.GetFileSystemEntries(Path.Combine(PspDir, "GAME"));
               
                foreach (string game in games)
                {
                    try
                    {
                        string EbootPbp = Path.Combine(game, "EBOOT.PBP");
                        if (!File.Exists(EbootPbp))
                            EbootPbp = Path.Combine(game, "PARAM.PBP");
                        string TitleId = Path.GetFileName(game);
                        string Title = GetTitleFromPbp(EbootPbp);
                        string ContentId = GetContentIdFromPbp(EbootPbp);

                        string LicenseFile = Path.Combine(PspDir, "LICENSE", ContentId + ".RIF");

                        if (TitleId.Length == 9 && File.Exists(LicenseFile))
                            pspGames.Items.Add(TitleId + " - " + Title);
                        
                    }
                    catch (Exception) { };

                }
            }
            catch (Exception) { };
            
        }

        public string[] SearchEdats(string gameFolder)
        {
            List<string> contentIds = new List<string>();
            foreach(string file in Directory.GetFiles(gameFolder, "*", SearchOption.AllDirectories))
            {
                if(Path.GetExtension(file).ToUpperInvariant() == ".EDAT")
                {
                    string contentId = GetContentIdFromPspEdat(file);
                    contentIds.Add(contentId);
                }
            }
            return contentIds.ToArray();
        }

        public string GetTitleFromPbp(string pbp)
        {
            byte[] SfoData = GetSfo(pbp);
            Sfo sfo = Sfo.ReadSfo(SfoData);
            return sfo["TITLE"] as String;
        }

        public byte[] GetSfo(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x08, SeekOrigin.Begin);
            int sfoOffset = ReadInt32(pbps);
            int sfoSize = ReadInt32(pbps);
            
            pbps.Seek(sfoOffset, SeekOrigin.Begin);
            sfoSize -= (int)pbps.Position;

            byte[] SfoData = new byte[sfoSize];
            pbps.Read(SfoData, 0x00, sfoSize);
            pbps.Dispose();
            return SfoData;
        }

        public byte[] GetIcon0(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x0C, SeekOrigin.Begin);
            int sfoOffset = ReadInt32(pbps);
            int iconSize = ReadInt32(pbps);

            
            pbps.Seek(sfoOffset, SeekOrigin.Begin);
            iconSize -= (int)pbps.Position;

            byte[] IconData = new byte[iconSize];
            pbps.Read(IconData, 0x00, iconSize);
            pbps.Dispose();
            return IconData;
        }
        public string GetContentIdFromPspEdat(string edat)
        {
            FileStream edats = File.OpenRead(edat);
            edats.Seek(0x10, SeekOrigin.Begin);
            byte[] ContentId = new byte[0x24];
            edats.Read(ContentId, 0x00, 0x24);
            edats.Close();
            return Encoding.UTF8.GetString(ContentId);
        }
        public string GetContentIdFromPs1Pbp(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x20, SeekOrigin.Begin);
            Int64 PSPData = ReadInt32(pbps);
            pbps.Seek(PSPData + 0x560, SeekOrigin.Begin);
            byte[] ContentId = new byte[0x24];
            pbps.Read(ContentId, 0x00, 0x24);
            pbps.Close();
            return Encoding.UTF8.GetString(ContentId);
        }
        public bool IsPsp(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x24, SeekOrigin.Begin);
            Int64 NPUMDIMGOffest = ReadInt32(pbps);
            pbps.Seek(NPUMDIMGOffest, SeekOrigin.Begin);
            byte[] Header = new byte[0x8];
            pbps.Read(Header, 0x00, 0x8);
            pbps.Close();
            string header = Encoding.UTF8.GetString(Header);
            if (header == "NPUMDIMG")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool IsPs1(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x24, SeekOrigin.Begin);
            Int64 PSIMGOffest = ReadInt32(pbps);
            pbps.Seek(PSIMGOffest, SeekOrigin.Begin);
            byte[] Header = new byte[0x8];
            pbps.Read(Header, 0x00, 0x8);
            pbps.Close();
            string header = Encoding.UTF8.GetString(Header);
            if (header == "PSISOIMG" /*Single Disc PSX*/ || header == "PSTITLEI" /*Multi Disc PSX*/)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public string GetContentIdFromPbp(string pbp)
        {
            if(IsPs1(pbp))
            {
                return GetContentIdFromPs1Pbp(pbp);
            }
            else if(IsPsp(pbp))
            {
                return GetContentIdFromPspPbp(pbp);
            }
            else
            {
                string[] cids = SearchEdats(Path.GetDirectoryName(pbp));
                if (cids.Length <= 0)
                    return "";
                return cids.First();
            }
        }

        public string GetContentIdFromPspPbp(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x24, SeekOrigin.Begin);
            Int64 NPUMDIMGOffest = ReadInt32(pbps);
            pbps.Seek(NPUMDIMGOffest + 0x10, SeekOrigin.Begin);
            byte[] ContentId = new byte[0x24];
            pbps.Read(ContentId, 0x00, 0x24);
            pbps.Close();
            return Encoding.UTF8.GetString(ContentId);
        }

        public CHOVYTRANSFER()
        {
            InitializeComponent();
        }

        private void CHOVYTRANSFER_Load(object sender, EventArgs e)
        {
            string PspDir = ReadSetting("PspFolder");
            if (PspDir != "")
            {
                pspFolder.Text = PspDir;
            }

            string cmaDir = ReadSetting("CmaDir");
            if(cmaDir == "")
                cmaDir = SettingsReader.BackupsFolder;
            
            SettingsReader.BackupsFolder = cmaDir;

            ChangePspDir(FindPspDir());
            ChangeCmaDir(SettingsReader.BackupsFolder);
            PopulatePspGameList();
        }

        private void pspFolder_TextChanged(object sender, EventArgs e)
        {
            WriteSetting("PspFolder", pspFolder.Text);
            PopulatePspGameList();
        }

        private void driveLetterSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulatePspGameList();
        }

        private void cmaDir_TextChanged(object sender, EventArgs e)
        {
            string dir = Path.Combine(driveLetterDst.Text, cmaDir.Text);
            WriteSetting("CmaDir", dir);
            SettingsReader.BackupsFolder = dir;
        }
        private void driveLetterDst_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmaDir_TextChanged(sender, e);
        }
        private void transVita_EnabledChanged(object sender, EventArgs e)
        {
            Color red = Color.FromArgb(192, 0, 0);
            Color black = Color.Black;
            bool enabled = this.transVita.Enabled;
            this.transVita.ForeColor = enabled ? red : black;
            this.transVita.BackColor = enabled ? black : red;
        }

        private void CHOVYTRANSFER_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private async void transVita_Click(object sender, EventArgs e)
        {

            if(!Directory.Exists(SettingsReader.BackupsFolder))
            {
                MessageBox.Show("CMA Folder Doesn't Exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(Path.Combine(driveLetterSrc.Text,pspFolder.Text)))
            {
                MessageBox.Show("PSP Folder Doesn't Exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (pspGames.SelectedIndex < 0)
            {
                MessageBox.Show("No PSP Game Selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            transVita.Enabled = false;
            driveLetterDst.Enabled = false;
            driveLetterSrc.Enabled = false;
            pspFolder.ReadOnly = true;
            cmaDir.ReadOnly = true;
            pspGames.Enabled = false;

            bool isDlc = false;

            string titleId = pspGames.SelectedItem.ToString().Substring(0, 9);
            string pspDir = Path.Combine(driveLetterSrc.Text, pspFolder.Text);
            string gameFolder = Path.Combine(pspDir, "GAME", titleId);
            string ebootFile = Path.Combine(gameFolder, "EBOOT.PBP");
            if (!File.Exists(ebootFile))
            {
                isDlc = true;
                ebootFile = Path.Combine(gameFolder, "PARAM.PBP");
            }

            List<string> licenseFiles = new List<string>();
            string cid = GetContentIdFromPbp(ebootFile);
            licenseFiles.Add(Path.Combine(pspDir, "LICENSE", cid + ".RIF"));
            string sigFile = Path.Combine(gameFolder, "__sce_ebootpbp");
            
            bool isPs1 = IsPs1(ebootFile);

            if (!File.Exists(licenseFiles.First()))
            {
                MessageBox.Show("Could not find LICENSE file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                transVita.Enabled = true;
                driveLetterDst.Enabled = true;
                driveLetterSrc.Enabled = true;
                pspFolder.ReadOnly = false;
                cmaDir.ReadOnly = false;
                pspGames.Enabled = true;
                return;
            }

            FileStream rif = File.OpenRead(licenseFiles.First());
            byte[] bAid = new byte[0x08];
            rif.Seek(0x08, SeekOrigin.Begin);
            rif.Read(bAid, 0x00, 0x08);
            rif.Close();
            string sAid = BitConverter.ToString(bAid).Replace("-", "").ToLower();
            UInt64 uAid = BitConverter.ToUInt64(bAid, 0x00);

            if(File.Exists(sigFile))
            {
                File.Delete(sigFile);
            }

            byte[] EbootSig = chovy_gen(ebootFile, uAid);
            Account CmaAccount = new Account(uAid);
            CmaAccount.Devkit = IsDexAidSet();
            

            /*
             *  BUILD PSVIMG FILE(s)
             */

            // Pacakge GAME
            
            byte[] CmaKey = CmaAccount.CmaKey;

            string[] entrys = Directory.GetFileSystemEntries(gameFolder, "*", SearchOption.AllDirectories);
            long noEntrys = entrys.LongLength;
            string parentPath = "ux0:pspemu/temp/game/PSP/GAME/" + titleId;
            int noBlocks = 0;
            foreach (string fileName in Directory.GetFiles(gameFolder, "*", SearchOption.AllDirectories))
                noBlocks += Convert.ToInt32(new FileInfo(fileName).Length / PSVIMGConstants.PSVIMG_BLOCK_SIZE);
            progressBar.Maximum = noBlocks;


            string pgameFolder;
            string pgameFolderLicense;
            string scesys;
            if (!isPs1)
            {
                if(!IsDexAidSet())
                {
                    pgameFolder = Path.Combine(SettingsReader.PspFolder, sAid, titleId, "game");
                    pgameFolderLicense = Path.Combine(SettingsReader.PspFolder, sAid, titleId, "license");
                    scesys = Path.Combine(SettingsReader.PspFolder, sAid, titleId, "sce_sys");
                }
                else
                {
                    pgameFolder = Path.Combine(SettingsReader.PspFolder, "0000000000000000", titleId, "game");
                    pgameFolderLicense = Path.Combine(SettingsReader.PspFolder, "0000000000000000", titleId, "license");
                    scesys = Path.Combine(SettingsReader.PspFolder, "0000000000000000", titleId, "sce_sys");
                }
               
            }
            else
            {
                if(!IsDexAidSet())
                {
                    pgameFolder = Path.Combine(SettingsReader.Ps1Folder, sAid, titleId, "game");
                    pgameFolderLicense = Path.Combine(SettingsReader.Ps1Folder, sAid, titleId, "license");
                    scesys = Path.Combine(SettingsReader.Ps1Folder, sAid, titleId, "sce_sys");
                }
                else
                {
                    pgameFolder = Path.Combine(SettingsReader.Ps1Folder, "0000000000000000", titleId, "game");
                    pgameFolderLicense = Path.Combine(SettingsReader.Ps1Folder, "0000000000000000", titleId, "license");
                    scesys = Path.Combine(SettingsReader.Ps1Folder, "0000000000000000", titleId, "sce_sys");
                }
                
            }
           
            Directory.CreateDirectory(pgameFolder);
            Directory.CreateDirectory(pgameFolderLicense);
            Directory.CreateDirectory(scesys);

            string psvimgFilepathLicense = Path.Combine(pgameFolderLicense, "license.psvimg");
            string psvimgFilepath = Path.Combine(pgameFolder, "game.psvimg");

            string psvmdFilepathLicense = Path.Combine(pgameFolderLicense, "license.psvmd");
            string psvmdFilepath = Path.Combine(pgameFolder, "game.psvmd");

            await Task.Run(() =>
            {
                using (FileStream gamePsvimg = File.Open(psvimgFilepath, FileMode.Create, FileAccess.ReadWrite))
                {
                    PSVIMGBuilder builder = new PSVIMGBuilder(gamePsvimg, CmaKey);
                    builder.RegisterCallback((ProgressInfo inf) =>
                    {
                        Invoke((Action)delegate {
                            int tBlocks = builder.BlocksWritten;
                            if (tBlocks > noBlocks) tBlocks = noBlocks;
                            progressBar.Value = tBlocks;
                            decimal progress = Math.Floor(((decimal)tBlocks / (decimal)noBlocks) * 100);
                            progressStatus.Text = progress.ToString() + "%";
                            currentFile.Text = inf.CurrentProcess;
                        });
                    });

                    foreach (string entry in entrys)
                    {
                        string relativePath = entry.Remove(0, gameFolder.Length);
                        relativePath = relativePath.Replace('\\', '/');

                        if (Path.GetExtension(entry).ToUpperInvariant() == ".EDAT")
                        {
                            string edatContentId = GetContentIdFromPspEdat(entry);
                            string rifPath = Path.Combine(pspDir, "LICENSE", edatContentId + ".RIF");
                            if (!licenseFiles.Contains(rifPath) && File.Exists(rifPath))
                                licenseFiles.Add(rifPath);
                        }

                        bool isDir = File.GetAttributes(entry).HasFlag(FileAttributes.Directory);

                        if (isDir)
                        {
                            builder.AddDir(entry, parentPath, relativePath);
                        }
                        else
                        {
                            builder.AddFile(entry, parentPath, relativePath);
                        }
                    }

                    // add __sce_ebootpbp
                    if(!isDlc)
                        builder.AddFile(EbootSig, parentPath, "/__sce_ebootpbp");

                    long ContentSize = builder.Finish();

                    using (FileStream gamePsvmd = File.Open(psvmdFilepath, FileMode.Create, FileAccess.ReadWrite))
                        PSVMDBuilder.CreatePsvmd(gamePsvmd, gamePsvimg, ContentSize, "game", CmaKey);
                }


                // Package LICENSE
                using (FileStream licensePsvimg = File.Open(psvimgFilepathLicense, FileMode.Create, FileAccess.ReadWrite))
                {
                    PSVIMGBuilder builder = new PSVIMGBuilder(licensePsvimg, CmaKey);
                    foreach (string licenseFile in licenseFiles)
                        builder.AddFile(licenseFile, "ux0:pspemu/temp/game/PSP/LICENSE", "/" + Path.GetFileNameWithoutExtension(licenseFile) + ".rif");
                    long ContentSize = builder.Finish();

                    using (FileStream licensePsvmd = File.Open(psvmdFilepathLicense, FileMode.Create, FileAccess.ReadWrite))
                        PSVMDBuilder.CreatePsvmd(licensePsvmd, licensePsvimg, ContentSize, "license", CmaKey);
                }
            });
            
            // Write PARAM.SFO & ICON0.PNG

            byte[] ParamSfo = GetSfo(ebootFile);
            byte[] Icon0 = GetIcon0(ebootFile);
            File.WriteAllBytes(Path.Combine(scesys, "param.sfo"), ParamSfo);
            File.WriteAllBytes(Path.Combine(scesys, "icon0.png"), Icon0);

            if(!isPs1)
            {
                MessageBox.Show("Success!\nYou can now Restore the game onto your PSVita from\n[Content Mannager] (Copy Content) -> (PC) -> (Applications) -> (PC -> This System) -> (PSP/Others)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Success!\nYou can now Restore the game onto your PSVita from\n[Content Mannager] (Copy Content) -> (PC) -> (Applications) -> (PC -> This System) -> (PlayStation)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar.Value = 0;
            progressStatus.Text = "0%";
            currentFile.Text = "Waiting ...";

            transVita.Enabled = true;
            driveLetterDst.Enabled = true;
            driveLetterSrc.Enabled = true;
            pspFolder.ReadOnly = false;
            cmaDir.ReadOnly = false;
            pspGames.Enabled = true;
        }

        private void dexToggle_Click(object sender, EventArgs e)
        {
            if(IsDexAidSet())
            {
                WriteSetting("dexAid", "false");
                MessageBox.Show("Cex AID Will be used for CMA.", "CexAid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                WriteSetting("dexAid", "true");
                MessageBox.Show("Dex AID (0000000000000000) Will be used for CMA.","DexAid",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
