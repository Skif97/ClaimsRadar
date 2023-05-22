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
        private bool show = false;
        private long timerForUpdateClaimRadar;
        private long countShowClaim = 0;

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            api.Input.RegisterHotKey(
              "showClaims",
              Lang.Get("Show claims"),
              GlKeys.R,
              HotkeyType.GUIOrOtherControls);

            api.Input.SetHotKeyHandler("showClaims", OnOffClaimRadar);

            claims = new List<LandClaim>();

        }

        private bool OnOffClaimRadar(KeyCombination keyCombination) 
        {
            if (!show) 
            {
                show = true;
                timerForUpdateClaimRadar = capi.World.RegisterGameTickListener(new Action<float>(UpdateClaimRadar), 1000, 0);
                GenerateListShowClaim();
                AddClaimsToHighlight();
            }
            else 
            {
                show = false;
                capi.World.UnregisterGameTickListener(timerForUpdateClaimRadar);
                ClearHighlightBlocks();


            }
            return true;
        }

        private void ClearHighlightBlocks() 
        {
            for (int i = 0; i < countShowClaim; i++)
            {
                capi.World.HighlightBlocks(capi.World.Player, i + 100, new List<BlockPos>(), EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Cube);
            }
        }

        private void UpdateClaimRadar(float dt) 
        {
            GenerateListShowClaim();
            AddClaimsToHighlight();
        }



        private void GenerateListShowClaim() 
        {
            claims.Clear();
            List<LandClaim> allClaims = capi.World.Claims.All.ToList();
            foreach (LandClaim claim in allClaims) 
            {
                foreach (Cuboidi area in claim.Areas)
                {
                    if (capi.World.Player.Entity.Pos.DistanceTo(area.Center.ToBlockPos().ToVec3d()) < 300.0 ||
                        capi.World.Player.Entity.Pos.DistanceTo(area.Start.ToBlockPos().ToVec3d()) < 300.0 ||
                        capi.World.Player.Entity.Pos.DistanceTo(area.End.ToBlockPos().ToVec3d()) < 300.0)
                    {
                        claims.Add(claim);
                    }
                } 
            }
        }
        private void AddClaimsToHighlight() 
        {
            int i = 0;
            List<BlockPos> list = new List<BlockPos>();
            foreach(LandClaim claim in claims) 
            {
                foreach(Cuboidi area in claim.Areas) 
                {
                    list.Clear();
                    list.Add(new BlockPos(area.MinX, area.MinY, area.MinZ));
                    list.Add(new BlockPos(area.MaxX, area.MaxY, area.MaxZ));
                    capi.World.HighlightBlocks(capi.World.Player, i + 100, list, EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Cube);
                    i++;
                }
                 
            }
            
            for(int j=i; j < countShowClaim; j++) 
            {
                capi.World.HighlightBlocks(capi.World.Player, j + 100, new List<BlockPos>(), EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Cube);
            }
            countShowClaim = i;

        }
    }

}
