using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RemoteDesktop
{
    public class StoreManager
    {
        // Save credentials except password to XML
        public void SaveCredentials(List<ConnectionCredentials> credentials)
        {
            DeletePW(credentials);
            XmlSerializer serializer = new XmlSerializer(typeof(List<ConnectionCredentials>));

            // Serialize List of Connnection Credentials to XML
            using (FileStream stream = File.OpenWrite(Directory.GetCurrentDirectory() + @"\" + fileName))
            {
                serializer.Serialize(stream, credentials);
            }
        }

        // Return the list of saved credentials from XML file
        public List<ConnectionCredentials> LoadCredentials()
        {
            List<ConnectionCredentials> credentials = new List<ConnectionCredentials>(); 
            if (VerifyIfFileExists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ConnectionCredentials>));

                // Desirialize list of credentials from XML and cast to method return type
                using (FileStream stream = File.OpenRead(Directory.GetCurrentDirectory() + @"\" + fileName))
                {
                    credentials = (List<ConnectionCredentials>)serializer.Deserialize(stream);
                }
            }

            return credentials;
        }

        // Create XML file if it doesn't already exist
        private void CreateFileIfNeeded()
        {
            if (!VerifyIfFileExists(fileName))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                {
                    writer.Dispose();
                }
            }
        }

        // Return true if file exists otherwise false
        private bool VerifyIfFileExists(string fileName)
        {
            string fullFilePath = string.Concat(Directory.GetCurrentDirectory(), @"\", fileName);
            return File.Exists(fullFilePath);
        }

        // We do not want to store PW so they will be deleted from credentials list
        private void DeletePW(List<ConnectionCredentials> credentials)
        {
            credentials.ForEach(x => x.UserPassword = string.Empty);
        }

        // Name of the file that will store our data
        private const string fileName = "ucs.xml";
    }
}
