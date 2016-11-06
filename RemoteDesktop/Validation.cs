using AxMSTSCLib;
using System.Collections.Generic;

namespace RemoteDesktop
{
    public class Validation
    {
        // Very simple Validation 
        // it will look if all connection credentials are not empty
        // returns true if validation succeded otherwise false
        public bool Validate(ConnectionCredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.ServerIP) ||
                string.IsNullOrWhiteSpace(credentials.UserName) ||
                string.IsNullOrWhiteSpace(credentials.UserPassword))
            {
                return false;
            }

            return true;
        }

        // Validate if remote object is in connected state
        public bool ValidateConnection(AxMsTscAxNotSafeForScripting remote)
        {
            bool connected;

            return connected = remote.Connected.ToString() == "1";
        }

        // Verify if credentials already exist in list
        public bool AreCredentialsDuplicate(ConnectionCredentials credentials, List<ConnectionCredentials> credentialsList)
        {
            foreach (var item in credentialsList)
            {
                if (credentials.ServerIP == item.ServerIP && credentials.UserName == item.UserName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
