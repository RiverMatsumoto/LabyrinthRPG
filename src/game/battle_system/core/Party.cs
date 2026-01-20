using System.Collections;
using System.Collections.Generic;
using Godot;

public class Party : IEnumerable<Battler>
{
    private const int MAX_CAPACITY = 6;
    private const int MAX_MEMBERS_ROW = 3;
    private Battler[] _members = new Battler[MAX_CAPACITY];
    private int _frontRowCount = 0;
    private int _backRowCount = 0;

    public int MemberCount => _frontRowCount + _backRowCount;

    /// <summary>
    /// Adds a battler to the front row of the party.
    /// </summary>
    /// <param name="battler">The battler to add.</param>
    public void AddToFrontRow(Battler battler)
    {
        if (_frontRowCount < MAX_MEMBERS_ROW)
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
        if (_backRowCount < MAX_MEMBERS_ROW)
        {
            _members[MAX_MEMBERS_ROW + _backRowCount] = battler;
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
            yield return _members[MAX_MEMBERS_ROW + i];
    }
    public Battler GetFrontRowMember(int index) => index < _frontRowCount ? _members[index] : null;
    public Battler GetBackRowMember(int index) => index < _backRowCount ? _members[MAX_MEMBERS_ROW + index] : null;
    public Battler GetMember(int index) => index < _frontRowCount ? _members[index] : _members[MAX_MEMBERS_ROW + index];

    public IEnumerator<Battler> GetEnumerator()
    {
        for (int i = 0; i < _frontRowCount; i++)
            yield return _members[i];
        for (int i = 0; i < _backRowCount; i++)
            yield return _members[MAX_MEMBERS_ROW + i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
