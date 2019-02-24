using Microsoft.CommonLib;
using Microsoft.ConfigReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace BlueprintMgmt
{
    public class BlueprintMgr
    {
        private const string AuthScheme = "Bearer";
        private IConfigReader _config;
        private string _mgmtResource;
        private string _token;
        private string _tenantId;
        private string _mgmtGroupId;
        private string _subscriptionId;
        private string _objectId;

        public BlueprintMgr(IConfigReader config)
        {
            _config = config;
            Initialize();
            Authenticate();
        }

        private void Initialize()
        {
            _mgmtResource = _config["MgmtResource"];
            _tenantId = _config["ida:TenantId"];
            _mgmtGroupId = _config["ManagementGroupId"];
            _objectId = _config["ida:ObjectId"];
            _subscriptionId = _config["SubscriptionId"];
        }

        private void Authenticate()
        {
            var auth = new Auth(_config);
            _token = auth.Authenticate(_mgmtResource).Result;
        }

        private StringContent GetContent(string blueprintName, string data)
        {
            data = data.Replace("{ManagementGroupId}", _mgmtGroupId);
            data = data.Replace("{SubscriptionId}", _subscriptionId);
            data = data.Replace("{BlueprintName}", blueprintName);
            data = data.Replace("{ObjectId}", _objectId);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            return content;
        }

        public async Task<string> GetAzureBlueprintPrincipal()
        {
            var adGraphSPUrl = string.Format(_config["ADGraphSPFormat"], _tenantId);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var result = await client.GetStringAsync(adGraphSPUrl);
            return result;
        }

        public async Task<string> Get(string blueprintName)
        {
            var client = new HttpClient();

            var getUrl = String.Format(_config["GetUrlFormat"], _mgmtGroupId, blueprintName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var result = await client.GetStringAsync(getUrl);
            return result;
        }

        public async Task<string> GetArtifacts(string blueprintName)
        {
            var client = new HttpClient();

            var getUrl = String.Format(_config["GetArtifactsUrlFormat"], _mgmtGroupId, blueprintName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var result = await client.GetStringAsync(getUrl);
            return result;
        }

        public async Task<string> GetArtifact(string blueprintName, string artifactName)
        {
            var client = new HttpClient();

            var getUrl = String.Format(_config["GetArtifactUrlFormat"], _mgmtGroupId, blueprintName, artifactName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var result = await client.GetStringAsync(getUrl);
            return result;
        }

        public async Task<string> CreateBlueprint(string blueprintName, string blueprintData)
        {

            var client = new HttpClient();

            var createUrl = string.Format(_config["CreateBlueprintUrlFormat"], _mgmtGroupId, blueprintName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);


            var content = GetContent(blueprintName, blueprintData);

            var response = await client.PutAsync(createUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> AddArtifact(string blueprintName, string artifactName, string artifactData)
        {

            var client = new HttpClient();

            var artifactUrl = string.Format(_config["AddArtifactUrlFormat"], _mgmtGroupId, blueprintName, artifactName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var content = GetContent(blueprintName, artifactData);

            var response = await client.PutAsync(artifactUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> Publish(string blueprintName, string version)
        {
            var client = new HttpClient();

            var assignUrl = string.Format(_config["PublishUrlFormat"], _mgmtGroupId, blueprintName, version);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var response = await client.PutAsync(assignUrl, null);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> Assign(string blueprintName, string assignmentName, string assignmentData)
        {
            var client = new HttpClient();

            var assignUrl = string.Format(_config["AssignUrlFormat"], _subscriptionId, assignmentName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var content = GetContent(blueprintName, assignmentData);

            var response = await client.PutAsync(assignUrl, content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<string> Unassign(string blueprintAssignmentName)
        {
            var client = new HttpClient();

            var unassignUrl = string.Format(_config["UnassignUrlFormat"], _subscriptionId, blueprintAssignmentName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var response = await client.DeleteAsync(unassignUrl);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public async Task<string> Delete(string blueprintName)
        {
            var client = new HttpClient();

            var deleteUrl = string.Format(_config["DeleteUrlFormat"], _mgmtGroupId, blueprintName);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, _token);

            var response = await client.DeleteAsync(deleteUrl);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
