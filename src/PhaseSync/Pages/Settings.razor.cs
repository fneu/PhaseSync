using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using MudBlazor;
using NuGet.Configuration;
using PhaseSync.Blazor.Data;
using PhaseSync.Blazor.Options;
using PhaseSync.Core.Entity;
using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using System.Net.Http.Headers;
using Xive;

namespace PhaseSync.Blazor.Pages
{

    public partial class Settings : ComponentBase
    {
        [Inject]
        public HiveService HiveService { get; set; } = default!;

        [Inject]
        public IOptions<PhaseSyncOptions> PhaseSyncOptions { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        public IEntity<IProps> UserSettings { get; set; } = default!;
    
        public bool TAOConnected { get; set; } = false;
        public string TAOUserID { get; set; } = "";

        public bool PolarConnected { get; set; } = false;
        public bool PolarFormValid { get; set; }
        public string PolarEmail { get; set; } = "";
        public string PolarPassword { get; set; } = "";

        [Inject] ISnackbar Snackbar { get; set; }
        MudForm form;
        MudForm form2;

        public bool HasUnit { get; set; } = false;
        public string Unit { get; set; } = "";
        public bool HasMode { get; set; } = false;
        public string Mode { get; set; } = "";
        public bool HasMas { get; set; } = false;
        public double Mas { get; set; } = 3.4;
        public bool SetZones { get; set; } = false;
        public bool HasRadius { get; set; } = false;
        public double Radius { get; set; } = 0.15;
        public bool SettingsFormValid { get; set; }


        public bool SettingsComplete { get; set; } = false;

        private bool _syncEnabled;
        public bool SyncEnabled {
            get { return _syncEnabled; }
            set { 
                _syncEnabled = value;
                this.UserSettings.Update(
                    new EnableSync(value));
            }
        }

        protected async override Task OnInitializedAsync()
        {
            this.UserSettings = new SettingsOf(await HiveService.UserHive());
            this.PolarConnected = new PolarEmail.Has(this.UserSettings).Value() && new PolarPassword.Has(this.UserSettings).Value();
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();
            this.HasUnit = new ZoneUnit.Has(this.UserSettings).Value();
            this.HasMode = new ZoneMode.Has(this.UserSettings).Value();
            this.HasMas = new ZoneMas.Has(this.UserSettings).Value();
            this.HasRadius = new ZoneRadius.Has(this.UserSettings).Value();
            if (this.HasUnit)
            {
                this.Unit = new ZoneUnit.Of(this.UserSettings).Value();
            }
            if (this.HasMode)
            {
                this.Mode = new ZoneMode.Of(this.UserSettings).Value();
            }
            if (new SetZones.Has(this.UserSettings).Value())
            {
                this.SetZones = new SetZones.Of(this.UserSettings).Value();
            }
            if (this.HasMas)
            {
                this.Mas = new ZoneMas.Of(this.UserSettings).Value();
            }
            if (this.HasRadius)
            {
                this.Radius = new ZoneRadius.Of(UserSettings).Value();
            }
            this.SyncEnabled = new EnableSync.Of(UserSettings).Value();

            this.CheckSettings();
        }

        public void AuthorizeTAO()
        {
            var randomState = Guid.NewGuid().ToString();
            this.UserSettings.Update(new TaoState(randomState));
            var parameters = new Dictionary<string, string>
            {
                {"response_type", "code" },
                {"client_id", "PhaseSync" },
                {"redirect_uri", "http://localhost/auth/trainasone" },
                {"scope", "WORKOUT" },
                {"state", randomState }
            };

            var url = new UriBuilder("https://beta.trainasone.com/oauth/authorise")
            {
                Query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))
            }.ToString();

            NavigationManager.NavigateTo(url);
        }

        public void RemoveTAO()
        {
            if (!new TaoToken.Has(this.UserSettings).Value())
            {
                this.TAOConnected = false;
                return;
            }

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://beta.trainasone.com");
            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/revoke");

            var parameters = new Dictionary<string, string>
            {
                {"token", new TaoToken.Of(UserSettings).Value()}
            };
            request.Content = new FormUrlEncodedContent(parameters);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", new TaoToken.Of(UserSettings).Value());

            var response =  httpClient.Send(request);

            if (response.IsSuccessStatusCode)
            {
                this.UserSettings.Update(new TaoToken(""));
            }
            this.TAOConnected = new TaoToken.Has(this.UserSettings).Value();

            this.CheckSettings();
        }

        private async Task SetPolarPassword()
        {
            await form.Validate();

            if (form.IsValid)
            {
                this.UserSettings.Update(
                    new PolarEmail(this.PolarEmail),
                    new PolarPassword(this.PolarPassword, PhaseSyncOptions.Value.PasswordEncryptionSecret)
                    );
                this.PolarConnected = true;
                Snackbar.Add("Polar credentials were updated!");
            }

            this.CheckSettings();
        }

        private async Task SetSettings()
        {
            await form.Validate();

            if (form2.IsValid)
            {
                this.UserSettings.Update(
                    new ZoneUnit(Unit),
                    new ZoneMode(Mode),
                    new SetZones(SetZones),
                    new ZoneMas(Mas),
                    new ZoneRadius(Radius)
                    );
                this.HasUnit = true;
                this.HasMode = true;
                this.HasMas = true;
                this.HasRadius = true;
                Snackbar.Add("Phased target settings were updated!");
            }

            this.CheckSettings();
        }

        private void CheckSettings()
        {
            if (this.TAOConnected && this.PolarConnected && this.HasUnit && this.HasMode && this.HasMas && this.HasRadius)
            {
                this.SettingsComplete = true;

                this.UserSettings.Update(
                    new SettingsComplete(true));
            }
            else
            {
                this.SettingsComplete = false;
                this.UserSettings.Update(
                    new SettingsComplete(false)
                );
                this.SyncEnabled = false;
            }
        }
    }
}
