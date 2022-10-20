// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using LegoCity.Api.Client.Hubs.V1;
using Microsoft.AspNetCore.SignalR;

namespace LegoCity.Api.Hubs.V1
{
    
    /// <summary>
    /// 
    /// </summary>
    public class LegoCityHubV1 : Hub<ILegoCityClientV1>, ILegoCityHubV1
    {
    }
}
