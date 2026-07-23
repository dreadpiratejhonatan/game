using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Combate: perseguição, ataque com cooldown, IA hostil e morte de unidades.
/// </summary>
public class CombatSystem : ISystem
{
    private readonly World _world;

    public CombatSystem(World world)
    {
        _world = world;
    }

    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        var list = entities.ToList();
        var toDestroy = new List<Entity>();

        foreach (var entity in list)
        {
            var state = entity.GetComponent<CombatStateComponent>();
            if (state == null) continue;

            if (state.AttackCooldownRemaining > 0f)
                state.AttackCooldownRemaining -= deltaTime;
            if (state.HitFlashTimer > 0f)
                state.HitFlashTimer -= deltaTime;
        }

        // IA: NPCs sem alvo (inimigos e aliados) adquirem hostil próximo
        foreach (var entity in list)
        {
            if (entity.HasComponent<PlayerControlledComponent>()) continue;

            var combat = entity.GetComponent<CombatTargetComponent>();
            var stats = entity.GetComponent<UnitStatsComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var team = entity.GetComponent<TeamComponent>();

            if (team == null || combat == null || stats == null || transform == null) continue;
            if (combat.Target is { IsActive: true }) continue;

            combat.Target = FindNearestHostile(entity, list, stats.DetectionRange);
        }

        // Combate ativo
        foreach (var entity in list)
        {
            var combat = entity.GetComponent<CombatTargetComponent>();
            var stats = entity.GetComponent<UnitStatsComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var moveTarget = entity.GetComponent<MoveTargetComponent>();
            var state = entity.GetComponent<CombatStateComponent>();
            var render = entity.GetComponent<RenderComponent>();

            if (combat?.Target == null || stats == null || transform == null || state == null)
                continue;

            var target = combat.Target;
            if (!target.IsActive)
            {
                combat.Target = null;
                continue;
            }

            var targetTransform = target.GetComponent<TransformComponent>();
            var targetRender = target.GetComponent<RenderComponent>();
            var targetStats = target.GetComponent<UnitStatsComponent>();
            var targetTeam = target.GetComponent<TeamComponent>();
            var myTeam = entity.GetComponent<TeamComponent>();

            if (targetTransform == null || targetStats == null || myTeam == null || targetTeam == null
                || !TeamComponent.AreHostile(myTeam.Faction, targetTeam.Faction))
            {
                combat.Target = null;
                continue;
            }

            var myCenter = GetCenter(transform, render);
            var targetCenter = GetCenter(targetTransform, targetRender);
            var distance = Vector2.Distance(myCenter, targetCenter);

            // Virar sprite na direção do alvo
            var sprite = entity.GetComponent<SpriteComponent>();
            if (sprite != null && Math.Abs(targetCenter.X - myCenter.X) > 1f)
                sprite.FacingLeft = targetCenter.X < myCenter.X;

            if (distance > stats.AttackRange)
            {
                // Persegue até o alcance
                if (moveTarget != null && targetRender != null)
                    moveTarget.Target = targetTransform.Position;
            }
            else
            {
                // Dentro do alcance: para e ataca
                if (moveTarget != null)
                    moveTarget.Target = null;

                if (state.AttackCooldownRemaining <= 0f)
                {
                    targetStats.CurrentHealth -= stats.AttackDamage;
                    state.AttackCooldownRemaining = stats.AttackCooldown;

                    var targetState = target.GetComponent<CombatStateComponent>();
                    if (targetState != null)
                        targetState.HitFlashTimer = 0.15f;

                    // Inimigo revida se ainda não tem alvo
                    var targetCombat = target.GetComponent<CombatTargetComponent>();
                    if (targetCombat != null && (targetCombat.Target == null || !targetCombat.Target.IsActive))
                        targetCombat.Target = entity;

                    Console.WriteLine($"[COMBAT] {stats.UnitType} hit {targetStats.UnitType} " +
                                      $"for {stats.AttackDamage:0} ({targetStats.CurrentHealth:0}/{targetStats.MaxHealth:0})");

                    if (targetStats.CurrentHealth <= 0f)
                    {
                        Console.WriteLine($"[COMBAT] {targetStats.UnitType} destroyed.");
                        toDestroy.Add(target);
                        combat.Target = null;
                    }
                }
            }
        }

        foreach (var dead in toDestroy.Distinct())
        {
            // Limpa referências de quem mirava o morto
            foreach (var entity in list)
            {
                var combat = entity.GetComponent<CombatTargetComponent>();
                if (combat?.Target == dead)
                    combat.Target = null;
            }

            _world.DestroyEntity(dead);
        }
    }

    private static Entity? FindNearestHostile(Entity self, List<Entity> entities, float range)
    {
        var selfTeam = self.GetComponent<TeamComponent>();
        var selfTransform = self.GetComponent<TransformComponent>();
        var selfRender = self.GetComponent<RenderComponent>();
        if (selfTeam == null || selfTransform == null) return null;

        var selfCenter = GetCenter(selfTransform, selfRender);
        Entity? best = null;
        var bestDist = range;

        foreach (var other in entities)
        {
            if (other == self || !other.IsActive) continue;

            var otherTeam = other.GetComponent<TeamComponent>();
            if (otherTeam == null || !TeamComponent.AreHostile(selfTeam.Faction, otherTeam.Faction))
                continue;

            var otherTransform = other.GetComponent<TransformComponent>();
            if (otherTransform == null) continue;

            var dist = Vector2.Distance(selfCenter, GetCenter(otherTransform, other.GetComponent<RenderComponent>()));
            if (dist < bestDist)
            {
                bestDist = dist;
                best = other;
            }
        }

        return best;
    }

    private static Vector2 GetCenter(TransformComponent transform, RenderComponent? render)
    {
        if (render == null) return transform.Position;
        return transform.Position + render.Size / 2f;
    }
}
