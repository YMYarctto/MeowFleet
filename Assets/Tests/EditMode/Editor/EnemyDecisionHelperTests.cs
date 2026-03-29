using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemyDecisionHelperTests
{
    [Test]
    public void OrderSkills_SortsAscendingByOrder()
    {
        var skills = new List<Skill>
        {
            new TestSkill(99),
            new TestSkill(0),
            new TestSkill(10),
        };

        var result = EnemyDecisionHelper.OrderSkills(skills);

        Assert.That(result[0].Order, Is.EqualTo(0));
        Assert.That(result[1].Order, Is.EqualTo(10));
        Assert.That(result[2].Order, Is.EqualTo(99));
    }

    [Test]
    public void SelectTorpedoRow_PrefersScoutedCoordsBeforeFallbackMaps()
    {
        var preferredCoords = new Queue<Vector2Int>(new[] { new Vector2Int(3, 1) });
        var excludedRows = new List<int>();

        var row = EnemyDecisionHelper.SelectTorpedoRow(
            preferredCoords,
            excludedRows,
            coord => coord.x == 3,
            _ => 1,
            _ => 2);

        Assert.That(row, Is.EqualTo(3));
        Assert.That(preferredCoords.Count, Is.EqualTo(0));
    }

    [Test]
    public void SelectTorpedoRow_SkipsExcludedScoutedRows()
    {
        var preferredCoords = new Queue<Vector2Int>(new[]
        {
            new Vector2Int(2, 0),
            new Vector2Int(4, 2),
        });
        var excludedRows = new List<int> { 2 };

        var row = EnemyDecisionHelper.SelectTorpedoRow(
            preferredCoords,
            excludedRows,
            _ => true,
            _ => 1,
            _ => 3);

        Assert.That(row, Is.EqualTo(4));
        Assert.That(preferredCoords.Count, Is.EqualTo(0));
    }

    [Test]
    public void SelectTorpedoRow_FallsBackToCurrentMapBeforeHuntMap()
    {
        var preferredCoords = new Queue<Vector2Int>();
        var excludedRows = new List<int>();

        var row = EnemyDecisionHelper.SelectTorpedoRow(
            preferredCoords,
            excludedRows,
            _ => false,
            _ => 5,
            _ => 6);

        Assert.That(row, Is.EqualTo(5));
    }

    [Test]
    public void SelectTorpedoRow_FallsBackToHuntMapWhenCurrentMapHasNoRow()
    {
        var preferredCoords = new Queue<Vector2Int>();
        var excludedRows = new List<int>();

        var row = EnemyDecisionHelper.SelectTorpedoRow(
            preferredCoords,
            excludedRows,
            _ => false,
            _ => -1,
            _ => 6);

        Assert.That(row, Is.EqualTo(6));
    }

    class TestSkill : Skill
    {
        readonly int order;

        public TestSkill(int order)
        {
            this.order = order;
        }

        public override int Order => order;

        public override void OnSkillInvoke(Vector2Int target)
        {
        }
    }
}
