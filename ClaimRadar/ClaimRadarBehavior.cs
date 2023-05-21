using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace ClaimRadar
{
    public class ClaimRadarBehavior : EntityBehavior
    {
        private ICoreClientAPI capi;
        private long updateClaimRadarTimer;

        public ClaimRadarBehavior(Entity entity) : base(entity)
        {
            Initialize();
        }

        public void Initialize()
        {
            if (entity.Api.Side == EnumAppSide.Client)
            {
                capi = (ICoreClientAPI)entity.Api;
            }

            updateClaimRadarTimer = entity.World.RegisterGameTickListener(new Action<float>(UpdateClaimRadar), 1000, 0);

        }

        public void UpdateClaimRadar(float dt)
        {

            if (entity.HasBehavior("ClaimRadarBehavior"))
            {


            }

        }

        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            base.OnEntityDespawn(despawn);
            entity.World.UnregisterGameTickListener(updateClaimRadarTimer);
        }

        public override string PropertyName()
        {
            return "ClaimRadarBehavior";
        }
    }
}
