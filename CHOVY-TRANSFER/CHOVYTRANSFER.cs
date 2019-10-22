using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;
using Param_SFO;
using System.Runtime.InteropServices;
using System.Text;
using KeyDerivation;
using PSVIMGTOOLS;
using System.Drawing;

namespace CHOVY_TRANSFER
{
    public partial class CHOVYTRANSFER : Form
    {
        [DllImport("CHOVY.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int chovy_gen(string ebootpbp, UInt64 AID, string outscefile);

        public string GetCmaDir()
        {
            string Dir = "";
            try
            {
                //try qcma
                Dir = Registry.CurrentUser.OpenSubKey(@"Software\codestation\qcma").GetValue("appsPath").ToString();
            }
            catch (Exception)
            {
                try
                {
                    //try sony cma
                    Dir = Registry.CurrentUser.OpenSubKey(@"Software\Sony Corporation\Content Manager Assistant\Settings").GetValue("ApplicationHomePath").ToString();
                }
                catch (Exception)
                {
                    try
                    {
                        //try devkit cma
                        Dir = Registry.CurrentUser.OpenSubKey(@"Software\SCE\PSP2\Services\Content Manager Assistant for PlayStation(R)Vita DevKit\Settings").GetValue("ApplicationHomePath").ToString();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            string DefaultDir = Path.Combine(Environment.GetEnvironmentVariable("HOMEDRIVE"), Environment.GetEnvironmentVariable("HOMEPATH"), "Documents", "PS Vita");
                            if (Directory.Exists(DefaultDir))
                            {
                                Dir = DefaultDir;
                            }
                        }
                        catch (Exception)
                        {
                            //Do nothing
                        }
                    }
                }

            }

            if (ReadSetting("CmaDir") != "")
            {
                Dir = ReadSetting("CmaDir");
            }
            
            return Dir.Replace("/","\\");
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
                        

                        string TitleId = Path.GetFileName(game);
                        string Title = GetTitleFromPbp(EbootPbp);
                        string ContentId = GetContentIdFromPbp(EbootPbp);

                        string LicenseFile = Path.Combine(PspDir, "LICENSE", ContentId);

                        if (TitleId.Length == 9 && File.Exists(LicenseFile));
                        {
                            pspGames.Items.Add(TitleId + " - " + Title);
                        } 
                        
                    }
                    catch (Exception) { };

                }
            }
            catch (Exception) { };
            
        }
        public string GetTitleFromPbp(string pbp)
        {
            byte[] SfoData = GetSfo(pbp);

            using (MemoryStream ms = new MemoryStream(SfoData, 0x00, SfoData.Length)) 
            {
                PARAM_SFO sfo = new PARAM_SFO(ms);
                return sfo.Title;
            }
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

        public bool IsPs1(string pbp)
        {
            FileStream pbps = File.OpenRead(pbp);
            pbps.Seek(0x24, SeekOrigin.Begin);
            Int64 NPUMDIMGOffest = ReadInt32(pbps);
            pbps.Seek(NPUMDIMGOffest, SeekOrigin.Begin);
            byte[] Header = new byte[0x8];
            pbps.Read(Header, 0x00, 0x8);
            pbps.Close();
            if(Encoding.UTF8.GetString(Header) == "PSISOIMG")
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
            else
            {
                return GetContentIdFromPspPbp(pbp);
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

            ChangePspDir(FindPspDir());
            ChangeCmaDir(GetCmaDir());
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
            WriteSetting("CmaDir", Path.Combine(driveLetterDst.Text, cmaDir.Text));
        }
        private void driveLetterDst_SelectedIndexChanged(object sender, EventArgs e)
        {
            WriteSetting("CmaDir", Path.Combine(driveLetterDst.Text, cmaDir.Text));
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

        private void transVita_Click(object sender, EventArgs e)
        {

            if(!Directory.Exists(Path.Combine(driveLetterDst.Text, cmaDir.Text)))
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

            string titleId = pspGames.SelectedItem.ToString().Substring(0, 9);
            string gameFolder = Path.Combine(driveLetterSrc.Text, pspFolder.Text, "GAME", titleId);
            string ebootFile = Path.Combine(gameFolder, "EBOOT.PBP");
            string cid = GetContentIdFromPbp(ebootFile);
            string licenseFile = Path.Combine(driveLetterSrc.Text, pspFolder.Text, "LICENSE", cid + ".RIF");
            string sigFile = Path.Combine(gameFolder, "__sce_ebootpbp");
            string backupDir = Path.Combine(driveLetterDst.Text, cmaDir.Text);

            bool isPs1 = IsPs1(ebootFile);

            if (!File.Exists(licenseFile))
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

            FileStream rif = File.OpenRead(licenseFile);
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

            int ChovyGenRes = chovy_gen(ebootFile, uAid, sigFile);

            if (!File.Exists(sigFile) || ChovyGenRes != 0)
            {
                MessageBox.Show("CHOVY-GEN Failed! Please check CHOVY.DLL exists\nand that the Microsoft Visual C++ 2015 Redistributable Update 3 RC is installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                transVita.Enabled = true;
                driveLetterDst.Enabled = true;
                driveLetterSrc.Enabled = true;
                pspFolder.ReadOnly = false;
                cmaDir.ReadOnly = false;
                pspGames.Enabled = true;
                return;
            }

            /*
             *  BUILD PSVIMG FILE(s)
             */

            // Pacakge GAME

            byte[] CmaKey = CmaKeys.GenerateKey(bAid);

            string[] entrys = Directory.GetFileSystemEntries(gameFolder, "*", SearchOption.AllDirectories);
            long noEntrys = entrys.LongLength;
            string parentPath = "ux0:pspemu/temp/game/PSP/GAME/" + titleId;
            int noBlocks = 0;
            foreach (string fileName in Directory.GetFiles(gameFolder, "*", SearchOption.AllDirectories))
            {
                noBlocks += Convert.ToInt32(new FileInfo(fileName).Length / PSVIMGConstants.PSVIMG_BLOCK_SIZE);
            }
            progressBar.Maximum = noBlocks;


            string pgameFolder;
            string pgameFolderl;
            string scesys;
            if (!isPs1)
            {
                pgameFolder = Path.Combine(backupDir, "PGAME", sAid, titleId, "game");
                pgameFolderl = Path.Combine(backupDir, "PGAME", sAid, titleId, "license");
                scesys = Path.Combine(backupDir, "PGAME", sAid, titleId, "sce_sys");
            }
            else
            {
                pgameFolder = Path.Combine(backupDir, "PSGAME", sAid, titleId, "game");
                pgameFolderl = Path.Combine(backupDir, "PSGAME", sAid, titleId, "license");
                scesys = Path.Combine(backupDir, "PSGAME", sAid, titleId, "sce_sys");
            }
           
            Directory.CreateDirectory(pgameFolder);
            Directory.CreateDirectory(pgameFolderl);
            Directory.CreateDirectory(scesys);

            string psvimgFilepathl = Path.Combine(pgameFolderl, "license.psvimg");
            string psvimgFilepath = Path.Combine(pgameFolder, "game.psvimg");

            string psvmdFilepathl = Path.Combine(pgameFolderl, "license.psvmd");
            string psvmdFilepath = Path.Combine(pgameFolder, "game.psvmd");

            FileStream gamePsvimg = File.OpenWrite(psvimgFilepath);
            gamePsvimg.SetLength(0);
            PSVIMGBuilder builder = new PSVIMGBuilder(gamePsvimg, CmaKey);

            foreach (string entry in entrys)
            {
                string relativePath = entry.Remove(0, gameFolder.Length);
                relativePath = relativePath.Replace('\\', '/');

                bool isDir = File.GetAttributes(entry).HasFlag(FileAttributes.Directory);

                if (isDir)
                {
                    builder.AddDir(entry, parentPath, relativePath);
                }
                else
                {
                    builder.AddFileAsync(entry, parentPath, relativePath);
                    while (!builder.HasFinished)
                    {
                        try
                        {
                            int tBlocks = builder.BlocksWritten;
                            progressBar.Value = tBlocks;
                            decimal progress = Math.Floor(((decimal)tBlocks / (decimal)noBlocks) * 100);
                            progressStatus.Text = progress.ToString() + "%";
                        }
                        catch (Exception) { }

                        Application.DoEvents();
                    }
                }
            }

            long ContentSize = builder.Finish();
            gamePsvimg = File.OpenRead(psvimgFilepath);
            FileStream gamePsvmd = File.OpenWrite(psvmdFilepath);
            PSVMDBuilder.CreatePsvmd(gamePsvmd, gamePsvimg, ContentSize, "game", CmaKey);
            gamePsvmd.Close();
            gamePsvimg.Close();


            // Package LICENSE
            FileStream licensePsvimg = File.OpenWrite(psvimgFilepathl);
            licensePsvimg.SetLength(0);
            builder = new PSVIMGBuilder(licensePsvimg, CmaKey);
            builder.AddFile(licenseFile, "ux0:pspemu/temp/game/PSP/LICENSE", "/" + cid + ".rif");
            ContentSize = builder.Finish();

            licensePsvimg = File.OpenRead(psvimgFilepathl);
            FileStream licensePsvmd = File.OpenWrite(psvmdFilepathl);
            PSVMDBuilder.CreatePsvmd(licensePsvmd, licensePsvimg, ContentSize, "license", CmaKey);
            licensePsvmd.Close();
            licensePsvimg.Close();

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

            transVita.Enabled = true;
            driveLetterDst.Enabled = true;
            driveLetterSrc.Enabled = true;
            pspFolder.ReadOnly = false;
            cmaDir.ReadOnly = false;
            pspGames.Enabled = true;
        }

    }
}
