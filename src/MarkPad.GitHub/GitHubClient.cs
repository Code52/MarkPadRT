using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MarkPad.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Foundation;
using Windows.Security.Authentication.Web;

namespace MarkPad.GitHub
{
    public class GitHubClient : ISource
    {
        private const string ClientId = "ed0cdbf084078f60b8a3";
        private const string ClientSecret = "a0a442815b7530386c90088a98cfd018877624d2";
        private const string RedirectUri = "http://vikingco.de";
        private const string Accesstokenuri = "https://github.com/login/oauth/access_token";
        private const string ApiBaseUrl = "https://api.github.com/";
        private string AccessToken;

        private async Task<string> GetMaster(string user, string repo)
        {
            //GET  /repos/:user/:repo/git/refs/heads/master
            return "";
        }

        //GET /repos/:user/:repo/git/commits/SHA-LATEST-COMMIT

        //POST /repos/:user/:repo/git/trees/

        private async Task CreateTree()
        {

        }

        public async Task<bool> Login()
        {
            var url = "https://github.com/login/oauth/authorize?client_id=" + Uri.EscapeDataString(ClientId) + "&redirect_uri=" + Uri.EscapeDataString(RedirectUri) + "&scope=repo&display=popup&response_type=token";
            var startUri = new Uri(url);
            var endUri = new Uri(RedirectUri);
            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
            string code = "";
            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var decoder = new WwwFormUrlDecoder(webAuthenticationResult.ResponseData.Replace(RedirectUri + "/?", ""));
                    code = decoder.First(x => x.Name == "code").Value;
                    break;
                case WebAuthenticationStatus.ErrorHttp:
                    return false;
                    break;
                default:
                    return false;
                    break;
            }
            var c = new HttpClient();
            var data = new Dictionary<string, string>
                           {
                               {"client_id", ClientId},
                               {"client_secret", ClientSecret},
                               {"code", code}
                           };
            var content = new FormUrlEncodedContent(data);
            var request = await c.PostAsync(Accesstokenuri, content);
            var result = await request.Content.ReadAsStringAsync();
            AccessToken = new WwwFormUrlDecoder(result).GetFirstValueByName("access_token");
            return true;
        }

        public IFile GetFile()
        {
            return null;
        }

        public IEnumerable<IFile> GetFiles()
        {
            return null;
        }

        private string GetUrl(string path)
        {
            return string.Format("{0}{1}?access_token={2}", ApiBaseUrl, path, AccessToken);
        }

        public async Task SaveFile(string name, string contents, string user, string repo)
        {
            var c = new HttpClient();

            var refquery = await c.GetAsync(GetUrl(string.Format("repos/{0}/{1}/git/refs/heads/master", user, repo)));
            var SHA_LATEST_COMMIT = JObject.Parse(await refquery.Content.ReadAsStringAsync())["object"]["sha"].ToString();

            var baseTreeQuery = await c.GetAsync(GetUrl(string.Format("repos/{0}/{1}/git/commits/{2}", user, repo, SHA_LATEST_COMMIT)));
            var SHA_BASE_TREE = JObject.Parse(await baseTreeQuery.Content.ReadAsStringAsync())["tree"]["sha"].ToString();

            //Create a new tree
            string url = "/repos/:user/:repo/git/trees/";
            var tree = new
            {
                base_tree = SHA_BASE_TREE,
                tree = new[] 
                  {  
                      new
                        {
                            path = name,
                            mode = "100644",
                            type = "blob",
                            content = contents
                        }
                  }
            };

            var newTreeQuery = await c.PostAsync(GetUrl(string.Format("repos/{0}/{1}/git/trees", user, repo)), new StringContent(JsonConvert.SerializeObject(tree)));
            var SHA_NEW_TREE = JObject.Parse(await newTreeQuery.Content.ReadAsStringAsync())["sha"].ToString();

            //Create a new commit
            var newCommit = new
                {
                    message = "my commit message",
                    parents = new[] { SHA_LATEST_COMMIT },
                    tree = SHA_NEW_TREE
                };
            var newCommitQuery = await c.PostAsync(GetUrl(string.Format("repos/{0}/{1}/git/commits", user, repo)), new StringContent(JsonConvert.SerializeObject(newCommit)));
            var newCommitResult = await newCommitQuery.Content.ReadAsStringAsync();
            var SHA_NEW_COMMIT = JObject.Parse(newCommitResult)["sha"].ToString();

            //finalise
            var content = "{\"sha\": \"" + SHA_NEW_COMMIT + "\"}";
            var newReferenceQuery = await c.PostAsync(GetUrl(string.Format("repos/{0}/{1}/git/refs/heads/master", user, repo)), new StringContent(content));
            var newReferenceResult = await newReferenceQuery.Content.ReadAsStringAsync();
        }


        public void SaveFile(IFile file)
        {

        }

        public void SaveFiles(IEnumerable<IFile> files)
        {

        }
    }

    //POST /repos/:user/:repo/git/commits

    ///repos/:user/:repo/git/refs/head/master 

    /*String of the file mode - one of 100644 for file (blob), 100755 for executable (blob), 040000 for subdirectory (tree), 160000 for submodule (commit) or 120000 for a blob that specifies the path of a symlink*/
}
