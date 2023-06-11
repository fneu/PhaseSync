using PhaseSync.Core.Entity.Settings;
using PhaseSync.Core.Entity.Settings.Input;
using Xive.Hive;

namespace Test.PhaseSync.Core.Entity.Settings
{
    public sealed class SettingsTests
    {
        private const string TESTUSERID = "1234";

        [Fact]
        public void StoresPolarEmail()
        {
            var email = "test@test.test";
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new PolarEmail(email));

            Assert.True(new PolarEmail.Has(settings).Value());
            Assert.Equal(email, new PolarEmail.Of(settings).Value());
        }

        [Fact]
        public void StoresPolarPassword()
        {
            var pw = "asdf";
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new PolarPassword(pw, TESTUSERID));

            Assert.True(new PolarPassword.Has(settings).Value());
            Assert.Equal(pw, new PolarPassword.Of(settings, TESTUSERID).Value());
        }

        [Fact]
        public void EncryptsPolarPasswordWithRandomIV()
        {
            var pw = "asdf";
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new PolarPassword(pw, "passphrase"));
            var firstPW = settings.Memory().Value("polar.password.encrypted");
            var firstIV = settings.Memory().Value("polar.password.IV");
            settings.Update(new PolarPassword(pw, "passphrase"));
            var secondPW = settings.Memory().Value("polar.password.encrypted");
            var secondIV = settings.Memory().Value("polar.password.IV");

            Assert.True(firstIV != secondIV);
            Assert.True(firstPW != secondPW);
            Assert.True(firstPW != pw);
            Assert.True(secondPW != pw);
            Assert.Equal(pw, new PolarPassword.Of(settings, "passphrase").Value());
        }

        [Fact]
        public void StoresSetZones()
        {
            var setZones = true;
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new SetZones(setZones));

            Assert.True(new SetZones.Has(settings).Value());
            Assert.Equal(setZones, new SetZones.Of(settings).Value());
        }

        [Fact]
        public void StoresTaoToken()
        {
            var token = "qwerasdf";
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new TaoToken(token));

            Assert.True(new TaoToken.Has(settings).Value());
            Assert.Equal(token, new TaoToken.Of(settings).Value());
        }

        [Fact]
        public void DeletesTaoToken()
        {
            var token = "";
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new TaoToken(token));

            Assert.False(new TaoToken.Has(settings).Value());
        }

        [Fact]
        public void StoresZoneMas()
        {
            var mas = 3.4;
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new ZoneMas(mas));

            Assert.True(new ZoneMas.Has(settings).Value());
            Assert.Equal(mas, new ZoneMas.Of(settings).Value());
        }

        [Theory]
        [InlineData("PACE", "PACE")]
        [InlineData("SPEED", "SPEED")]
        [InlineData("INVALID", "PACE")]
        public void StoresZoneMode(string userMode, string setMode)
        {
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new ZoneMode(userMode));

            Assert.True(new ZoneMode.Has(settings).Value());
            Assert.Equal(setMode, new ZoneMode.Of(settings).Value());
        }
        
        [Fact]
        public void StoresZoneRadius()
        {
            var radius = 0.18;
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new ZoneRadius(radius));

            Assert.True(new ZoneRadius.Has(settings).Value());
            Assert.Equal(radius, new ZoneRadius.Of(settings).Value());
        }

        [Theory]
        [InlineData("METRIC", "METRIC")]
        [InlineData("IMPERIAL", "IMPERIAL")]
        [InlineData("INVALID", "METRIC")]
        public void StoresZoneUnit(string userUnit, string setUnit)
        {
            var settings = new SettingsOf(new RamHive(TESTUSERID));
            settings.Update(new ZoneUnit(userUnit));

            Assert.True(new ZoneUnit.Has(settings).Value());
            Assert.Equal(setUnit, new ZoneUnit.Of(settings).Value());
        }
    }
}
