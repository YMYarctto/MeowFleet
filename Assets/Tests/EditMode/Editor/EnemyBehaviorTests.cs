using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class EnemyBehaviorTests
{
    SeedController seedController;

    [SetUp]
    public void SetUp()
    {
        var seedObject = new GameObject("SeedController_Test");
        seedController = seedObject.AddComponent<SeedController>();
        seedController.SetSeed(123456789L);
    }

    [TearDown]
    public void TearDown()
    {
        if (seedController != null)
        {
            Object.DestroyImmediate(seedController.gameObject);
        }
    }

    [Test]
    public void UpdatePossibleMapAfterDestroy_DoesNotProduceNegativeProbabilities()
    {
        var behavior = CreateBehavior(new Vector2Int(4, 4));
        var shipLayout = CreateTwoCellShipLayout();
        var absoluteLayout = new LayoutDATA(new List<Vector2Int> { new(1, 1), new(2, 1) }, shipLayout.CoreNumber);
        var shipInfo = new KeyValuePair<int, LayoutDATA>(100, shipLayout);

        behavior.UpdatePossibleMapAfterHit(new Vector2Int(1, 1), shipInfo);
        behavior.Remove(new Vector2Int(1, 1));
        behavior.UpdatePossibleMapAfterDestroy(new Vector2Int(2, 1), shipInfo, absoluteLayout);

        foreach (var map in GetProbabilityMaps(behavior).Values)
        {
            if (map == null)
            {
                continue;
            }

            foreach (var kv in GetProbabilityValues(map))
            {
                Assert.That(kv.Value, Is.GreaterThanOrEqualTo(0), $"Probability at {kv.Key} became negative.");
            }
        }
    }

    [Test]
    public void UpdatePossibleMapAfterDestroy_RemovesShipAndAdjacentCellsFromHitMap()
    {
        var behavior = CreateBehavior(new Vector2Int(5, 5));
        var shipLayout = CreateTwoCellShipLayout();
        var absoluteLayout = new LayoutDATA(new List<Vector2Int> { new(1, 1), new(2, 1) }, shipLayout.CoreNumber);
        var shipInfo = new KeyValuePair<int, LayoutDATA>(100, shipLayout);

        behavior.UpdatePossibleMapAfterDestroy(new Vector2Int(2, 1), shipInfo, absoluteLayout);

        var hitMap = GetHitMap(behavior);
        var invalidCoords = new HashSet<Vector2Int>(absoluteLayout.ToList);
        foreach (var coord in absoluteLayout.GetAdjacentCellsInMap(Vector2Int.zero))
        {
            invalidCoords.Add(coord);
        }

        foreach (var coord in invalidCoords)
        {
            Assert.That(hitMap.Contains(coord), Is.False, $"Destroyed ship coord {coord} should be removed from hit_map.");
        }
    }

    [Test]
    public void CalculateHuntMap_FallsBackToRemainingHitMapWhenProbabilityMapIsEmpty()
    {
        var behavior = CreateBehavior(new Vector2Int(3, 3));
        var fallback = new Vector2Int(2, 2);

        SetPrivateField(behavior, "hit_map", new List<Vector2Int> { fallback });
        GetProbabilityMaps(behavior)[-1] = new ProbabilityMap();

        var result = behavior.CalculateHuntMap();

        Assert.That(result, Is.EqualTo(fallback));
    }

    [Test]
    public void CalculatePossibleMapWithoutRow_FallsBackToAvailableRowsWhenProbabilityMapIsEmpty()
    {
        var behavior = CreateBehavior(new Vector2Int(5, 5));

        SetPrivateField(behavior, "hit_map", new List<Vector2Int>
        {
            new(2, 0),
            new(2, 3),
            new(4, 1),
        });
        GetProbabilityMaps(behavior)[-1] = new ProbabilityMap();

        var row = behavior.CalculatePossibleMapWithoutRow(new List<int> { 2 });

        Assert.That(row, Is.EqualTo(4));
    }

    [Test]
    public void CalculateCurrentMapWithoutRow_ReturnsMinusOneWhenCurrentMapHasNoAvailableRows()
    {
        var behavior = CreateBehavior(new Vector2Int(4, 4));

        SetPrivateField(behavior, "current_target_index", 100);
        GetProbabilityMaps(behavior)[100] = new ProbabilityMap();

        var row = behavior.CalculateCurrentMapWithoutRow(new List<int>());

        Assert.That(row, Is.EqualTo(-1));
    }

    [Test]
    public void CalculatePossibleMap_FallsBackToRemainingHitMapWhenCurrentTargetMapIsNull()
    {
        var behavior = CreateBehavior(new Vector2Int(3, 3));
        var fallback = new Vector2Int(2, 1);

        SetPrivateField(behavior, "current_target_index", 100);
        SetPrivateField(behavior, "hit_map", new List<Vector2Int> { fallback });

        var maps = GetProbabilityMaps(behavior);
        maps[-1] = new ProbabilityMap();
        maps[100] = null;

        var result = behavior.CalculatePossibleMap();

        Assert.That(result, Is.EqualTo(fallback));
    }

    [Test]
    public void CalculateSkillRange_FallsBackToHuntMapWhenCurrentTargetMapIsNull()
    {
        var behavior = CreateBehavior(new Vector2Int(4, 4));
        var range = new LayoutDATA(new List<Vector2Int> { Vector2Int.zero, Vector2Int.right }, 0);

        SetPrivateField(behavior, "current_target_index", 100);
        GetProbabilityMaps(behavior)[100] = null;

        var result = behavior.CalculateSkillRange(new Vector2Int(1, 1), range);

        var possibleRanges = range.AllLayout();
        Assert.That(possibleRanges.Exists(layout => AreCoordsEqual(layout.ToList, result)), Is.True);
    }

    [Test]
    public void UpdatePossibleMapAfterDestroy_SwitchesToRemainingTrackedTarget()
    {
        var shipA = CreateTwoCellShipLayout();
        var shipB = new LayoutDATA(new List<Vector2Int> { Vector2Int.zero, Vector2Int.up }, 1);
        var behavior = CreateBehavior(new Vector2Int(5, 5), shipA, shipB);

        behavior.UpdatePossibleMapAfterHit(new Vector2Int(1, 1), new KeyValuePair<int, LayoutDATA>(100, shipA));
        behavior.Remove(new Vector2Int(1, 1));
        behavior.UpdatePossibleMapAfterHit(new Vector2Int(3, 3), new KeyValuePair<int, LayoutDATA>(200, shipB));
        behavior.Remove(new Vector2Int(3, 3));

        var absoluteLayoutB = new LayoutDATA(new List<Vector2Int> { new(3, 3), new(3, 4) }, shipB.CoreNumber);
        behavior.UpdatePossibleMapAfterDestroy(new Vector2Int(3, 4), new KeyValuePair<int, LayoutDATA>(200, shipB), absoluteLayoutB);

        var currentTarget = GetPrivateField<int>(behavior, "current_target_index");
        Assert.That(currentTarget, Is.EqualTo(100));
    }

    EnemyBehavior CreateBehavior(Vector2Int size)
    {
        return CreateBehavior(size, CreateTwoCellShipLayout());
    }

    EnemyBehavior CreateBehavior(Vector2Int size, params LayoutDATA[] shipLayouts)
    {
        var behavior = new EnemyBehavior();
        behavior.Init(size, new List<LayoutDATA>(shipLayouts));
        return behavior;
    }

    LayoutDATA CreateTwoCellShipLayout()
    {
        return new LayoutDATA(new List<Vector2Int> { Vector2Int.zero, Vector2Int.right }, 1);
    }

    Dictionary<int, ProbabilityMap> GetProbabilityMaps(EnemyBehavior behavior)
    {
        return GetPrivateField<Dictionary<int, ProbabilityMap>>(behavior, "map_dict");
    }

    List<Vector2Int> GetHitMap(EnemyBehavior behavior)
    {
        return GetPrivateField<List<Vector2Int>>(behavior, "hit_map");
    }

    Dictionary<Vector2Int, int> GetProbabilityValues(ProbabilityMap map)
    {
        var probabilityMapData = GetPrivateField<ProbabilityMapDATA>(map, "probability_map");
        return GetPrivateField<Dictionary<Vector2Int, int>>(probabilityMapData, "_map");
    }

    T GetPrivateField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"Field {fieldName} was not found on {target.GetType().Name}.");
        return (T)field.GetValue(target);
    }

    void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"Field {fieldName} was not found on {target.GetType().Name}.");
        field.SetValue(target, value);
    }

    bool AreCoordsEqual(List<Vector2Int> a, List<Vector2Int> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }
}
