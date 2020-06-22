using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NitroxModel.DataStructures
{
    class SaveProfile
    {
        #region Constants
        /// <summary>
        /// The foldername where Nitrox SaveProfiles are stored
        /// </summary>
        public const string ProfileFolder = "Profiles";
        #endregion

        #region StaticProperties
        public static string SettingsPath { get; set; }

        public static List<SaveProfile> Profiles { get; private set; }
        #endregion

        #region Properties
        public string ProfileName { get; private set; }
        public string ProfilePath { get; private set; }

        /// <summary>
        /// The path to the profiles "world" folder
        /// </summary>
        public string WorldPath
        {
            get
            {
                return Path.Combine(ProfilePath, "world");
            }
        }

        /// <summary>
        /// The path to the profiles "dev" folder
        /// </summary>
        public string DevPath
        {
            get
            {
                return Path.Combine(ProfilePath, "dev");
            }
        }

        /// <summary>
        /// The path to the servers.list file
        /// </summary>
        public string ServerListPath
        {
            get
            {
                return Path.Combine(ProfilePath, "servers.lst");
            }
        }

        /// <summary>
        /// The profiles serverList
        /// </summary>
        public List<string> ServerList { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new SaveProfile with given name or loads if existent
        /// </summary>
        /// <param name="name"></param>
        public SaveProfile(string name)
        {
            ProfileName = name;
            ServerList = new List<string>();

            string ProfilePath = Path.Combine(SettingsPath, ProfileFolder, ProfileName);
            if (!Directory.Exists(ProfilePath))
            {
                Directory.CreateDirectory(ProfilePath);
                Directory.CreateDirectory(Path.Combine(ProfilePath, "world"));
                Directory.CreateDirectory(Path.Combine(ProfilePath, "dev"));
            }
            else
            {
                LoadServerList();
            }
        }
        #endregion

        #region PublicMember
        /// <summary>
        /// Adds a server ip to the profiles server list
        /// </summary>
        /// <param name="serverIp"></param>
        public void AddServer(string serverIp)
        {
            if(!ServerList.Contains(serverIp))
            {
                ServerList.Add(serverIp);
            }
        }

        /// <summary>
        /// Removes the profile from disk
        /// </summary>
        public void Delete()
        {
            Directory.Delete(ProfilePath, true);
            if(Profiles.Contains(this))
            {
                Profiles.Remove(this);
            }
        }
        
        /// <summary>
        /// Saves the current profile settings to disk
        /// </summary>
        public void SaveToDisk()
        {
            SaveServerList();
        }
        #endregion

        #region PublicStatics
        /// <summary>
        /// Returns the "default" profile
        /// </summary>
        /// <returns></returns>
        public static SaveProfile GetDefaultProfile()
        {
            return SaveProfile("default");
        }
        
        /// <summary>
        /// Gets a list of all existing Profiles
        /// </summary>
        /// <returns></returns>
        public static List<SaveProfile> GetProfiles()
        {
            Profiles = new List<SaveProfile>();

            // List all directories in Profiles folder
            foreach (string profile in Directory.EnumerateDirectories(Path.Combine(SettingsPath, ProfileFolder)))
            {
                SaveProfile tmp = new SaveProfile(profile, Path.Combine(SettingsPath, ProfileFolder, profile));

                if (tmp.SanityCheck)
                {
                    Profiles.Add(tmp);
                }
            }
            return Profiles;
        }
        #endregion

        #region PrivateMember

        #region Save
        private void SaveServerList()
        {
            if (File.Exists(ServerListPath))
            {
                try
                {
                    File.Delete(ServerListPath);
                }
                catch
                {
                    return;
                }
            }

            File.WriteAllLines(ServerListPath, ServerList.ToArray());
        }
        #endregion

        #region Load
        private void LoadServerList()
        {
            if (File.Exists(ServerList))
            {
                ServerList = File.ReadAllLines(ServerList);
            }
            else
            {
                ServerList = new List<string>();
            }
        }

        private bool SanityCheck()
        {
            bool sane = File.Exists(ServerListPath);
            sane &= Directory.Exists(ProfileFolder);
            sane &= Directory.Exists(WorldPath);
            sane &= Directory.Exists(DevPath);
            
            return sane;
        }
        #endregion

        #endregion
    }
}
