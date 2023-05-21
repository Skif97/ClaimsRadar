using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.GameContent;
using Vintagestory.Server;


namespace ClaimRadar
{
    public class ClientMod : ModSystem
    {

        private ICoreClientAPI capi;
        private List<LandClaim> claims;

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            api.Input.RegisterHotKey(
              "showClaims",
              Lang.Get("show claims"),
              GlKeys.R,
              HotkeyType.GUIOrOtherControls);

            api.Input.SetHotKeyHandler("showClaims", UpdateClaimRadar);

            claims = new List<LandClaim>();

        }

        private bool UpdateClaimRadar(KeyCombination keyCombination) 
        {

            GenerateListShowClaim(capi.World.Player);
            AddClaimsToHighlight(capi.World.Player);
            return true;
        }

        private void GenerateListShowClaim(IClientPlayer byPlayer) 
        {
            claims.Clear();
            List<LandClaim> allClaims = capi.World.Claims.All.ToList();
            foreach (LandClaim claim in allClaims) 
            {
                if (claim.OwnedByPlayerUid == null) continue;
                if (byPlayer.Entity.Pos.DistanceTo(claim.Areas[0].Center.ToBlockPos().ToVec3d()) < 150.0) 
                {
                    claims.Add(claim);
                }
            }
        }
        private void AddClaimsToHighlight(IClientPlayer byPlayer) 
        {
            int i = 100;
            List<BlockPos> list = new List<BlockPos>();
            foreach(LandClaim claim in claims) 
            {
                foreach(Cuboidi area in claim.Areas) 
                {
                    list.Clear();
                    for (int j = 0; area.MinX+j< area.MaxX; j++) 
                    {
                        for (int k = 0; area.MinY + k < area.MaxY; k++)
                        {
                            for (int h = 0; area.MinZ + h < area.MaxZ; h++)
                            {
                                list.Add(new BlockPos(area.MinX + j, area.MinY + k, area.MinZ + h));
                            }
                        }
                    }
                    byPlayer.Entity.Api.World.HighlightBlocks(byPlayer, i, list);
                    i++;
                }
                 
            }
        }
    }

}
