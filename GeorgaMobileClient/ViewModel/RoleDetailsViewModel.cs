/* GeoRGA Mobile Client -- a multi-platform mobile app for the
 * Geographic Resouce and Group Allocation project (https://georga.app/)
 * 
 * Copyright (C) 2023 Thomas Mielke D8AE2CE41CB1D1A61087165B95DC1917252AD305 
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using GeorgaMobileDatabase.Model;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using GraphQL.Client.Http;
using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Serializer.Newtonsoft;

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(RoleId), nameof(RoleId))]
public partial class RoleDetailsViewModel : RoleItemViewModel
{
    #region query property implementation
    private string roleId;
    public string RoleId
    {
        get => roleId;
        set
        {
            SetProperty(ref roleId, value);
            if (!GetRoleFromDb(roleId))
                Result = "Database integrity error: Couldn't find role with the specified ID.";
        }
    }
    #endregion

    #region view control

    public ICommand RefreshCommand { protected set; get; }
    public ICommand EditCommand { protected set; get; }
    public ICommand BackCommand { protected set; get; }
    public ICommand StoreCommand { protected set; get; }
    public ICommand CancelCommand { protected set; get; }

    [ObservableProperty]
    private bool isEditing;
    [ObservableProperty]
    private bool isEditEnabled;
    [ObservableProperty]
    private bool isRefreshEnabled;
    [ObservableProperty]
    private bool isStoreEnabled;
    [ObservableProperty]
    private bool isCancelEnabled;
    [ObservableProperty]
    string result;

    #endregion

    #region constructor

    IConfiguration configuration;
    NetworkSettings settings;

    public RoleDetailsViewModel()
    {
        configuration = MauiProgram.Services.GetService<IConfiguration>();
        settings = configuration.GetRequiredSection("NetworkSettings").Get<NetworkSettings>();

        RefreshCommand = new Command(Refresh);
        EditCommand = new Command(Edit);
        BackCommand = new Command(Back);
        StoreCommand = new Command(Store);
        CancelCommand = new Command(Cancel);
        IsRefreshEnabled = true;
        IsEditEnabled = true;
        IsStoreEnabled = false;
        IsCancelEnabled = true;
        IsEditing = false;

        Title = GeorgaMobileClient.Properties.Resources.Profile;
        Result = "";
        // QualificationCategories = new ObservableCollection<PersonPropertyGroup>();
        // organizations = new ObservableCollection<OrganizationViewModel>();
        // Qualifications = new ObservableCollection<PersonPropertyViewModel>();
        // qualificationsOfThisPerson = new List<string>();

        // Refresh();
    }

    #endregion


    #region commands

    async void Refresh()
    {
        if (!IsRefreshEnabled) return;
        IsRefreshEnabled = false;
        Result = "";

        SetBusy(true);

        // ToDo: not only refresh person options, but also basic profile data like email 

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)       // TODO: fallback to offline cache, if other network errors occur
        {
/*            await System.Threading.Tasks.Task.WhenAll(GetPersonOptions(), GetProfileDataFromApi());    // do two queries at once

            foreach (string personQualId in qualificationsOfThisPerson)         // set the Value member in 
                foreach (var qual in Qualifications)                            // Qualifications to true
                    if (qual.Id == personQualId)                                // if qualification is 
                    {                                                           // possessed by the person
                        qual.Value = true;
                        break;
                    }
            if (!await SavePersonToDb())
                Result = "Problem while saving profile data to local device.";*/
        }
        else
        {
 //           _ = await GetPersonFromDb();
        }

        SetBusy(false);

//        RaisePersonOptionsChangedEvent();
        IsRefreshEnabled = true;
    }

    public async void Edit()
    {
        if (!IsEditEnabled) return;
        IsEditEnabled = false;

        SetBusy(true);
/*        if (await GetProfileDataFromApi())
        {
            IsEditEnabled = true;
            IsStoreEnabled = true;
            IsCancelEnabled = true;
            IsEditing = true;
            Qualifications.ToList().ForEach(p => p.IsEditing = true);
        }
 */       SetBusy(false);
    }

    async void Back()
    {
        await Shell.Current.GoToAsync("///main");
    }

    // store form data
    async void Store()
    {
        if (!IsStoreEnabled) return;
        IsStoreEnabled = false;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            Result = "Connectivity Error: No internet connection currently.";
            IsStoreEnabled = true;
            return;
        }

        ValidateAllProperties();
        if (HasErrors)
        {
            Result = "Please provide valid data.";
            IsStoreEnabled = true;
            return;
        }

        SetBusy(true);
