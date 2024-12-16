using UnityEngine;
using System.Collections.Generic;

public class ResourceBank : MonoBehaviour
{
    private Dictionary<ResourceType, ResourceContainer> containers_;

    void Start()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            containers_[type] = new ResourceContainer(type);
        }
    }

    public bool Has(ResourceType type, uint amount)
    {
        return containers_[type].Has(amount);
    }

    public void Add(ResourceType type, uint change)
    {
        containers_[type].Add(change);
    }

    public void Subtract(ResourceType type, uint change)
    {
        containers_[type].Substract(change);
    }

    public void Set(ResourceType type, uint newAmount)
    {
        containers_[type].Set(newAmount);
    }
}