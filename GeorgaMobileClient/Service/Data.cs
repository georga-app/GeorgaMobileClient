using GeorgaMobileDatabase;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static SQLite.SQLite3;

namespace GeorgaMobileClient.Service;

public class DataTemplate
{
    public string Query;
}

public partial class Data
{
    const string _androidEndpoint = "http://10.0.2.2:80/graphql";
    const string _otherPlatformsEndpoint = "http://localhost:80/graphql";

    Database _db = DependencyService.Get<Database>();
    List<DataTemplate> _templates;
    string _querystring;
    GraphQLHttpClient _graphQLClient;

    public string Result;

    public Data()
    {   
        // init graphQL client
        _graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? _androidEndpoint : _otherPlatformsEndpoint, new NewtonsoftJsonSerializer());

        // add all models
        _templates = new List<DataTemplate>();

        // ___ add all models to the query ___
        AddProject();
        AddPerson();
        AddOperation();
        AddOrganization();

        // build request
        var sb = new StringBuilder();
        sb.AppendLine(@"query GetAll {");  //  ($email: String)
        foreach (var template in _templates)
            sb.AppendLine(template.Query);
        sb.AppendLine(@"}");
        _querystring = sb.ToString();
    }

    public void SetAuthToken()
    {
        _graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
    }

    public async Task<bool> CacheAll()
    {
        var request = new GraphQLRequest
        {
            Query = _querystring/*,
            Variables = new
            {
                email = App.Instance.User.Email
            }*/
        };

        dynamic response = null;
        try
        {
            response = await _graphQLClient.SendQueryAsync<dynamic>(request);
            if (QueryHasErrors(response))
                return false;
        }
        catch (GraphQLHttpRequestException e)
        {
            Result = e.Content;
            return false;
        }
        catch (Exception e)
        {
            if (response?.Errors?.Length > 0)
                Result = response.Errors[0].Message;
            else
                Result = e.Message;
            return false;
        }

        // ___ save all models to the database ___
        await SaveProjectToDb(response);
        await SavePersonToDb(response);
        await SaveOperationToDb(response);
        await SaveOrganizationToDb(response);

        return true;
    }

    private bool QueryHasErrors(dynamic obj)
    {
        if (obj == null)
        {
            Result = "Application error (object is null)";  // this shouldn't happen
            return true;
        }
        dynamic errors;
        try
        {
            errors = obj.Errors;
        }
        catch (Exception e)
        {
            return false;
        }

        if (errors?.Length > 0)
        {
            Result = "";
            foreach (dynamic error in errors)
            {
                try
                {
                    Result += $"\r\nField '{error.field}': ";
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
                {
                    Result += $"\r\n";
                }

                try
                {
                    JArray messages = error.messages;
                    foreach (var message in messages)
                        Result += $"{message}\r\n";
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException e)
                {
                    Result += $"{error.Message}\r\n";
                }
            }
            return true;
        }
        else
            return false;
    }
}