/*
        qualificationsOfThisPerson.Clear();
        foreach (var qual in Qualifications)
            if (qual.Value)
                qualificationsOfThisPerson.Add(qual.Id);

        if (!await SavePersonToDb())
            Result = "Problem while saving changes to local device.";
        else if (await SendProfileDataToApi())
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsEditing = false;
                Qualifications.ToList().ForEach(p => p.IsEditing = false);
            });
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsStoreEnabled = true;
                Qualifications.ToList().ForEach(p => p.IsEditing = true);
            });
        }
*/
        SetBusy(false);

        IsEditing = false;

        Refresh();
    }

    async void Cancel()
    {
        if (!IsCancelEnabled) return;
        IsCancelEnabled = false;

        Refresh();

        IsRefreshEnabled = true;
        IsEditEnabled = true;
        IsEditing = false;
//        Qualifications.ToList().ForEach(p => p.IsEditing = false);
    }

    #endregion


    #region api stuff

    async Task<bool> GetPersonOptions()
    {
        var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);

        var qualificationsRequest = new GraphQLRequest
        {
            Query = """
                        query GetPersonOptions {
                          listPersonPropertyGroups {
                            edges {
                              node {
                                id
                                organization { id }
                                codename
                                name
                              }
                            }
                          }
                          listPersonProperties {
                            edges {
                              node {
                                id
                                name
                                group {
                                  id
                                }
                              }
                            }
                          }
                        }
                        """
        };


        dynamic qualificationsResponse = null;
        try
        {
            qualificationsResponse = await graphQLClient.SendQueryAsync<dynamic>(qualificationsRequest);
//            if (QueryHasErrors(qualificationsResponse))
//                return false;
        }
        catch (GraphQLHttpRequestException e)
        {
            Result = e.Content;
            return false;
        }
        catch (Exception e)
        {
            if (qualificationsResponse?.Errors?.Length > 0)
                Result = qualificationsResponse.Errors[0].Message;
            else
                Result = e.Message;
            return false;
        }
