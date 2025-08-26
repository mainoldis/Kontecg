using JetBrains.Annotations;

namespace Kontecg.Configuration
{
    /// <summary>
    /// Defines the contract for encrypting and decrypting sensitive setting values in the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ISettingEncryptionService provides a security layer for protecting sensitive configuration
    /// settings such as passwords, API keys, connection strings, and other confidential information.
    /// It ensures that sensitive data is encrypted when stored and decrypted when retrieved.
    /// </para>
    /// <para>
    /// <strong>Security Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Transparent Encryption:</strong> Automatic encryption/decryption without manual intervention</description></item>
    /// <item><description><strong>Setting-Specific:</strong> Encryption can be configured per setting definition</description></item>
    /// <item><description><strong>Algorithm Flexibility:</strong> Support for different encryption algorithms and key management</description></item>
    /// <item><description><strong>Performance Optimized:</strong> Efficient encryption/decryption for high-frequency operations</description></item>
    /// <item><description><strong>Key Management:</strong> Secure handling of encryption keys and certificates</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Encryption Process:</strong>
    /// <list type="number">
    /// <item><description>Setting value is checked against setting definition for encryption requirement</description></item>
    /// <item><description>If encryption is required, plain value is encrypted before storage</description></item>
    /// <item><description>Encrypted value is stored in the setting store</description></item>
    /// <item><description>When retrieved, encrypted value is automatically decrypted</description></item>
    /// <item><description>Plain value is returned to the application</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Use Cases:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Database Passwords:</strong> Encrypt database connection passwords</description></item>
    /// <item><description><strong>API Keys:</strong> Protect external service API keys and tokens</description></item>
    /// <item><description><strong>Connection Strings:</strong> Secure database and service connection strings</description></item>
    /// <item><description><strong>License Keys:</strong> Protect software license and activation keys</description></item>
    /// <item><description><strong>User Credentials:</strong> Encrypt service account credentials</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Implementation Considerations:</strong>
    /// <list type="bullet">
    /// <item><description>Use strong encryption algorithms (AES, RSA, etc.)</description></item>
    /// <item><description>Implement proper key management and rotation</description></item>
    /// <item><description>Consider performance impact of encryption operations</description></item>
    /// <item><description>Provide fallback mechanisms for key recovery</description></item>
    /// <item><description>Log encryption operations for audit purposes</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> Implementations should be thread-safe and support
    /// concurrent encryption/decryption operations from multiple threads.
    /// </para>
    /// </remarks>
    public interface ISettingEncryptionService
    {
        /// <summary>
        /// Encrypts a plain setting value based on the setting definition's encryption requirements.
        /// </summary>
        /// <param name="settingDefinition">
        /// The setting definition that contains encryption configuration and metadata.
        /// </param>
        /// <param name="plainValue">
        /// The plain text value to be encrypted. Can be null if the setting has no value.
        /// </param>
        /// <returns>
        /// The encrypted value if encryption is required and the value is not null, 
        /// or the original plain value if encryption is not required, or null if the input value is null.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method encrypts a plain setting value according to the encryption configuration
        /// specified in the setting definition. The encryption process is transparent to the
        /// calling code and handles all the complexity of key management and algorithm selection.
        /// </para>
        /// <para>
        /// <strong>Encryption Logic:</strong>
        /// <list type="bullet">
        /// <item><description>Check if the setting definition requires encryption</description></item>
        /// <item><description>If encryption is not required, return the plain value unchanged</description></item>
        /// <item><description>If the plain value is null, return null</description></item>
        /// <item><description>Apply the appropriate encryption algorithm and key</description></item>
        /// <item><description>Return the encrypted value in a format suitable for storage</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Security Considerations:</strong>
        /// <list type="bullet">
        /// <item><description>Use cryptographically secure random number generators</description></item>
        /// <item><description>Implement proper key derivation and management</description></item>
        /// <item><description>Consider using authenticated encryption (AEAD) for integrity</description></item>
        /// <item><description>Protect encryption keys from unauthorized access</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> Encryption operations can be computationally expensive.
        /// Consider caching encrypted values or using efficient algorithms for high-frequency settings.
        /// </para>
        /// </remarks>
        [CanBeNull]
        string Encrypt([NotNull] SettingDefinition settingDefinition, [CanBeNull] string plainValue);

        /// <summary>
        /// Decrypts an encrypted setting value based on the setting definition's encryption requirements.
        /// </summary>
        /// <param name="settingDefinition">
        /// The setting definition that contains encryption configuration and metadata.
        /// </param>
        /// <param name="encryptedValue">
        /// The encrypted value to be decrypted. Can be null if the setting has no value.
        /// </param>
        /// <returns>
        /// The decrypted plain value if the value was encrypted, or the original value if it was not encrypted,
        /// or null if the input value is null.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method decrypts an encrypted setting value according to the encryption configuration
        /// specified in the setting definition. The decryption process is transparent to the calling
        /// code and handles all the complexity of key management and algorithm selection.
        /// </para>
        /// <para>
        /// <strong>Decryption Logic:</strong>
        /// <list type="bullet">
        /// <item><description>Check if the setting definition requires encryption</description></item>
        /// <item><description>If encryption is not required, return the value unchanged</description></item>
        /// <item><description>If the encrypted value is null, return null</description></item>
        /// <item><description>Apply the appropriate decryption algorithm and key</description></item>
        /// <item><description>Return the decrypted plain value</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Error Handling:</strong>
        /// <list type="bullet">
        /// <item><description>Handle corrupted or invalid encrypted data gracefully</description></item>
        /// <item><description>Provide meaningful error messages for decryption failures</description></item>
        /// <item><description>Consider fallback mechanisms for key rotation scenarios</description></item>
        /// <item><description>Log decryption errors for security monitoring</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Security Considerations:</strong>
        /// <list type="bullet">
        /// <item><description>Verify the integrity of encrypted data during decryption</description></item>
        /// <item><description>Use secure key storage and retrieval mechanisms</description></item>
        /// <item><description>Implement proper access controls for decryption operations</description></item>
        /// <item><description>Consider using hardware security modules (HSM) for key storage</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> Decryption operations can be computationally expensive.
        /// Consider caching decrypted values or using efficient algorithms for high-frequency settings.
        /// </para>
        /// </remarks>
        [CanBeNull]
        string Decrypt([NotNull] SettingDefinition settingDefinition, [CanBeNull] string encryptedValue);
    }
}
