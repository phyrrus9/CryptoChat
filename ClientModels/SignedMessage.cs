using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClientModels
{
    public class SignedMessage
    {
        public static Func<string /* in exchange fingerprint */, AsymmetricKeyParameter /* out key */> X_KeyLookup = null;
        public static Func<string /* in exchange fingerprint */, string /* out user id */> X_UserLookup = null;
        public class SignatureData
        {
            [Required] public string ExchangeFingerprint { get; set; }
            [Required] public DateTime Nonce { get; set; }
            [Required] public byte[] Signature { get; set; }
            public byte[] Data => System.Text.Encoding.UTF8.GetBytes($"{Nonce:O}|{ExchangeFingerprint}");
        }
        [Required] public SignatureData X_MessageSignature { get; set; }
        public void SignData(AsymmetricKeyParameter key)
        {
            X_MessageSignature = new()
            {
                Nonce = DateTime.UtcNow,
                ExchangeFingerprint = CryptoService.Fingerprint((RsaKeyParameters)key)
            };
            X_MessageSignature.Signature = CryptoService.SignatureGenerate(X_MessageSignature.Data, key);
        }
        public bool VerifyData() => VerifyData(X_KeyLookup?.Invoke(X_MessageSignature.ExchangeFingerprint));
        public bool VerifyData(AsymmetricKeyParameter key)
        {
            if (key == null || X_MessageSignature.Nonce > DateTime.UtcNow.AddMinutes(2.5) || X_MessageSignature.Nonce <= DateTime.UtcNow.AddMinutes(-2.5))
                return false;
            return CryptoService.SignatureValidate(X_MessageSignature.Data, X_MessageSignature.Signature, key);
        }
        public string GetUserId() => X_UserLookup?.Invoke(X_MessageSignature.ExchangeFingerprint);
    }
}
