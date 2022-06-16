using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{

    public interface ICollider
    {
        delegate void ColliderDelegate(object owner, List<BaseCollider> array);
        event ColliderDelegate OnEnterEvent;
        event ColliderDelegate OnStayEvent;
        event ColliderDelegate OnLeaveEvent;
    }
}