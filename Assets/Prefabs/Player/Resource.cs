public enum ResourceType
{
    Gold,
    Silver,
    Copper,
}

public class ResourceContainer
{
    public ResourceType resourceType;
    private uint amount_ = 0;

    //--------------------------------------------------

    ResourceContainer(ResourceType type)
    {
        resourceType = type;
    }

    public bool Has(uint amount)
    {
        return amount_ >= amount;
    }

    public void Add(uint change)
    {
        score += change;
    }

    public void Substract(uint change)
    {
        if (change >= amount_) amount_ = 0;
        else                   amount_ -= change;
    }

    public void Set(uint newAmount)
    {
        amount_ = newAmount;
    }

    private static readonly Dictionary<Team, string> RESOURCE_NAMES =
    new Dictionary<Team, string>{
        {ResourceType.Gold,   "Gold"},
        {ResourceType.Silver, "Silver"},
        {ResourceType.Copper, "Copper"},
    };

    public string ResourceName()
    {
        return RESOURCE_NAMES[resourceType];
    }
}
