using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Math.EC.Rfc8032;
using System;
using System.Text.Json;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Nop.Services.Logging;

namespace Delsami.Plugin.Misc.TorobApi.Filters
{
    public class TorobAuthenticationFilter : IActionFilter
    {
        private readonly TorobApiSettings _settings;
        private readonly ILogger _logger;


        public TorobAuthenticationFilter(TorobApiSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }



        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Headers.TryGetValue("X-Torob-Token", out var token))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false
                });
                var type = typeof(Ed25519);

                return;
            }

            if (!request.Headers.TryGetValue("X-Torob-Token-Version", out var version) || version != "1")
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false
                });

                return;
            }

            if (!JwtValidator(token, request.Host.Value))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false
                });

                return;
            }

        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
        }




        public bool JwtValidator(string token, string host)
        {
            try
            {

                var parts = token.Split('.');
                if (parts.Length != 3)
                    return false;

                var header = JsonDocument.Parse(Encoding.UTF8.GetString(Base64UrlDecode(parts[0])));

                var payload = JsonDocument.Parse(Encoding.UTF8.GetString(Base64UrlDecode(parts[1])));

                // alg
                if (!header.RootElement.TryGetProperty("alg", out var alg) || alg.GetString() != "EdDSA")
                {
                    _logger.Information($"alg:{alg.GetString()}");
                    return false;
                }
                // aud
                if (!payload.RootElement.TryGetProperty("aud", out var aud) || aud.GetString() != host.Replace("www.", ""))
                {
                    _logger.Information($"host:{host}");
                    return false;
                }
                // exp
                if (!payload.RootElement.TryGetProperty("exp", out var exp) ||
                    !exp.TryGetInt64(out var expValue) ||
                    DateTimeOffset.UtcNow >= DateTimeOffset.FromUnixTimeSeconds(expValue))
                {
                    _logger.Information($"exp:{DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64())}");
                    return false;
                }
                // nbf
                if (payload.RootElement.TryGetProperty("nbf", out var nbf) &&
                    nbf.TryGetInt64(out var nbfValue) &&
                    DateTimeOffset.UtcNow < DateTimeOffset.FromUnixTimeSeconds(nbfValue))
                { 
                    _logger.Information($"nbf:{DateTimeOffset.FromUnixTimeSeconds(nbf.GetInt64())}");

                    return false;
                }
                // Signature
                var signature = Base64UrlDecode(parts[2]);

                var signingInput = Encoding.ASCII.GetBytes($"{parts[0]}.{parts[1]}");

                string torobPublicKey = _settings.TorobPublicKey;

                var key = PublicKeyFactory.CreateKey(Convert.FromBase64String(torobPublicKey));

                if (key is not Ed25519PublicKeyParameters publicKey)
                    return false;

                return Ed25519.Verify(
                    signature,
                    0,
                    publicKey.GetEncoded(),
                    0,
                    signingInput,
                    0,
                    signingInput.Length);
            }
            catch
            {
                return false;
            }
        }

        private static byte[] Base64UrlDecode(string value)
        {
            value = value
                .Replace('-', '+')
                .Replace('_', '/');

            switch (value.Length % 4)
            {
                case 2: value += "=="; break;
                case 3: value += "="; break;
            }

            return Convert.FromBase64String(value);
        }
    }

}
