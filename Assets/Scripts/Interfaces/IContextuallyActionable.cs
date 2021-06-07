using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IContextuallyActionable
{
    void DoThing();
    void DoThingLonger();
    int GetPriority();
}