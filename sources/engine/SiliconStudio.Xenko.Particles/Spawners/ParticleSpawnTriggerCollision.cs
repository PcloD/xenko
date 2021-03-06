// Copyright (c) 2014-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using SiliconStudio.Core;
using SiliconStudio.Xenko.Particles.Updaters;

namespace SiliconStudio.Xenko.Particles.Spawners
{
    /// <summary>
    /// <see cref="ParticleSpawnTriggerCollision"/> triggers when the parent particle collides with a surface
    /// </summary>
    [DataContract("ParticleSpawnTriggerCollision")]
    [Display("On Hit")]
    public class ParticleSpawnTriggerCollision : ParticleSpawnTrigger<ParticleCollisionAttribute>
    {
        public override void PrepareFromPool(ParticlePool pool)
        {
            if (pool == null)
            {
                FieldAccessor = ParticleFieldAccessor<ParticleCollisionAttribute>.Invalid();
                return;
            }

            FieldAccessor = pool.GetField(ParticleFields.CollisionControl);
        }

        public unsafe override float HasTriggered(Particle parentParticle)
        {
            if (!FieldAccessor.IsValid())
                return 0f;

            var collisionAttribute = (*((ParticleCollisionAttribute*)parentParticle[FieldAccessor]));
            return (collisionAttribute.HasColided) ? 1f : 0f;
        }
    }
}
