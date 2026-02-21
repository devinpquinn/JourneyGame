using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RegionEvent", menuName = "JourneyGame/Region Event")]
public class RegionEventData : ScriptableObject
{
    [SerializeField] private string title = "Untitled Event";
    [SerializeField] private bool repeatable = true;
    [SerializeField] private string startNodeId = "start";
    [SerializeField] private List<EventNodeData> nodes = new List<EventNodeData>();

    public string Title => title;
    public bool Repeatable => repeatable;
    public string StartNodeId => startNodeId;
    public IReadOnlyList<EventNodeData> Nodes => nodes;

    public EventNodeData GetNode(string nodeId)
    {
        for (int index = 0; index < nodes.Count; index++)
        {
            if (nodes[index].NodeId == nodeId)
            {
                return nodes[index];
            }
        }

        return null;
    }

    public EventNodeData GetStartNode()
    {
        EventNodeData startNode = GetNode(startNodeId);
        if (startNode != null)
        {
            return startNode;
        }

        if (nodes.Count > 0)
        {
            return nodes[0];
        }

        return null;
    }
}

[Serializable]
public class EventNodeData
{
    [SerializeField] private string nodeId = "start";
    [SerializeField] [TextArea(3, 10)] private string bodyText;
    [SerializeField] private List<EventEffect> effects = new List<EventEffect>();
    [SerializeField] private HeroAttribute testAttribute = HeroAttribute.None;
    [SerializeField] private string successNodeId;
    [SerializeField] private string failureNodeId;
    [SerializeField] private string nextNodeId;

    public string NodeId => nodeId;
    public string BodyText => bodyText;
    public IReadOnlyList<EventEffect> Effects => effects;
    public HeroAttribute TestAttribute => testAttribute;
    public string SuccessNodeId => successNodeId;
    public string FailureNodeId => failureNodeId;
    public string NextNodeId => nextNodeId;
    public bool HasTest => testAttribute != HeroAttribute.None;
}

[Serializable]
public class EventEffect
{
    [SerializeField] private HeroEffectTarget target;
    [SerializeField] private int amount;

    public HeroEffectTarget Target => target;
    public int Amount => amount;
}