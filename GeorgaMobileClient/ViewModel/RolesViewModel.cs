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
using GeorgaMobileDatabase.Model;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using static SQLite.SQLite3;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic.Devices;

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(ShiftId), nameof(ShiftId))]
public partial class RolesViewModel : DatabaseViewModel
{
    IConfiguration configuration;
    NetworkSettings settings;

    public RolesViewModel()
    {
        configuration = MauiProgram.Services.GetService<IConfiguration>();
        settings = configuration.GetRequiredSection("NetworkSettings").Get<NetworkSettings>();
    }

    private string shiftId;
    public string ShiftId
    {
        get => shiftId;
        set
        {
            SetProperty(ref shiftId, value);
            Roles = new ObservableCollection<RoleDetailsViewModel>();
            var thisShiftTask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Shift>(async () => await Db.GetShiftById(shiftId));
            var thisShift = thisShiftTask.Result;
            string startTimeText = "this Shift";
            string endTimeText = "";
            if (thisShift != null)
            {
                startTimeText = DateOnly.FromDateTime(thisShift.StartTime.DateTime).ToString() + "    " + TimeOnly.FromDateTime(thisShift.StartTime.DateTime);
                endTimeText = thisShift.EndTime.DateTime.ToString();
                if (DateOnly.FromDateTime(thisShift.StartTime.DateTime) == DateOnly.FromDateTime(thisShift.EndTime.DateTime))  // only display end date, if endtime is not on same day as starttime
                    endTimeText = TimeOnly.FromDateTime(thisShift.EndTime.DateTime).ToString();
                Title = $"Roles for {startTimeText} - {endTimeText}";
            }
            else
                Title = "Roles";
            var shiftRoles = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Role>>(async () => await Db.GetRolesByShiftId(shiftId));
            var roles = shiftRoles.Result;
            if (roles == null) return;
            foreach (var role in roles)
            {
                var roleItem = new RoleDetailsViewModel()
                {
                    Id = role.Id,
                    Acceptance = "",
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive ? "" : " (inactive)",
                    IsTemplate = role.IsTemplate ? "template" : "",
                    NeedsAdminAcceptance = role.NeedsAdminAcceptance ? "needs admin acceptance" : "",
                    StartTime = startTimeText,
                    EnrollmentDeadline = thisShift.EnrollmentDeadline.DateTime.ToString(),
                    EndTime = endTimeText,
                    State = thisShift.State
                };
                roleItem.HelpersNeeded = role.Quantity;
                roleItem.ParticipantsAccepted = role.ParticipantsAccepted;
                roleItem.ParticipantsPending = role.ParticipantsPending;
                roleItem.ParticipantsDeclined = role.ParticipantsDeclined;

                var acceptanceTask = System.Threading.Tasks.Task.Run<Participant>(
                        async () => await Db.GetParticipantByPersonIdAndRoleId(App.Instance.User.Id, role.Id)
                    );
                    var participant = acceptanceTask.Result;
                    if (participant.Acceptance == "ACCEPTED")  // rejected status for one rule can't be overruled by rejected status for another rule
                    {
                        roleItem.Acceptance = participant.Acceptance;
                    }

                    if (participant.Acceptance != "")
                        roleItem.Acceptance = participant.Acceptance;  // TODO: clarify state priorities for multiple roles with different states

                Roles.Add(roleItem);
            }
        }
    }

    [ObservableProperty]
    string name;
    [ObservableProperty]
    string description;

    [ObservableProperty]
    ObservableCollection<RoleDetailsViewModel> roles;

    string Result;
    int currentItemIndex;

    public async System.Threading.Tasks.Task SelectItem(int itemIndex)
    {
        var shiftRolesTask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Role>(
            async () => await Db.GetRoleById(Roles[itemIndex].Id)
        );
        var shiftRole = shiftRolesTask.Result;
        if (shiftRole == null)
        {
            await Application.Current.MainPage.DisplayAlert("Data Integrity Error", $"Couldn't find role record for this shift, so can't participate.", "OK");
            return;
        }

        // set a transient state and trigger API call to change acceptance state on the server
        currentItemIndex = itemIndex;
        string acceptance = Roles[itemIndex].Acceptance;
        if (acceptance == "ACCEPTING" || acceptance == "ACCEPTED")
        {
            Roles[itemIndex].Acceptance = "DECLINING";  // transient
            Participate(shiftRole.Id, App.Instance.User.Id, "DECLINED");
        }
        else
        {
            Roles[itemIndex].Acceptance = "ACCEPTING"; // transient
            Participate(shiftRole.Id, App.Instance.User.Id, "ACCEPTED");
        }         
    }

    private async void Participate(string roleId, string personId, string acceptance)
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var participantTask = System.Threading.Tasks.Task.Run<Participant>(
                async () => await Db.GetParticipantByPersonIdAndRoleId(App.Instance.User.Id, roleId)
            );
            var participantInDb = participantTask.Result;

            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);

            GraphQLRequest jwtRequest;
            if (participantInDb == null)
                jwtRequest = new GraphQLRequest
                {
                    Query = """
					mutation CreateParticipant (
						$roleId: ID!
						$personId: ID!
						$acceptance: String!
					) 
					{
					    createParticipant (
					        input: {
					            role: $roleId
					            person: $personId
					            acceptance: $acceptance
					            adminAcceptance: "PENDING"
					        }
					    ) {
					        errors {
					            field
					            messages
					        }
					        participant {
					            id
					        }
					    }
					}
					""",
                    Variables = new
                    {
                        roleId,
                        personId,
                        acceptance
                    }
                };
            else
                jwtRequest = new GraphQLRequest
                {
                    Query = """
					    mutation UpdateParticipant (
					        $participantId: ID!
					        $acceptance: String
					    ) 
					    {
					        updateParticipant (
					            input: {
					                id: $participantId
					                acceptance: $acceptance
					            }
					        ) {
					            errors {
					                field
					                messages
					            }
					            participant {
					                id
					            }
					        }
					    }
					    """,
                    Variables = new
                    {
                        participantId = participantInDb.Id,
                        acceptance
                    }
                };

            dynamic jwtResponse = null;
            try
            {
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
                if (QueryHasErrors(jwtResponse))
                    await Application.Current.MainPage.DisplayAlert("Error while applying for shift", Result, "OK");
                else
                    Roles[currentItemIndex].Acceptance = acceptance;
            }
            catch (GraphQLHttpRequestException e)
            {
                Result = e.Content;
                return;
            }
            catch (Exception e)
            {
                if (jwtResponse?.Errors?.Length > 0)
                {
                    Result = jwtResponse.Errors[0].Message;
                }
                else
                {
                    if (e is SQLite.SQLiteException)
                        Result = $"Devices database reports '{e.Message}'";
                    else
                        Result = e.Message;
                }
                return;
            }
        }
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

        if (errors?.Length > 0)  // first try to resolve error on query level
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
        {
            Result = "";
            if (obj.Data != null)
            {
                foreach (JProperty queryResult in obj.Data.Children())
                {

                    foreach (dynamic error in queryResult.Value["errors"])
                    {
                        try
                        {
                            Result += $"\r\nDatabase server query '{queryResult.Name}' / Field '{error.field}': ";
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
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
