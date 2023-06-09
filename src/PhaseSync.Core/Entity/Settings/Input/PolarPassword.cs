using System.Security.Cryptography;
using System.Text;
using Xive;
using Yaapii.Atoms.Scalar;

namespace PhaseSync.Core.Entity.Settings.Input
{
    public sealed class PolarPassword : EntityInputEnvelope<IProps>
    {
        private const string KEY_ENC = "polar.password.encrypted";
        private const string KEY_IV = "polar.password.IV";

        public PolarPassword(string value, string passphrase) : base(
            (props) =>
            {
                using Aes aes = Aes.Create();
                aes.Key = new AesKey(passphrase).Value();
                aes.GenerateIV();
                var IV = aes.IV;

                using var output = new MemoryStream();
                using var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(Encoding.Unicode.GetBytes(value));
                cryptoStream.FlushFinalBlock();

                props.Refined(KEY_ENC, BitConverter.ToString(output.ToArray()));
                props.Refined(KEY_IV, BitConverter.ToString(IV));
            }
        )
        { }

        public sealed class Has : ScalarEnvelope<bool>
        {
            public Has(IEntity<IProps> settings) : base(
               () => settings.Memory().Names().Contains(KEY_ENC)
            )
            { }
        }

        public sealed class Of : ScalarEnvelope<string>
        {
            public Of(IEntity<IProps> settings, string passphrase) : base(
                () => 
                {
                    using Aes aes = Aes.Create();
                    aes.Key = new AesKey(passphrase).Value();
                    aes.IV = new BytesFromString(settings.Memory().Value(KEY_IV)).Value();

                    using var input = new MemoryStream(new BytesFromString(settings.Memory().Value(KEY_ENC)).Value());
                    using var cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
                    using var output = new MemoryStream();
                    cryptoStream.CopyTo(output);
                    return Encoding.Unicode.GetString(output.ToArray());
                }
            )
            { }
        }

        private sealed class AesKey: ScalarEnvelope<byte[]>
        {
            public AesKey(string passphrase) : base(
                () =>
                Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.Unicode.GetBytes(passphrase),
                    Array.Empty<byte>(),
                    1000,
                    HashAlgorithmName.SHA384,
                    16
                )
            )
            { }
        }

        private sealed class BytesFromString: ScalarEnvelope<byte[]>
        {
            public BytesFromString(string byteString) : base(
                () =>
                {
                    var stringArray = byteString.Split('-');
                    var byteArray = new byte[stringArray.Length];
                    for(int i = 0; i < stringArray.Length; i++) byteArray[i] = Convert.ToByte(stringArray[i], 16);
                    return byteArray;
                }
            )
            { }
        }
    }
}
