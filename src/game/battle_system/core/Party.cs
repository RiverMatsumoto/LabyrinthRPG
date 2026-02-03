using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

[Serializable]
public class Party : IEnumerable<Battler>
{
    private readonly int MaxCapacity;
    public readonly int MaxMembersRow = 3;
    private readonly Battler[] _members;
    private int _frontRowCount = 0;
    private int _backRowCount = 0;

    public int MemberCount => _frontRowCount + _backRowCount;
    public int FrontRowCount => _frontRowCount;
    public int BackRowCount => _backRowCount;

    public Party()
    {
        MaxMembersRow = 3;
        MaxCapacity = 2 * MaxMembersRow;
        _members = new Battler[MaxCapacity];
    }

    // Used for enemy party ill probably hardcode the size somewhere
    public Party(int maxMembersRow)
    {
        MaxMembersRow = maxMembersRow;
        MaxCapacity = 2 * MaxMembersRow;
        _members = new Battler[MaxCapacity];
    }

    /// <summary>
    /// Adds a battler to the front row of the party.
    /// </summary>
    /// <param name="battler">The battler to add.</param>
    public void AddToFrontRow(Battler battler)
    {
        if (_frontRowCount < MaxMembersRow)
        {
            _members[_frontRowCount] = battler;
            _frontRowCount++;
        }
        else
            GD.PrintErr("Tried to add Character to front row. Front row is full");
    }

    /// <summary>
    /// Adds a battler to the back row of the party.
    /// </summary>
    /// <param name="battler">The battler to add.</param>
    public void AddToBackRow(Battler battler)
    {
        if (_backRowCount < MaxMembersRow)
        {
            _members[MaxMembersRow + _backRowCount] = battler;
            _backRowCount++;
        }
        else
            GD.PrintErr("Tried to add Character to back row. Back row is full");
    }

    public IEnumerable<Battler> GetFrontRowMembers()
    {
        for (int i = 0; i < _frontRowCount; i++)
            yield return _members[i];
    }

    public IEnumerable<Battler> GetBackRowMembers()
    {
        for (int i = 0; i < _backRowCount; i++)
            yield return _members[MaxMembersRow + i];
    }

    public IReadOnlyList<Battler> GetFrontRowBattlersAsList()
    {
        List<Battler> battlers = new List<Battler>();
        for (int i = 0; i < _frontRowCount; i++)
            battlers.Add(_members[i]);
        return battlers;
    }

    public IReadOnlyList<Battler> GetBackRowBattlersAsList()
    {
        List<Battler> battlers = new List<Battler>();
        for (int i = 0; i < _backRowCount; i++)
            battlers.Add(_members[MaxMembersRow + i]);
        return battlers;
    }

    // Front row battlers are first, mostly if you wanted to index the battlers as if it was an array
    // You lose front row and back row information using this method
    public IReadOnlyList<Battler> GetAllBattlersAsList()
    {
        List<Battler> battlers = new List<Battler>();
        for (int i = 0; i < _frontRowCount; i++)
            battlers.Add(_members[i]);
        for (int i = 0; i < _backRowCount; i++)
            battlers.Add(_members[MaxMembersRow + i]);
        return battlers;
    }

    public Battler GetFrontRowMember(int index) => index < _frontRowCount ? _members[index] : null;
    public Battler GetBackRowMember(int index) => index < _backRowCount ? _members[MaxMembersRow + index] : null;
    public Battler GetMember(int index) => index < _frontRowCount ? _members[index] : _members[MaxMembersRow + index];
    // public IReadOnlyList<Battler> GetAllBattlers() =>

    public IEnumerator<Battler> GetEnumerator()
    {
        for (int i = 0; i < _frontRowCount; i++)
            yield return _members[i];
        for (int i = 0; i < _backRowCount; i++)
            yield return _members[MaxMembersRow + i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
