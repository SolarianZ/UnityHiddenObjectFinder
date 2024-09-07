using System;

namespace GBG.HiddenObjectFinder.Editor
{
    [Flags]
    public enum ObjectTypes
    {
        GameObject = 1,
        Component = 2,
        Other = 4,
    }
}
