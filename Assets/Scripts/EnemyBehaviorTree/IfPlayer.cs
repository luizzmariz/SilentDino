using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "New Sequencing", story: "If [Player]", category: "Flow", id: "89d5bf9e0b7b159c85295016ed911758")]
public partial class IfPlayer : Composite
{
    [SerializeReference] public BlackboardVariable<bool> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

