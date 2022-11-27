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

namespace GeorgaMobileClient.ViewModel;

[QueryProperty(nameof(TaskId), nameof(TaskId))]
public partial class ShiftsViewModel : DatabaseViewModel
{
    const string itemGlyph = "\uf133";  // or f073 ?
    const string itemGlyphCheck = "\uf274";
    const string itemGlyphNeutral = "\uf133";
    const string itemGlyphPlus = "\uf271";
    const string itemGlyphMinus = "\uf272";
    const string itemGlyphX = "\uf273";

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
            var opsShift = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Shift>>(async () => await Db.GetShiftByTaskId(taskId));
            var ops = opsShift.Result;
            if (ops == null) return;
            foreach (var op in ops)
            {
                var shift = new ShiftDetailsViewModel()
                {
                    Id = op.Id,
                    Glyph = itemGlyph,
                    StartTime = DateOnly.FromDateTime(op.StartTime.DateTime).ToString() + "    " + TimeOnly.FromDateTime(op.StartTime.DateTime),
                    EnrollmentDeadline = op.EnrollmentDeadline.DateTime.ToString(),
                    EndTime = op.EndTime.DateTime.ToString(),
                    State = op.State
                };
                if (DateOnly.FromDateTime(op.StartTime.DateTime) == DateOnly.FromDateTime(op.EndTime.DateTime))  // only display date, if endtime is not on same day as starttime
                    shift.EndTime = TimeOnly.FromDateTime(op.EndTime.DateTime).ToString();

                // compute helpers needed
                var shiftRolesTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Role>>(async () => await Db.GetRoleByShiftId(shift.Id));
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
                shift.Participants = participantsAccepted.ToString() + "/" + participantsPending.ToString() + "/" + participantsDeclined.ToString();

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
        currentItemIndex = itemIndex;
        if (Shifts[itemIndex].Glyph == itemGlyphPlus)
            Shifts[itemIndex].Glyph = itemGlyphMinus;
        else
            Shifts[itemIndex].Glyph = itemGlyphPlus;

        var shiftRolesTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Role>>(async () => await Db.GetRoleByShiftId(Shifts[itemIndex].Id));
        var shiftRoles = shiftRolesTask.Result;
        if (shiftRoles != null)
            Participate(shiftRoles.FirstOrDefault().Id, App.Instance.User.Id, "ACCEPT");
    }

    private async void Participate(string roleId, string personId, string acceptance)
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);

            var jwtRequest = new GraphQLRequest
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

            dynamic jwtResponse = null;
            try
            {
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
                if (QueryHasErrors(jwtResponse))
                    await Application.Current.MainPage.DisplayAlert("Error while applying for shift", Result, "OK");
                else
                    Shifts[currentItemIndex].Glyph = itemGlyphCheck;
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
