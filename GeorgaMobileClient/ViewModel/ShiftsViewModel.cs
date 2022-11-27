using CommunityToolkit.Mvvm.ComponentModel;
using GeorgaMobileDatabase.Model;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using static SQLite.SQLite3;

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
                foreach (var role in shiftRoles)
                {
                    helpersNeeded += role.Quantity;

                    // compute helpers already acquired -- scrap that, done in role now on server
                    // var shiftParticipantsTask = System.Threading.Tasks.Task.Run<List<GeorgaMobileDatabase.Model.Participant>>(async () => await Db.GetParticipantByRoleId(shift.Id));
                    // var shiftParticipants = shiftParticipantsTask.Result;
                    // foreach (var participant in shiftParticipants)
                    // {
                    //     participantsAcquired ++;
                    // }
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

    public async System.Threading.Tasks.Task SelectItem(int itemIndex)
    {
        if (Shifts[itemIndex].Glyph == itemGlyphPlus)
            Shifts[itemIndex].Glyph = itemGlyphMinus;
        else
            Shifts[itemIndex].Glyph = itemGlyphPlus;
    }

    private async void SendToApi()
    {
/*        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var graphQLClient = new GraphQLHttpClient(DeviceInfo.Platform == DevicePlatform.Android ? settings.AndroidEndpoint : settings.OtherPlatformsEndpoint, new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", App.Instance.User.Token);

            var jwtRequest = new GraphQLRequest
            {
                Query = """
					mutation CreateParticipant (
						$person: String!
						$role: String!
					)
					{
						personAuth: tokenAuth (
						input: {
							email: $email
							password: $password
						})
						{
							id
							payload
							token
							refreshExpiresIn
						}
					}
					""",
                Variables = new
                {
                    email = Email,
                    password = Password
                }
            };

            string token = "", id = "";
            dynamic jwtResponse = null;
            try
            {
                jwtResponse = await graphQLClient.SendQueryAsync<dynamic>(jwtRequest);
                if (QueryHasErrors(jwtResponse))
                    return;

                // login, if token has been aquired successfully
                token = jwtResponse.Data.personAuth.token;

                _ = await Db.Login(ComputeSha256Hash(Email.ToLower()), Password);
                App.Instance.User.Email = Email;
                App.Instance.User.Password = Password;
                App.Instance.User.Id = id;
                App.Instance.User.Token = token;
                App.Instance.User.Authenticated = true;

                D.SetAuthToken();
                Result = await D.CacheAll();
                if (Result != "")
                    await Application.Current.MainPage.DisplayAlert("Data Service Error", Result, "OK");
#if DEBUG
                else
                    // automatically go to page of interest for debugging purposes
                    await Shell.Current.GoToAsync("//projects");
                // await Shell.Current.GoToAsync("//profile");					
#endif
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
                        Result = $"Devices database reports '{e.Message}' -- make sure you entered your password correctly.";
                    else
                        Result = e.Message;
                }
                return;
            }
        }
*/
    }
}
