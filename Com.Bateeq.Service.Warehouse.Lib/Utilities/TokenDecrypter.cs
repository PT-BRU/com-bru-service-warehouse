using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Utilities
{
    public class TokenDecrypter
    {
        public static List<string> GetStore(string token)
        {
            var res = new List<string>();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            foreach (var claim in jwtSecurityToken.Claims)
            {
                if (claim.Type == "stores")
                {
                    StoreViewModel stores = JsonConvert.DeserializeObject<StoreViewModel>(claim.Value);
                    res.Add(stores.Code);
                }
                
            }

            return res;
        }
    }
}
