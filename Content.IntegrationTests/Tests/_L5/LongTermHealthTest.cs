using Content.Server._L5.LongTermHealth;
using Content.Shared._L5.CCVar;
using Content.Shared._L5.LongTermHealth;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._L5;

[TestFixture]
[TestOf(typeof(LongTermHealthSystem))]
public sealed class LongTermHealthTest
{
    private const string AsphyxiationType = "Asphyxiation";
    private const string BluntType = "Blunt";
    private const string HeatType = "Heat";
    private const string CausticType = "Caustic";
    private const string PoisonType = "Poison";
    private const string CellularType = "Cellular";

    [Test]
    public async Task TestLongTermHealth()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;

        var entSys = server.ResolveDependency<IEntitySystemManager>();
        var entMan = server.ResolveDependency<IEntityManager>();
        var proto = server.ResolveDependency<IPrototypeManager>();

        var config = server.ResolveDependency<IConfigurationManager>();
        config.SetCVar(L5CCVars.LongTermHealthEnabled, true);
        config.SetCVar(L5CCVars.ChanceToBecomeSevere, 0f); // Don't want random failures.
        var duration = TimeSpan.FromSeconds(config.GetCVar(L5CCVars.LongTermEffectsDuration));

        var map = await pair.CreateTestMap();

        EntityUid target = default;
        LongTermHealthComponent lthComp = default;
        DamageableComponent damage = default;
        DamageableSystem damageSys = default;
        DamageTypePrototype Asphyxiation = default;
        DamageTypePrototype Blunt = default;
        DamageTypePrototype Heat = default;
        DamageTypePrototype Caustic = default;
        DamageTypePrototype Poison = default;
        DamageTypePrototype Cellular = default;

        await server.WaitPost(() =>
        {
            target = entMan.Spawn("MobHuman", map.MapCoords);
            lthComp = entMan.GetComponent<LongTermHealthComponent>(target);
            damage = entMan.GetComponent<DamageableComponent>(target);
            damageSys = entSys.GetEntitySystem<DamageableSystem>();

            Asphyxiation = proto.Index<DamageTypePrototype>(AsphyxiationType);
            Blunt = proto.Index<DamageTypePrototype>(BluntType);
            Heat = proto.Index<DamageTypePrototype>(HeatType);
            Caustic = proto.Index<DamageTypePrototype>(CausticType);
            Poison = proto.Index<DamageTypePrototype>(PoisonType);
            Cellular = proto.Index<DamageTypePrototype>(CellularType);
        });

        await server.WaitRunTicks(5);

        await server.WaitAssertion(() =>
        {
            // Test return damage
            // TODO test that return damage is applied after update
            // -- Asphyx
            var asphyxThreshold = config.GetCVar(L5CCVars.AsphyxLungDamageMildThreshold);
            var toDeal = FixedPoint2.New(asphyxThreshold + 1);
            damageSys.ChangeDamage(target, new DamageSpecifier(Asphyxiation, toDeal), true);
            Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.MildLungDamage), Is.True);

            damageSys.ChangeDamage(target, new DamageSpecifier(Asphyxiation, -toDeal), true);
            Assert.Multiple(() =>
            {
                Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.MildLungDamage), Is.False);
                Assert.That(lthComp.CurrentEffects.TryGetValue(EffectType.MildLungDamage, out var asphyxDuration), Is.True);
                Assert.That(asphyxDuration, Is.EqualTo(duration));
            });

            // -- Poison
            var poisonThreshold = config.GetCVar(L5CCVars.PoisonReturnThreshold);
            toDeal = FixedPoint2.New(poisonThreshold + 1);
            damageSys.ChangeDamage(target, new DamageSpecifier(Poison, toDeal), true);
            Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.PoisonReturn), Is.True);

            damageSys.ChangeDamage(target, new DamageSpecifier(Poison, -toDeal), true);
            Assert.Multiple(() =>
            {
                Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.PoisonReturn), Is.False);
                Assert.That(lthComp.CurrentEffects.TryGetValue(EffectType.PoisonReturn, out var poisonReturn), Is.True);
                Assert.That(poisonReturn, Is.EqualTo(duration));
            });

            // -- Burn
            var burnThreshold = config.GetCVar(L5CCVars.BurnReturnThreshold);
            toDeal = FixedPoint2.New(burnThreshold + 1);
            damageSys.ChangeDamage(target, new DamageSpecifier(Heat, toDeal), true);
            Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.BurnReturn), Is.True);

            damageSys.ChangeDamage(target, new DamageSpecifier(Heat, -toDeal), true);
            Assert.Multiple(() =>
            {
                Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.BurnReturn), Is.False);
                Assert.That(lthComp.CurrentEffects.TryGetValue(EffectType.BurnReturn, out var burnReturn), Is.True);
                Assert.That(burnReturn, Is.EqualTo(duration));
            });
            // TODO test that return damage is caustic

            // Test TBI
            var tbiThreshold = config.GetCVar(L5CCVars.AirlossBrainDamageMildThreshold);
            toDeal = FixedPoint2.New(tbiThreshold + 1);
            damageSys.ChangeDamage(target, new DamageSpecifier(Asphyxiation, toDeal), true);
            Assert.That(lthComp.UpcomingEffects.ContainsKey(EffectType.MildBrainDamage), Is.True);

            damageSys.ChangeDamage(target, new DamageSpecifier(Asphyxiation, -toDeal), true);
            EffectType tbi = default;
            foreach (var type in EffectTypeExtensions.MildTBIs)
            {
                if (lthComp.CurrentEffects.ContainsKey(type))
                {
                    tbi = type;
                    break;
                }
            }
            Assert.Multiple(() =>
            {
                Assert.That(lthComp.CurrentEffects.ContainsKey(EffectType.MildBrainDamage), Is.True);
                Assert.That(EffectTypeExtensions.MildTBIs.Contains(tbi), Is.True, "Failed to add a random TBI.");
                Assert.That(lthComp.CurrentEffects.TryGetValue(tbi, out var effect), Is.True);
                Assert.That(effect, Is.EqualTo(duration));
                // TODO test that components/status effects were added
            });

            // Test genetic
            lthComp.CurrentEffects = new();
            var geneticThreshold = config.GetCVar(L5CCVars.GeneticNewEffectRollAmount);
            toDeal = FixedPoint2.New(geneticThreshold + 1);
            damageSys.ChangeDamage(target, new DamageSpecifier(Cellular, toDeal), true);
            Assert.That(lthComp.UpcomingGeneticEffects, Is.EqualTo(1), "Failed to prepare first random effect.");
            damageSys.ChangeDamage(target, new DamageSpecifier(Cellular, toDeal), true);
            Assert.That(lthComp.UpcomingGeneticEffects,  Is.EqualTo(2), "Failed to prepare second random effect.");
            Assert.That(lthComp.UpcomingGeneticEffects,  Is.EqualTo(2), "Failed to prepare second random effect.");

            damageSys.ChangeDamage(target, new DamageSpecifier(Cellular, -toDeal), true);
            Assert.That(lthComp.CurrentEffects.Count, Is.EqualTo(1), "Failed to roll first random effect.");
            damageSys.ChangeDamage(target, new DamageSpecifier(Cellular, -toDeal), true);
            Assert.That(lthComp.CurrentEffects.Count,  Is.EqualTo(2), "Failed to roll second random effect.");
        });
    }
}
