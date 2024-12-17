using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player", story: "If Found Player", category: "Conditions", id: "888f970707989d09fe0df1cefaff2812")]
public partial class PlayerCondition : Condition
{

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
