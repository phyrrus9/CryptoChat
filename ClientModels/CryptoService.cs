using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public class CryptoService
{
    public enum KeyType
    {
        Public,
        Private,
        Both
    }
    protected static byte[] Process(byte[] data, AsymmetricKeyParameter key, bool encrypt)
    {
        OaepEncoding eng = new(new RsaEngine());
        eng.Init(encrypt, key);
        int blockSize = eng.GetInputBlockSize();
        List<byte> bytes = new();
        for (int chunkPosition = 0; chunkPosition < data.Length; chunkPosition += blockSize)
        {
            int chunkSize = Math.Min(blockSize, data.Length - chunkPosition);
            bytes.AddRange(eng.ProcessBlock(data, chunkPosition, chunkSize));
        }
        return bytes.ToArray();
    }
    public static byte[] Encrypt(byte[] data, AsymmetricKeyParameter key) => Process(data, key, encrypt: true);
    public static byte[] Decrypt(byte[] data, AsymmetricKeyParameter key) => Process(data, key, encrypt: false);
    public static AsymmetricCipherKeyPair GenerateKeypair(int RsaKeySize)
    {
        using RSACryptoServiceProvider rsaProvider = new(RsaKeySize);
        RSAParameters rsaKeyInfo = rsaProvider.ExportParameters(true);
        return DotNetUtilities.GetRsaKeyPair(rsaKeyInfo);
    }

    public static bool SignatureValidate(byte[] data, byte[] signature, AsymmetricKeyParameter key)
    {
        ISigner signClientSide = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id);
        signClientSide.Init(false, key);
        signClientSide.BlockUpdate(data, 0, data.Length);
        return signClientSide.VerifySignature(signature);
    }
    public static byte[] SignatureGenerate(byte[] data, AsymmetricKeyParameter key)
    {
        ISigner sign = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id);
        sign.Init(true, key);
        sign.BlockUpdate(data, 0, data.Length);
        return sign.GenerateSignature();
    }

    public static string ExportKeypair(AsymmetricCipherKeyPair keypair, KeyType keytype)
    {
        using TextWriter tw = new StringWriter();
        PemWriter pw = new(tw);
        pw.WriteObject(keytype switch
        {
            KeyType.Public => keypair.Public,
            KeyType.Private => keypair.Private,
            KeyType.Both => keypair,
            _ => throw new ArgumentException(nameof(keytype))
        });
        return tw.ToString();
    }
    public static AsymmetricKeyParameter ImportKey(string pem)
    {
        using TextReader tr = new StringReader(pem);
        PemReader pr = new(tr);
        object key = pr.ReadObject();
        if (key is RsaKeyParameters pubkey)
            return pubkey;
        else if (key is AsymmetricCipherKeyPair prikey)
            return prikey.Private;
        throw new DataException();
    }
    public static string Fingerprint(AsymmetricCipherKeyPair keyPair) => Fingerprint((RsaKeyParameters)keyPair.Public);
    public static string Fingerprint(RsaKeyParameters key)
    {
        byte[] modbytes = key.Modulus.ToByteArrayUnsigned();
        using SHA256 sha = SHA256.Create();
        return string.Join(string.Empty, sha.ComputeHash(modbytes).Select(x => x.ToString("x2")));
    }
}
