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

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(TaskId), nameof(TaskId))]
public partial class ShiftsViewModel : DatabaseViewModel
{
    IConfiguration configuration;
    NetworkSettings settings;

    public ShiftsViewModel()
    {
        configuration = MauiProgram.Services.GetService<IConfiguration>();
        settings = configuration.GetRequiredSection("NetworkSettings").Get<NetworkSettings>();
    }

    private string taskId;
    public string TaskId
    {
        get => taskId;
        set
        {
            SetProperty(ref taskId, value);
            Shifts = new ObservableCollection<ShiftDetailsViewModel>();
            var thisMetatask = System.Threading.Tasks.Task.Run<GeorgaMobileDatabase.Model.Task>(async () => await Db.GetTaskById(taskId));
            var thisTask = thisMetatask.Result;
            if (thisTask != null)
                Title = $"Shifts for {thisTask.Name}";
            else
                Title = "Shifts";
            var opsShift = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Shift>>(async () => await Db.GetShiftsByTaskId(taskId));
            var ops = opsShift.Result;
            if (ops == null) return;
            foreach (var op in ops)
            {
                var shift = new ShiftDetailsViewModel()
                {
                    Id = op.Id,
                    Acceptance = "",
                    StartTime = DateOnly.FromDateTime(op.StartTime.DateTime).ToString() + "    " + TimeOnly.FromDateTime(op.StartTime.DateTime),
                    EnrollmentDeadline = op.EnrollmentDeadline.DateTime.ToString(),
                    EndTime = op.EndTime.DateTime.ToString(),
                    State = op.State
                };
                if (DateOnly.FromDateTime(op.StartTime.DateTime) == DateOnly.FromDateTime(op.EndTime.DateTime))  // only display end date, if endtime is not on same day as starttime
                    shift.EndTime = TimeOnly.FromDateTime(op.EndTime.DateTime).ToString();

                // compute helpers needed
                var shiftRolesTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Role>>(async () => await Db.GetRolesByShiftId(shift.Id));
                var shiftRoles = shiftRolesTask.Result;
                int participantsAccepted = 0;
                int participantsPending = 0;
                int participantsDeclined = 0;
                int helpersNeeded = 0;
                if (shiftRoles != null)
                    foreach (var role in shiftRoles)
                    {
                        helpersNeeded += role.Quantity;
                        participantsAccepted += role.ParticipantsAccepted;
                        participantsPending += role.ParticipantsPending;
                        participantsDeclined += role.ParticipantsDeclined;
                    }
                shift.HelpersNeeded = helpersNeeded;
                shift.ParticipantsAccepted = participantsAccepted;
                shift.ParticipantsPending = participantsPending;
                shift.ParticipantsDeclined = participantsDeclined;

                // get acceptance status of user via role and participant tables
                foreach (var role in shiftRoles)
                {
                    var acceptanceTask = System.Threading.Tasks.Task.Run<Participant>(
                            async () => await Db.GetParticipantByPersonIdAndRoleId(App.Instance.User.Id, role.Id)
                        );
                    var participant = acceptanceTask.Result;
                    if (participant != null && participant.Acceptance == "ACCEPTED")  // rejected status for one rule can't be overruled by rejected status for another rule
                    {
                        shift.Acceptance = participant.Acceptance;
                    }

                    if (participant != null && participant.Acceptance != "")
                        shift.Acceptance = participant.Acceptance;  // TODO: clarify state priorities for multiple roles with different states
                }

                Shifts.Add(shift);
            }
        }
    }

    [ObservableProperty]
    string name;
    [ObservableProperty]
    string description;

    [ObservableProperty]
    ObservableCollection<ShiftDetailsViewModel> shifts;

    string Result;
    int currentItemIndex;

    public async System.Threading.Tasks.Task SelectItem(int itemIndex)
    {
        await Shell.Current.GoToAsync($"/roles?ShiftId={Uri.EscapeDataString(Shifts[itemIndex].Id)}");
        return;


        var shiftRolesTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Role>>(
            async () => await Db.GetRolesByShiftId(Shifts[itemIndex].Id)
        );
        var shiftRoles = shiftRolesTask.Result;
        if (shiftRoles == null || shiftRoles.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Data Integrity Error", $"Couldn't find role record for this shift, so can't participate.", "OK");
            return;
        }

        // set a transient state and trigger API call to change acceptance state on the server
        currentItemIndex = itemIndex;
        string acceptance = Shifts[itemIndex].Acceptance;
        if (acceptance == "ACCEPTING" || acceptance == "ACCEPTED")
        {
            Shifts[itemIndex].Acceptance = "DECLINING";  // transient
            Participate(shiftRoles.FirstOrDefault().Id, App.Instance.User.Id, "DECLINED");
        }
        else
        {
            Shifts[itemIndex].Acceptance = "ACCEPTING"; // transient
            Participate(shiftRoles.FirstOrDefault().Id, App.Instance.User.Id, "ACCEPTED");
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

            var jwtRequest = new GraphQLRequest
            {
                Query = (participantInDb.Acceptance == null ? "mutation CreateParticipant " : "mutation UpdateParticipant ") +
                    """
					(
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

            dynamic jwtResponse = null;
            try
            {
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
                if (QueryHasErrors(jwtResponse))
                    await Application.Current.MainPage.DisplayAlert("Error while applying for shift", Result, "OK");
                else
                    Shifts[currentItemIndex].Acceptance = acceptance;
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
