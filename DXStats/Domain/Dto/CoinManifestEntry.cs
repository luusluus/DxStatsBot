using Newtonsoft.Json;
using System.Collections.Generic;

namespace DXStats.Domain.Dto
{
    public class CoinManifestEntry
    {
        public string Blockchain { get; set; }
        public string Ticker { get; set; }
        [JsonProperty("ver_id")]
        public string VersionId { get; set; }
        [JsonProperty("ver_name")]
        public string VersionName { get; set; }
        [JsonProperty("conf_name")]
        public string ConfigFileName { get; set; }
        [JsonProperty("dir_name_linux")]
        public string DirectoryNameLinux { get; set; }
        [JsonProperty("dir_name_mac")]
        public string DirectoryNameMac { get; set; }
        [JsonProperty("dir_name_win")]
        public string DirectoryNameWindows { get; set; }
        [JsonProperty("repo_url")]
        public string RepositoryUrl { get; set; }
        public List<string> Versions { get; set; }
        [JsonProperty("xbridge_conf")]
        public string XBridgeConfig { get; set; }
        [JsonProperty("wallet_conf")]
        public string WalletConfig { get; set; }
    }
}
