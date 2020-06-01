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

        public List<string> ServerList { get; private set; }
        #endregion

        #region Constructor
        public SaveProfile(string name)
        {
            ProfileName = name;
            ServerList = new List<string>();

            // Create Folderstructure
            string ProfilePath = Path.Combine(SettingsPath, ProfileFolder, ProfileName);
            Directory.CreateDirectory(ProfilePath);
            Directory.CreateDirectory(Path.Combine(ProfilePath, "world"));
            Directory.CreateDirectory(Path.Combine(ProfilePath, "dev"));
        }

        public SaveProfile(string name, string path)
        {
            ProfileName = name;
            ServerList = new List<string>();

            // PARTY
            // Add clean reading of servers
            ServerList = File.ReadAllLines(Path.Combine(path, "servers"));
        }
        #endregion


        #region PublicMember
        public string GetWorldPath()
        {
            return Path.Combine(ProfilePath, "world");
        }

        public string GetDevPath()
        {
            return Path.Combine(ProfilePath, "dev");
        }
        
        public void AddServer(string serverIp)
        {
            if(!ServerList.Contains(serverIp))
            {
                ServerList.Add(serverIp);
            }
        }

        public void Delete()
        {
            Directory.Delete(ProfilePath, true);
            if(Profiles.Contains(this))
            {
                Profiles.Remove(this);
            }
        }
        #endregion

        #region PublicStatics
        public static SaveProfile GetOrCreate(string name)
        {
            if (Profiles == null)
            {
                GetProfiles();
            }

            SaveProfile found = Profiles.Find(x => x.ProfileName == name);
            
            if (found != null)
                return found;
            else
                return new SaveProfile(name);
        }

        public static SaveProfile GetDefaultProfile()
        {
            return GetOrCreate("default");
        }
        
        public static List<SaveProfile> GetProfiles()
        {
            Profiles = new List<SaveProfile>();

            // List all directories in Profiles folder
            foreach (string profile in Directory.EnumerateDirectories(Path.Combine(SettingsPath, ProfileFolder)))
            {
                Profiles.Add(new SaveProfile(profile, Path.Combine(SettingsPath, ProfileFolder, profile)));
            }
            return Profiles;
        }
        #endregion
    }
}
