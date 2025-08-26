using System.IO;
using Kontecg.Timing;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

namespace Kontecg.Net.Security
{
    internal class SimpleCertificateProvider : ICertificateProvider
    {
        public X509Certificate GetSelfSignedCertificate()
        {
            SecureRandom random = new();
            X509Name caName = new X509Name("CN=ECG, CN=Kontecg");
            X509Name eeName = new X509Name("CN=Kontecg");
            AsymmetricCipherKeyPair caKey = GenerateEcKeyPair(random, "secp256r1");
            AsymmetricCipherKeyPair eeKey = GenerateRsaKeyPair(random, 2048);

            Org.BouncyCastle.X509.X509Certificate caCert =
                GenerateCertificate(caName, caName, caKey.Private, caKey.Public);
            Org.BouncyCastle.X509.X509Certificate eeCert =
                GenerateCertificate(caName, eeName, caKey.Private, eeKey.Public);
            bool caOk = ValidateSelfSignedCert(caCert, caKey.Public);
            bool eeOk = ValidateSelfSignedCert(eeCert, caKey.Public);

            using (FileStream f = File.OpenWrite("ca.cer"))
            {
                byte[] buf = caCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }

            using (FileStream f = File.OpenWrite("ee.cer"))
            {
                byte[] buf = eeCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }

            return DotNetUtilities.ToX509Certificate(eeCert);
        }

        private AsymmetricCipherKeyPair GenerateRsaKeyPair(SecureRandom secureRandom, int length)
        {
            KeyGenerationParameters keygenParam = new KeyGenerationParameters(secureRandom, length);

            RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        }

        private AsymmetricCipherKeyPair GenerateEcKeyPair(SecureRandom secureRandom, string curveName)
        {
            X9ECParameters ecParam = SecNamedCurves.GetByName(curveName);
            ECDomainParameters ecDomain = new ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);
            ECKeyGenerationParameters keygenParam = new ECKeyGenerationParameters(ecDomain, secureRandom);

            ECKeyPairGenerator keyGenerator = new ECKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        }

        private Org.BouncyCastle.X509.X509Certificate GenerateCertificate(
            X509Name issuer, X509Name subject,
            AsymmetricKeyParameter issuerPrivate,
            AsymmetricKeyParameter subjectPublic)
        {
            ISignatureFactory signatureFactory;
            if (issuerPrivate is ECPrivateKeyParameters)
            {
                signatureFactory = new Asn1SignatureFactory(
                    X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                    issuerPrivate);
            }
            else
            {
                signatureFactory = new Asn1SignatureFactory(
                    PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
                    issuerPrivate);
            }

            X509V3CertificateGenerator certGenerator = new X509V3CertificateGenerator();
            certGenerator.SetIssuerDN(issuer);
            certGenerator.SetSubjectDN(subject);
            certGenerator.SetSerialNumber(BigInteger.ValueOf(1));
            certGenerator.SetNotAfter(Clock.Now.ToUniversalTime().AddYears(1));
            certGenerator.SetNotBefore(Clock.Now.ToUniversalTime());
            certGenerator.SetPublicKey(subjectPublic);
            return certGenerator.Generate(signatureFactory);
        }

        private bool ValidateSelfSignedCert(Org.BouncyCastle.X509.X509Certificate cert, ICipherParameters pubKey)
        {
            cert.CheckValidity(Clock.Now.ToUniversalTime());
            byte[] tbsCert = cert.GetTbsCertificate();
            byte[] sig = cert.GetSignature();

            ISigner signer = SignerUtilities.GetSigner(cert.SigAlgName);
            signer.Init(false, pubKey);
            signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
            return signer.VerifySignature(sig);
        }
    }
}
