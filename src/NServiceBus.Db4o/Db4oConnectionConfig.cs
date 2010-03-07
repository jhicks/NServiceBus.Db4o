using System.Configuration;

namespace NServiceBus.Db4o
{
    public class Db4oConnectionConfig : ConfigurationSection
    {
        [ConfigurationProperty("DatabaseFile", IsRequired = false, DefaultValue = "db4o.yap")]
        public string DatabaseFile
        {
            get { return (string)this["DatabaseFile"]; }
            set { this["DatabaseFile"] = value; }
        }

        [ConfigurationProperty("Host", IsRequired = false, DefaultValue = "localhost")]
        public string Host
        {
            get { return (string)this["Host"]; }
            set { this["Host"] = value; }
        }

        [ConfigurationProperty("Port", IsRequired = false, DefaultValue = 2099)]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }

        [ConfigurationProperty("Username", IsRequired = false, DefaultValue = "db4o")]
        public string Username
        {
            get { return (string)this["Username"]; }
            set { this["Username"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = false, DefaultValue = "db4o")]
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("CurrentSessionContext", IsRequired = false, DefaultValue = "Db4o.Util.Impl.ThreadStaticCurrentSessionContext, Db4o.Util")]
        public string CurrentSessionContext
        {
            get { return (string)this["CurrentSessionContext"]; }
            set { this["CurrentSessionContext"] = value; }
        }
    }
}