/*
        var allQualificationCategories = qualificationsResponse?.Data?.listPersonPropertyGroups.edges.Children<JObject>();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            QualificationCategories.Clear();
            foreach (var qualificationCategory in allQualificationCategories)
            {
                QualificationCategories.Add(new PersonPropertyGroup()
                {
                    Id = qualificationCategory.node.id,
                    OrganizationId = qualificationCategory.node.organization.id,
                    Codename = qualificationCategory.node.codename,
                    Name = qualificationCategory.node.name
                });
            }
        });

        var listPersonProperties = qualificationsResponse?.Data?.listPersonProperties.edges.Children<JObject>();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Qualifications.Clear();
            foreach (var qualification in listPersonProperties)
            {
                Qualifications.Add(new PersonPropertyViewModel()
                {
                    Id = qualification.node.id,
                    GroupId = qualification.node.group.id,
                    Name = qualification.node.name,
                    Value = false
                });
            }
        });
*/
        /*var allRestrictions = qualificationsResponse?.Data?.allRestrictions.edges.Children<JObject>();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Restrictions.Clear();
            foreach (var restriction in allRestrictions)
            {
                Restrictions.Add(new Restriction()
                {
                    Id = restriction.node.id,
                    Name = restriction.node.name,
                });
            }
        });*/

        return true;
    }

    public async Task<bool> GetRoleDataFromApi()
    {
/*        var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
        var personRequest = new GraphQLRequest
        {
            Query = """
                    query getPersonProfile {
                        getPersonProfile {
                            id
                            firstName
                            lastName
                            email
                            properties {
                                edges {
                                    node {
                                        id
                                    }
                                }
                            }
                            organizationsSubscribed {
                                edges {
                                    node {
                                        id
                                        name
                                        icon
                                    }      
                                }
                            }
                        }
                    }
                    """
        };

        dynamic graphQLResponse = null;
        try
        {
            graphQLResponse = await graphQLClient.SendQueryAsync<dynamic>(personRequest);
            if (QueryHasErrors(graphQLResponse))
            {
                IsStoreEnabled = false;
                return false;
            }
            var getPersonProfile = graphQLResponse?.Data?.getPersonProfile;
            if (getPersonProfile == null)
            {
                IsStoreEnabled = false;
                return false;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Id = getPersonProfile.id;
                Email = getPersonProfile.email;
                FirstName = getPersonProfile.firstName;
                LastName = getPersonProfile.lastName;
                var qualificationEdges = getPersonProfile.properties.edges.Children<JObject>();
                qualificationsOfThisPerson.Clear();
                foreach (var qualification in qualificationEdges)
                    qualificationsOfThisPerson.Add(qualification.node.id.ToString());
                var organizationEdges = getPersonProfile.organizationsSubscribed.edges.Children<JObject>();
                organizations.Clear();
                foreach (var organization in organizationEdges)
                {
                    organizations.Add(new OrganizationViewModel()
                    {
                        Id = organization.node.id.ToString(),
                        Name = organization.node.name.ToString(),
                        Icon = organization.node.icon.ToString()
                    });
                }
            });
        }
        catch (GraphQLHttpRequestException e)
        {
            Result = e.Content;
            return false;
        }
        catch (Exception e)
        {
            if (graphQLResponse?.Errors?.Length > 0)
                Result = graphQLResponse.Errors[0].Message;
            else
                Result = e.Message;
            return false;
        }
        return true;
    }

    public async Task<bool> SendProfileDataToApi()
    {
        var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);
        var updatePersonRequest = new GraphQLRequest
        {
            Query = @"
  mutation UpdateProfile (
    $firstName: String
    $lastName: String
    $properties: [ID]
  ) {
    updatePersonProfile(
      input: {
        firstName: $firstName
        lastName: $lastName
        properties: $properties
      }
    ) {
      person {
        id
      }
      errors {
        field
        messages
      }
    }
  }",
            Variables = new
            {
                firstName = FirstName,
                lastName = LastName,
                properties = qualificationsOfThisPerson
            }
        };

        dynamic jwtResponse = null;
        try
        {
            jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(updatePersonRequest);
            if (QueryHasErrors(jwtResponse))
                return false;
        }
        catch (GraphQLHttpRequestException e)
        {
            Result = e.Content;
            return false;
        }
        catch (Exception e)
        {
            if (jwtResponse?.Errors?.Length > 0)
                Result = jwtResponse.Errors[0].Message;
            else
                Result = e.Message;
            return false;
        }
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
                    if (error.Path != null)
                    {
                        Result += $"\r\nQuery error '{error.Path[0]}'";
                        if (error.Locations != null)
                            Result += $" (line {error.Locations[0].Line},column {error.Locations[0].Column})";
                        Result += ": ";
                    }
                    else if (error.field != null)
                        Result += $"\r\nField '{error.field}': ";
                    else
                        Result += $"\r\n";
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

    #endregion

    #region database stuff

    public async Task<bool> SavePersonToDb()
    {
/*        if (String.IsNullOrEmpty(Id))  // is there valid data?
            return false;

        bool createNewRecord = false;
        var thisPerson = await Db.GetPersonByEmail(App.Instance.User.Email);    // record exists? if not,
        if (thisPerson == null)                                                 // create new db entry
        {
            thisPerson = new Person();
            thisPerson.Email = App.Instance.User.Email;
            createNewRecord = true;
        }

        thisPerson.Id = Id;
        thisPerson.FirstName = FirstName;
        thisPerson.LastName = LastName;
        thisPerson.Properties = "";

        int recordsCreated = 0;
        int recordsUpdated = 0;
        if (createNewRecord)
            recordsCreated = await Db.SavePersonAsync(thisPerson);
        else
            recordsUpdated = await Db.UpdatePersonAsync(thisPerson);
        Console.WriteLine($"{recordsCreated} record newly created, {recordsUpdated} existing records updated.");
*/
        return true;
    }

    public bool GetRoleFromDb(string roleId)
    {
        var roleTask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Role>(async () => await Db.GetRoleById(roleId));
        var role = roleTask.Result;
        if (role != null)
        {
            Id = role.Id;
            Acceptance = "";
            Name = role.Name;
            Description = role.Description;
            Quantity = role.Quantity;
            IsActive = role.IsActive;  // ? "" : " (inactive)";
            IsTemplate = role.IsTemplate;  // ? "template" : "";
            return true;
        }
        else
            return false;
    }

    #endregion
}
