namespace Forge.Security.Jwt.Service
{

    /// <summary>Configuration for token provider</summary>
    public class JwtTokenConfiguration
    {

        /// <summary>Gets or sets the unique secret information to generate access tokens.</summary>
        /// <value>The secret value. A GUID is a goog choice.</value>
        public string Secret { get; set; }

        /// <summary>Gets or sets the issuer.</summary>
        /// <value>The issuer, for example: <a href="https://jzo.hu">https://jzo.hu</a></value>
        public string Issuer { get; set; }

        /// <summary>Gets or sets the audience.</summary>
        /// <value>The audience, for example: <a href="https://jzo.hu">https://jzo.hu</a></value>
        public string Audience { get; set; }

        /// <summary>Gets or sets the access token expiration in minutes.</summary>
        /// <value>The access token expiration in minutes.
        /// Default is 10 minutes.</value>
        public int AccessTokenExpirationInMinutes { get; set; } = 10;

        /// <summary>Gets or sets the refresh token expiration in minutes.</summary>
        /// <value>The refresh token expiration in minutes.
        /// Default is 10 minutes.</value>
        public int RefreshTokenExpirationInMinutes { get; set; } = 10;

        /// <summary>Gets or sets a value indicating whether issuer will be validated.
        /// Do not turn it off, if False, it decreases the secuity level.</summary>
        /// <value>
        ///   <c>true</c> if [validate issuer]; otherwise, <c>false</c>.</value>
        public bool ValidateIssuer { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether [validate issuer signing key].
        /// Do not turn it off, if False, it decreases the secuity level.</summary>
        /// <value>
        ///   <c>true</c> if [validate issuer signing key]; otherwise, <c>false</c>.</value>
        public bool ValidateIssuerSigningKey { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether [validate audience].
        /// Do not turn it off, if False, it decreases the secuity level.</summary>
        /// <value>
        ///   <c>true</c> if [validate audience]; otherwise, <c>false</c>.</value>
        public bool ValidateAudience { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether [validate lifetime].
        /// Do not turn it off, if False, it decreases the secuity level.</summary>
        /// <value>
        ///   <c>true</c> if [validate lifetime]; otherwise, <c>false</c>.</value>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>Gets or sets the clock skew in minutes.
        /// Do not turn it off, if False, it decreases the secuity level.</summary>
        /// <value>The clock skew in minutes.</value>
        public int ClockSkewInMinutes { get; set; } = 1;

    }

}
