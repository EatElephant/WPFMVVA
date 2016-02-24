using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace BackendGUI.Models
{

    class AccountManager
    {
        //Path to where the accounts stored
        const string ACCOUNTS_PATH = @"C:\API\ImportantData\";
        const string FILENAME = ACCOUNTS_PATH + "AccountsList";

        static public AccountManager GlobalAccountManager = null;
        ObservableCollection<Account> _accounts = null;

        public AccountManager()
        {
            GlobalAccountManager = this;
            _accounts = new ObservableCollection<Account>();

            LoadStorage();
        }

        public void NewAccount()
        {
            _accounts.Add(new Account("UserName", "Password"));

            UpdateStorage();
        }

        public void DeleteAccount(int index)
        {
            if (_accounts == null)
            {
                MessageBox.Show("No existing accounts are found, deleting acount fails!", "BackendGUI");
                return;
            }
            if (index < 0 || index >= _accounts.Count)
            {
                MessageBox.Show("Incorrect index, deleting account fails!", "BackendGUI");
                return;
            }
            else
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete the the selected account?", "BackendGUI", System.Windows.Forms.MessageBoxButtons.YesNo))
                    _accounts.RemoveAt(index);
            }

            UpdateStorage();
        }

        public void UpdateStorage()
        {
            XElement root = new XElement("AccountsList");

            foreach (Account account in _accounts)
            {
                root.Add(new XElement("Account", new XAttribute("Username", account.Username), new XAttribute("Password",account.Password)));
            }

            if (!Directory.Exists(ACCOUNTS_PATH))
                Directory.CreateDirectory(ACCOUNTS_PATH);

            if (File.Exists(FILENAME))
                File.Delete(FILENAME);

            root.Save(FILENAME);

            EncryptFile(FILENAME);

            //File.SetAttributes(FILENAME, FileAttributes.Hidden);
        }

        public void LoadStorage()
        {
            if(File.Exists(FILENAME))
            {
                DecryptFile(FILENAME);

                XElement root = XElement.Load(FILENAME);

                foreach(XElement node in root.Elements("Account"))
                {
                    _accounts.Add(new Account((string)node.Attribute("Username"), (string)node.Attribute("Password")));
                }

                EncryptFile(FILENAME);
            }
        }

        string keyFile = ACCOUNTS_PATH + "api.data";
        string IVFile = ACCOUNTS_PATH + "api2.data";
        public void EncryptFile(string filename)
        {
            if(File.Exists(filename))
            {
                string originString = File.ReadAllText(filename);

                using (RijndaelManaged myRijndael = new RijndaelManaged())
                {
                    myRijndael.GenerateKey();
                    myRijndael.GenerateIV();

                    File.WriteAllBytes(keyFile, myRijndael.Key);

                    File.WriteAllBytes(IVFile, myRijndael.IV);

                    byte[]encryptedContent = MyRijndael.EncryptStringToBytes(originString, myRijndael.Key, myRijndael.IV);

                    File.WriteAllBytes(filename, encryptedContent);

                    //File.SetAttributes(keyFile, FileAttributes.Hidden);
                    //File.SetAttributes(IVFile, FileAttributes.Hidden);
                }
            }
        }

        public void DecryptFile(string filename)
        {
            if (File.Exists(filename) && File.Exists(keyFile) && File.Exists(IVFile))
            {
                byte[] key = File.ReadAllBytes(keyFile);
                byte[] IV = File.ReadAllBytes(IVFile);

                byte[] encryptedContent = File.ReadAllBytes(filename);

                string originString = MyRijndael.DecryptStringFromBytes(encryptedContent, key, IV);

                File.WriteAllText(filename, originString);
            }

        }



        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set { _accounts = value;}
        }
    }

    class Account
    {
        string _username;
        string _password;

        public Account(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string Username
        {
            get { return _username; }
            set 
            {
                _username = value;

                if (AccountManager.GlobalAccountManager != null)
                    AccountManager.GlobalAccountManager.UpdateStorage();
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;

                if (AccountManager.GlobalAccountManager != null)
                    AccountManager.GlobalAccountManager.UpdateStorage();
            }
        }

    }

    class MyRijndael
    {
        static public byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static public string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
