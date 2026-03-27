public struct Buff
{
    EBuff type;
    int value;
    int round;

    public readonly bool RoundEnd => round <= 0;
    public readonly int Value => value;

    public Buff(EBuff type, int round, int value=0)
    {
        this.type = type;
        this.value = value;
        this.round = round;
    }

    public void Add(int round,int value = 0)
    {
        this.round = round>this.round? round : this.round;
        this.value += value;
    }

    public void NextRound()
    {
        round--;
    }

    public bool Typeof(EBuff type)
    {
        return this.type == type;
    }

    public override string ToString()
    {
        return $"{{Value:{value},Round:{round}}}";
    }
}
